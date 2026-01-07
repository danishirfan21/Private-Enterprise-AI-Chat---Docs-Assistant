namespace EnterpriseAI.Core.Models;

/// <summary>
/// Represents information about an available AI model.
/// </summary>
public class ModelInfo
{
    public string Name { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public DateTime? ModifiedAt { get; set; }
    public Dictionary<string, object> Details { get; set; } = new();
}
