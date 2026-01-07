using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Core.Interfaces;

/// <summary>
/// Interface for AI chat completion services.
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Generates a streaming chat completion response.
    /// </summary>
    /// <param name="messages">The conversation history including the user's message.</param>
    /// <param name="modelName">The model to use for completion.</param>
    /// <param name="temperature">Controls randomness (0-1).</param>
    /// <param name="maxTokens">Maximum tokens to generate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async stream of response tokens.</returns>
    IAsyncEnumerable<string> GetStreamingCompletionAsync(
        IEnumerable<ChatMessage> messages,
        string modelName,
        float temperature = 0.7f,
        int maxTokens = 2000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of available models from Ollama.
    /// </summary>
    Task<IEnumerable<ModelInfo>> GetAvailableModelsAsync(CancellationToken cancellationToken = default);
}
