namespace EnterpriseAI.Core.Exceptions;

/// <summary>
/// Exception thrown when document processing fails.
/// </summary>
public class DocumentProcessingException : Exception
{
    public DocumentProcessingException(string message) : base(message)
    {
    }

    public DocumentProcessingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
