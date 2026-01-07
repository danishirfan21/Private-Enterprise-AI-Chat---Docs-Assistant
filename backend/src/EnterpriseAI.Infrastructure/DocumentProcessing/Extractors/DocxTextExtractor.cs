using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace EnterpriseAI.Infrastructure.DocumentProcessing.Extractors;

/// <summary>
/// Extracts text from DOCX documents using DocumentFormat.OpenXml.
/// </summary>
public class DocxTextExtractor : ITextExtractor
{
    public IEnumerable<string> SupportedExtensions => new[] { ".docx" };

    public async Task<string> ExtractTextAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var text = new StringBuilder();

            using (var document = WordprocessingDocument.Open(stream, false))
            {
                if (document.MainDocumentPart?.Document.Body != null)
                {
                    foreach (var paragraph in document.MainDocumentPart.Document.Body.Descendants<Paragraph>())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var paragraphText = paragraph.InnerText;
                        if (!string.IsNullOrWhiteSpace(paragraphText))
                        {
                            text.AppendLine(paragraphText);
                        }
                    }
                }
            }

            return text.ToString().Trim();
        }, cancellationToken);
    }
}
