using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Core.Interfaces;

/// <summary>
/// Interface for vector storage and similarity search.
/// </summary>
public interface IVectorStore
{
    /// <summary>
    /// Adds a vector embedding to the store.
    /// </summary>
    Task AddAsync(VectorEmbedding embedding, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple vector embeddings to the store.
    /// </summary>
    Task AddRangeAsync(IEnumerable<VectorEmbedding> embeddings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for the most similar vectors to the query vector.
    /// </summary>
    /// <param name="queryVector">The query embedding vector.</param>
    /// <param name="topK">Number of top results to return.</param>
    /// <param name="similarityThreshold">Minimum similarity score (0-1).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of search results ordered by similarity score (highest first).</returns>
    Task<IEnumerable<SearchResult>> SearchAsync(
        float[] queryVector,
        int topK = 5,
        float similarityThreshold = 0.7f,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all embeddings associated with a specific document.
    /// </summary>
    Task RemoveByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total number of embeddings in the store.
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
