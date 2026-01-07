namespace EnterpriseAI.Core.Models;

/// <summary>
/// Represents a chat conversation with message history.
/// </summary>
public class Conversation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "New Conversation";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<ChatMessage> Messages { get; set; } = new();
    public int MessageCount => Messages.Count;
}
