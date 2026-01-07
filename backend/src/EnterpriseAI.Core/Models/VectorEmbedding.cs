namespace EnterpriseAI.Core.Models;

/// <summary>
/// Represents a vector embedding with its associated text and metadata.
/// </summary>
public class VectorEmbedding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public Guid ChunkId { get; set; }
    public float[] Vector { get; set; } = Array.Empty<float>();
    public string Text { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
