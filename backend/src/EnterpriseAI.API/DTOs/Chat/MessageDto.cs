namespace EnterpriseAI.API.DTOs.Chat;

public class MessageDto
{
    public Guid Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public List<SourceAttributionDto>? Sources { get; set; }
}

public class SourceAttributionDto
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public float RelevanceScore { get; set; }
}
