namespace EnterpriseAI.Core.Interfaces;

/// <summary>
/// Interface for generating text embeddings.
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Generates an embedding vector for the given text.
    /// </summary>
    /// <param name="text">The text to embed.</param>
    /// <param name="modelName">The embedding model to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The embedding vector as an array of floats.</returns>
    Task<float[]> GenerateEmbeddingAsync(
        string text,
        string? modelName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embeddings for multiple texts in batch.
    /// </summary>
    /// <param name="texts">The texts to embed.</param>
    /// <param name="modelName">The embedding model to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of embedding vectors.</returns>
    Task<IEnumerable<float[]>> GenerateEmbeddingsAsync(
        IEnumerable<string> texts,
        string? modelName = null,
        CancellationToken cancellationToken = default);
}
