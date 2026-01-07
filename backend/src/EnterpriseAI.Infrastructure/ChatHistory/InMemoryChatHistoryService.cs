using System.Collections.Concurrent;
using EnterpriseAI.Core.Interfaces;
using EnterpriseAI.Core.Models;
using Microsoft.Extensions.Logging;

namespace EnterpriseAI.Infrastructure.ChatHistory;

/// <summary>
/// In-memory implementation of chat history service.
/// Stores conversations and messages in concurrent collections.
/// </summary>
public class InMemoryChatHistoryService : IChatHistoryService
{
    private readonly ConcurrentDictionary<Guid, Conversation> _conversations = new();
    private readonly ILogger<InMemoryChatHistoryService> _logger;

    public InMemoryChatHistoryService(ILogger<InMemoryChatHistoryService> logger)
    {
        _logger = logger;
    }

    public Task<Conversation> CreateConversationAsync(string? title = null, CancellationToken cancellationToken = default)
    {
        var conversation = new Conversation
        {
            Title = title ?? "New Conversation",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _conversations[conversation.Id] = conversation;
        _logger.LogInformation("Created conversation {ConversationId}", conversation.Id);

        return Task.FromResult(conversation);
    }

    public Task<Conversation?> GetConversationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _conversations.TryGetValue(id, out var conversation);
        return Task.FromResult(conversation);
    }

    public Task<IEnumerable<Conversation>> GetConversationsAsync(CancellationToken cancellationToken = default)
    {
        var conversations = _conversations.Values
            .OrderByDescending(c => c.UpdatedAt)
            .ToList();

        return Task.FromResult<IEnumerable<Conversation>>(conversations);
    }

    public Task AddMessageAsync(Guid conversationId, ChatMessage message, CancellationToken cancellationToken = default)
    {
        if (!_conversations.TryGetValue(conversationId, out var conversation))
        {
            throw new InvalidOperationException($"Conversation {conversationId} not found");
        }

        message.Timestamp = DateTime.UtcNow;
        conversation.Messages.Add(message);
        conversation.UpdatedAt = DateTime.UtcNow;

        // Auto-generate title from first user message
        if (conversation.Messages.Count == 1 && message.Role == "user")
        {
            conversation.Title = message.Content.Length > 50
                ? message.Content.Substring(0, 47) + "..."
                : message.Content;
        }

        _logger.LogDebug("Added message to conversation {ConversationId}", conversationId);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<ChatMessage>> GetMessagesAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        if (!_conversations.TryGetValue(conversationId, out var conversation))
        {
            return Task.FromResult(Enumerable.Empty<ChatMessage>());
        }

        return Task.FromResult<IEnumerable<ChatMessage>>(conversation.Messages);
    }

    public Task DeleteConversationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _conversations.TryRemove(id, out _);
        _logger.LogInformation("Deleted conversation {ConversationId}", id);

        return Task.CompletedTask;
    }

    public Task UpdateConversationTitleAsync(Guid id, string title, CancellationToken cancellationToken = default)
    {
        if (!_conversations.TryGetValue(id, out var conversation))
        {
            throw new InvalidOperationException($"Conversation {id} not found");
        }

        conversation.Title = title;
        conversation.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Updated conversation {ConversationId} title to '{Title}'", id, title);

        return Task.CompletedTask;
    }
}
