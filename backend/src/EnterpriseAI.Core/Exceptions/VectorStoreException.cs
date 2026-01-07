namespace EnterpriseAI.Core.Exceptions;

/// <summary>
/// Exception thrown when vector store operations fail.
/// </summary>
public class VectorStoreException : Exception
{
    public VectorStoreException(string message) : base(message)
    {
    }

    public VectorStoreException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
