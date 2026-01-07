namespace EnterpriseAI.API.DTOs.Chat;

public class ConversationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int MessageCount { get; set; }
}

public class ConversationDetailDto : ConversationDto
{
    public List<MessageDto> Messages { get; set; } = new();
}
