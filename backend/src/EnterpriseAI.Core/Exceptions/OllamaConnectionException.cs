namespace EnterpriseAI.Core.Exceptions;

/// <summary>
/// Exception thrown when connection to Ollama fails.
/// </summary>
public class OllamaConnectionException : Exception
{
    public OllamaConnectionException(string message) : base(message)
    {
    }

    public OllamaConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
