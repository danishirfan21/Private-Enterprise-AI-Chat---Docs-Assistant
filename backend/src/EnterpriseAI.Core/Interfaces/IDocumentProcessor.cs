using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Core.Interfaces;

/// <summary>
/// Interface for processing documents (extraction, chunking, embedding).
/// </summary>
public interface IDocumentProcessor
{
    /// <summary>
    /// Processes a document: extracts text, chunks it, generates embeddings, and stores them.
    /// </summary>
    /// <param name="document">The document to process.</param>
    /// <param name="fileStream">The document file stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of chunks created.</returns>
    Task<int> ProcessDocumentAsync(
        Document document,
        Stream fileStream,
        CancellationToken cancellationToken = default);
}
