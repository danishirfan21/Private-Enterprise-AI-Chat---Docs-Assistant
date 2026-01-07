namespace EnterpriseAI.Core.Models;

/// <summary>
/// Represents a chat message in a conversation.
/// </summary>
public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public List<SourceAttribution>? Sources { get; set; }
}

/// <summary>
/// Represents a source document used in RAG responses.
/// </summary>
public class SourceAttribution
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public float RelevanceScore { get; set; }
}
