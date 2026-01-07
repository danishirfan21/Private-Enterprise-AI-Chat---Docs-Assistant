using EnterpriseAI.Core.Interfaces;
using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Infrastructure.DocumentProcessing;

/// <summary>
/// Splits text into overlapping chunks for embedding and retrieval.
/// </summary>
public class TextChunker : ITextChunker
{
    public IEnumerable<DocumentChunk> ChunkText(
        string text,
        Guid documentId,
        int chunkSize = 512,
        int overlap = 50)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Enumerable.Empty<DocumentChunk>();
        }

        var chunks = new List<DocumentChunk>();

        // Simple word-based chunking with overlap
        var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
        {
            return chunks;
        }

        int chunkIndex = 0;
        int startWord = 0;

        while (startWord < words.Length)
        {
            // Take up to chunkSize words
            var chunkWords = words.Skip(startWord).Take(chunkSize).ToArray();
            var chunkText = string.Join(" ", chunkWords);

            var chunk = new DocumentChunk
            {
                DocumentId = documentId,
                Content = chunkText,
                ChunkIndex = chunkIndex,
                StartPosition = startWord,
                EndPosition = startWord + chunkWords.Length,
                Metadata = new Dictionary<string, object>
                {
                    { "word_count", chunkWords.Length },
                    { "char_count", chunkText.Length }
                }
            };

            chunks.Add(chunk);

            // Move forward by (chunkSize - overlap) to create overlapping chunks
            startWord += Math.Max(1, chunkSize - overlap);
            chunkIndex++;
        }

        return chunks;
    }
}
