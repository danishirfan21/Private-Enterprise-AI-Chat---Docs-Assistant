using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace EnterpriseAI.Infrastructure.DocumentProcessing.Extractors;

/// <summary>
/// Extracts text from PDF documents using PdfPig.
/// </summary>
public class PdfTextExtractor : ITextExtractor
{
    public IEnumerable<string> SupportedExtensions => new[] { ".pdf" };

    public async Task<string> ExtractTextAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var text = new StringBuilder();

            using (var document = PdfDocument.Open(stream))
            {
                foreach (Page page in document.GetPages())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var pageText = page.Text;
                    if (!string.IsNullOrWhiteSpace(pageText))
                    {
                        text.AppendLine(pageText);
                        text.AppendLine(); // Add spacing between pages
                    }
                }
            }

            return text.ToString().Trim();
        }, cancellationToken);
    }
}
