namespace EnterpriseAI.Infrastructure.DocumentProcessing.Extractors;

/// <summary>
/// Extracts text from plain text files.
/// </summary>
public class TxtTextExtractor : ITextExtractor
{
    public IEnumerable<string> SupportedExtensions => new[] { ".txt" };

    public async Task<string> ExtractTextAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream);
        var text = await reader.ReadToEndAsync(cancellationToken);
        return text.Trim();
    }
}
