using System.Collections.Concurrent;
using EnterpriseAI.API.DTOs.Documents;
using EnterpriseAI.Core.Interfaces;
using EnterpriseAI.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentProcessor _documentProcessor;
    private readonly IVectorStore _vectorStore;
    private readonly ILogger<DocumentsController> _logger;
    private readonly IConfiguration _configuration;

    // In-memory document storage (replace with database in production)
    private static readonly ConcurrentDictionary<Guid, Document> _documents = new();

    public DocumentsController(
        IDocumentProcessor documentProcessor,
        IVectorStore vectorStore,
        IConfiguration configuration,
        ILogger<DocumentsController> logger)
    {
        _documentProcessor = documentProcessor;
        _vectorStore = vectorStore;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a document for processing and embedding.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadDocument(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "No file provided" });
        }

        var maxFileSizeMB = _configuration.GetValue<int>("DocumentProcessing:MaxFileSizeMB", 10);
        var maxFileSize = maxFileSizeMB * 1024 * 1024;

        if (file.Length > maxFileSize)
        {
            return BadRequest(new { error = $"File size exceeds maximum of {maxFileSizeMB}MB" });
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var supportedTypes = _configuration.GetSection("DocumentProcessing:SupportedTypes").Get<string[]>()
            ?? new[] { "pdf", "docx", "txt" };

        if (!supportedTypes.Contains(extension.TrimStart('.')))
        {
            return BadRequest(new { error = $"File type {extension} is not supported. Supported types: {string.Join(", ", supportedTypes)}" });
        }

        var document = new Document
        {
            FileName = file.FileName,
            MimeType = file.ContentType,
            FileSize = file.Length,
            Status = DocumentStatus.Processing
        };

        _documents[document.Id] = document;

        _logger.LogInformation("Starting document processing for {FileName}", file.FileName);

        try
        {
            using var stream = file.OpenReadStream();
            var chunkCount = await _documentProcessor.ProcessDocumentAsync(document, stream, cancellationToken);

            document.ChunkCount = chunkCount;
            document.Status = DocumentStatus.Processed;

            _logger.LogInformation("Successfully processed document {DocumentId} with {ChunkCount} chunks",
                document.Id, chunkCount);

            var documentDto = new DocumentDto
            {
                Id = document.Id,
                FileName = document.FileName,
                MimeType = document.MimeType,
                FileSize = document.FileSize,
                UploadedAt = document.UploadedAt,
                ChunkCount = document.ChunkCount,
                Status = document.Status.ToString()
            };

            return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, documentDto);
        }
        catch (Exception ex)
        {
            document.Status = DocumentStatus.Failed;
            _logger.LogError(ex, "Failed to process document {DocumentId}", document.Id);
            return StatusCode(500, new { error = $"Failed to process document: {ex.Message}" });
        }
    }

    /// <summary>
    /// Gets a list of all uploaded documents.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
    public IActionResult GetDocuments()
    {
        var documents = _documents.Values
            .OrderByDescending(d => d.UploadedAt)
            .Select(d => new DocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                MimeType = d.MimeType,
                FileSize = d.FileSize,
                UploadedAt = d.UploadedAt,
                ChunkCount = d.ChunkCount,
                Status = d.Status.ToString()
            });

        return Ok(documents);
    }

    /// <summary>
    /// Gets a specific document by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetDocument(Guid id)
    {
        if (!_documents.TryGetValue(id, out var document))
        {
            return NotFound(new { error = $"Document {id} not found" });
        }

        var documentDto = new DocumentDto
        {
            Id = document.Id,
            FileName = document.FileName,
            MimeType = document.MimeType,
            FileSize = document.FileSize,
            UploadedAt = document.UploadedAt,
            ChunkCount = document.ChunkCount,
            Status = document.Status.ToString()
        };

        return Ok(documentDto);
    }

    /// <summary>
    /// Deletes a document and its associated embeddings.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(Guid id, CancellationToken cancellationToken)
    {
        if (!_documents.TryRemove(id, out var document))
        {
            return NotFound(new { error = $"Document {id} not found" });
        }

        // Remove associated embeddings from vector store
        await _vectorStore.RemoveByDocumentIdAsync(id, cancellationToken);

        _logger.LogInformation("Deleted document {DocumentId} ({FileName})", id, document.FileName);

        return NoContent();
    }
}
