namespace EnterpriseAI.Core.Models;

/// <summary>
/// Represents a search result from vector similarity search.
/// </summary>
public class SearchResult
{
    public VectorEmbedding Embedding { get; set; } = null!;
    public float SimilarityScore { get; set; }
}
