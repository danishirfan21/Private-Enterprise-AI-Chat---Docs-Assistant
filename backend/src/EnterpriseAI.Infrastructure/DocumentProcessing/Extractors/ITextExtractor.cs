namespace EnterpriseAI.Infrastructure.DocumentProcessing.Extractors;

/// <summary>
/// Interface for extracting text from different document types.
/// </summary>
public interface ITextExtractor
{
    /// <summary>
    /// Supported file extensions for this extractor (e.g., ".pdf", ".docx").
    /// </summary>
    IEnumerable<string> SupportedExtensions { get; }

    /// <summary>
    /// Extracts plain text from the document stream.
    /// </summary>
    Task<string> ExtractTextAsync(Stream stream, CancellationToken cancellationToken = default);
}
