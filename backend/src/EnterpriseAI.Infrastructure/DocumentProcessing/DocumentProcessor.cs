using EnterpriseAI.Core.Exceptions;
using EnterpriseAI.Core.Interfaces;
using EnterpriseAI.Core.Models;
using EnterpriseAI.Infrastructure.DocumentProcessing.Extractors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EnterpriseAI.Infrastructure.DocumentProcessing;

/// <summary>
/// Orchestrates document processing: extraction, chunking, embedding, and storage.
/// </summary>
public class DocumentProcessor : IDocumentProcessor
{
    private readonly IEnumerable<ITextExtractor> _extractors;
    private readonly ITextChunker _textChunker;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorStore;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DocumentProcessor> _logger;

    public DocumentProcessor(
        IEnumerable<ITextExtractor> extractors,
        ITextChunker textChunker,
        IEmbeddingService embeddingService,
        IVectorStore vectorStore,
        IConfiguration configuration,
        ILogger<DocumentProcessor> logger)
    {
        _extractors = extractors;
        _textChunker = textChunker;
        _embeddingService = embeddingService;
        _vectorStore = vectorStore;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<int> ProcessDocumentAsync(
        Document document,
        Stream fileStream,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing document {DocumentId}: {FileName}", document.Id, document.FileName);

            // Step 1: Extract text
            var fileExtension = Path.GetExtension(document.FileName).ToLowerInvariant();
            var extractor = _extractors.FirstOrDefault(e => e.SupportedExtensions.Contains(fileExtension));

            if (extractor == null)
            {
                throw new DocumentProcessingException($"No extractor found for file type: {fileExtension}");
            }

            var extractedText = await extractor.ExtractTextAsync(fileStream, cancellationToken);

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                throw new DocumentProcessingException("No text could be extracted from the document");
            }

            _logger.LogDebug("Extracted {CharCount} characters from document", extractedText.Length);

            // Step 2: Chunk text
            var chunkSize = _configuration.GetValue<int>("DocumentProcessing:ChunkSize", 512);
            var chunkOverlap = _configuration.GetValue<int>("DocumentProcessing:ChunkOverlap", 50);
            var maxChunks = _configuration.GetValue<int>("DocumentProcessing:MaxChunksPerDocument", 1000);

            var chunks = _textChunker.ChunkText(extractedText, document.Id, chunkSize, chunkOverlap).ToList();

            if (chunks.Count > maxChunks)
            {
                _logger.LogWarning("Document has {ChunkCount} chunks, limiting to {MaxChunks}", chunks.Count, maxChunks);
                chunks = chunks.Take(maxChunks).ToList();
            }

            _logger.LogInformation("Created {ChunkCount} chunks from document", chunks.Count);

            // Step 3: Generate embeddings
            var chunkTexts = chunks.Select(c => c.Content).ToList();
            var embeddings = await _embeddingService.GenerateEmbeddingsAsync(chunkTexts, cancellationToken: cancellationToken);
            var embeddingList = embeddings.ToList();

            _logger.LogInformation("Generated {EmbeddingCount} embeddings", embeddingList.Count);

            // Step 4: Store embeddings in vector store
            var vectorEmbeddings = chunks.Zip(embeddingList, (chunk, embedding) => new VectorEmbedding
            {
                DocumentId = document.Id,
                ChunkId = chunk.Id,
                Vector = embedding,
                Text = chunk.Content,
                Metadata = new Dictionary<string, object>
                {
                    { "chunk_index", chunk.ChunkIndex },
                    { "document_name", document.FileName }
                }
            }).ToList();

            await _vectorStore.AddRangeAsync(vectorEmbeddings, cancellationToken);

            _logger.LogInformation("Successfully processed document {DocumentId} with {ChunkCount} chunks",
                document.Id, chunks.Count);

            return chunks.Count;
        }
        catch (Exception ex) when (ex is not DocumentProcessingException)
        {
            _logger.LogError(ex, "Error processing document {DocumentId}", document.Id);
            throw new DocumentProcessingException($"Failed to process document: {ex.Message}", ex);
        }
    }
}
