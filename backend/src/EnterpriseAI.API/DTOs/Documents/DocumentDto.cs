namespace EnterpriseAI.API.DTOs.Documents;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public int ChunkCount { get; set; }
    public string Status { get; set; } = string.Empty;
}
