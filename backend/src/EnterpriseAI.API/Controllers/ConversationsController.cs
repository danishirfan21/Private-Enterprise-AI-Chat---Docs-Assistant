using EnterpriseAI.API.DTOs.Chat;
using EnterpriseAI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationsController : ControllerBase
{
    private readonly IChatHistoryService _chatHistoryService;
    private readonly ILogger<ConversationsController> _logger;

    public ConversationsController(
        IChatHistoryService chatHistoryService,
        ILogger<ConversationsController> logger)
    {
        _chatHistoryService = chatHistoryService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all conversations, ordered by most recent first.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations(CancellationToken cancellationToken)
    {
        var conversations = await _chatHistoryService.GetConversationsAsync(cancellationToken);

        var conversationDtos = conversations.Select(c => new ConversationDto
        {
            Id = c.Id,
            Title = c.Title,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            MessageCount = c.MessageCount
        });

        return Ok(conversationDtos);
    }

    /// <summary>
    /// Gets a specific conversation with all its messages.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ConversationDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConversation(Guid id, CancellationToken cancellationToken)
    {
        var conversation = await _chatHistoryService.GetConversationAsync(id, cancellationToken);

        if (conversation == null)
        {
            return NotFound(new { error = $"Conversation {id} not found" });
        }

        var conversationDto = new ConversationDetailDto
        {
            Id = conversation.Id,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            MessageCount = conversation.MessageCount,
            Messages = conversation.Messages.Select(m => new MessageDto
            {
                Id = m.Id,
                Role = m.Role,
                Content = m.Content,
                Timestamp = m.Timestamp,
                Sources = m.Sources?.Select(s => new SourceAttributionDto
                {
                    DocumentId = s.DocumentId,
                    FileName = s.FileName,
                    RelevanceScore = s.RelevanceScore
                }).ToList()
            }).ToList()
        };

        return Ok(conversationDto);
    }

    /// <summary>
    /// Gets all messages in a conversation.
    /// </summary>
    [HttpGet("{id}/messages")]
    [ProducesResponseType(typeof(IEnumerable<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessages(Guid id, CancellationToken cancellationToken)
    {
        var conversation = await _chatHistoryService.GetConversationAsync(id, cancellationToken);

        if (conversation == null)
        {
            return NotFound(new { error = $"Conversation {id} not found" });
        }

        var messages = await _chatHistoryService.GetMessagesAsync(id, cancellationToken);

        var messageDtos = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            Role = m.Role,
            Content = m.Content,
            Timestamp = m.Timestamp,
            Sources = m.Sources?.Select(s => new SourceAttributionDto
            {
                DocumentId = s.DocumentId,
                FileName = s.FileName,
                RelevanceScore = s.RelevanceScore
            }).ToList()
        });

        return Ok(messageDtos);
    }

    /// <summary>
    /// Creates a new conversation.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateConversation(
        [FromBody] CreateConversationRequest? request,
        CancellationToken cancellationToken)
    {
        var conversation = await _chatHistoryService.CreateConversationAsync(
            request?.Title,
            cancellationToken);

        var conversationDto = new ConversationDto
        {
            Id = conversation.Id,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            MessageCount = conversation.MessageCount
        };

        return CreatedAtAction(nameof(GetConversation), new { id = conversation.Id }, conversationDto);
    }

    /// <summary>
    /// Updates a conversation's title.
    /// </summary>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateConversation(
        Guid id,
        [FromBody] UpdateConversationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _chatHistoryService.UpdateConversationTitleAsync(id, request.Title, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound(new { error = $"Conversation {id} not found" });
        }
    }

    /// <summary>
    /// Deletes a conversation and all its messages.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteConversation(Guid id, CancellationToken cancellationToken)
    {
        await _chatHistoryService.DeleteConversationAsync(id, cancellationToken);
        _logger.LogInformation("Deleted conversation {ConversationId}", id);
        return NoContent();
    }
}

public class CreateConversationRequest
{
    public string? Title { get; set; }
}

public class UpdateConversationRequest
{
    public string Title { get; set; } = string.Empty;
}
