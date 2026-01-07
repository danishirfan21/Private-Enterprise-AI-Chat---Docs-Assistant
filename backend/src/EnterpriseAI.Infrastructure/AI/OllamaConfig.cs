namespace EnterpriseAI.Infrastructure.AI;

/// <summary>
/// Configuration for Ollama connection and settings.
/// </summary>
public class OllamaConfig
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string DefaultChatModel { get; set; } = "llama3:latest";
    public string DefaultEmbeddingModel { get; set; } = "nomic-embed-text";
    public int RequestTimeout { get; set; } = 300;
    public int MaxRetries { get; set; } = 3;
}
