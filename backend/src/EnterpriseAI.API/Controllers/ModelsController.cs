using EnterpriseAI.API.DTOs.Models;
using EnterpriseAI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModelsController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly ILogger<ModelsController> _logger;

    public ModelsController(
        IAIService aiService,
        ILogger<ModelsController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of available Ollama models.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ModelInfoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModels(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching available models");

        var models = await _aiService.GetAvailableModelsAsync(cancellationToken);

        var modelDtos = models.Select(m => new ModelInfoDto
        {
            Name = m.Name,
            Size = m.Size,
            ModifiedAt = m.ModifiedAt
        });

        return Ok(modelDtos);
    }
}
