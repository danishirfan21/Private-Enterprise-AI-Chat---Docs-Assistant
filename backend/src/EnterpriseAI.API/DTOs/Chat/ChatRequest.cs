namespace EnterpriseAI.API.DTOs.Chat;

public class ChatRequest
{
    public Guid? ConversationId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ModelName { get; set; } = "llama3:latest";
    public float Temperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 2000;
    public bool UseRAG { get; set; } = true;
    public string? SystemPrompt { get; set; }
}
