using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Core.Interfaces;

/// <summary>
/// Interface for splitting text into chunks.
/// </summary>
public interface ITextChunker
{
    /// <summary>
    /// Splits text into overlapping chunks.
    /// </summary>
    /// <param name="text">The text to chunk.</param>
    /// <param name="documentId">The ID of the source document.</param>
    /// <param name="chunkSize">Target chunk size in tokens.</param>
    /// <param name="overlap">Number of overlapping tokens between chunks.</param>
    /// <returns>List of document chunks.</returns>
    IEnumerable<DocumentChunk> ChunkText(
        string text,
        Guid documentId,
        int chunkSize = 512,
        int overlap = 50);
}
