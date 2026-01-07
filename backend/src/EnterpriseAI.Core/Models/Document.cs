namespace EnterpriseAI.Core.Models;

/// <summary>
/// Represents an uploaded document.
/// </summary>
public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int ChunkCount { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
}

/// <summary>
/// Document processing status.
/// </summary>
public enum DocumentStatus
{
    Pending,
    Processing,
    Processed,
    Failed
}
