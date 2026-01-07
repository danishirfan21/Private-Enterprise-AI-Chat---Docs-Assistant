namespace EnterpriseAI.API.DTOs.Models;

public class ModelInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public DateTime? ModifiedAt { get; set; }
}
