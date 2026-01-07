using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Core.Interfaces;

/// <summary>
/// Interface for managing chat conversations and message history.
/// </summary>
public interface IChatHistoryService
{
    /// <summary>
    /// Creates a new conversation.
    /// </summary>
    Task<Conversation> CreateConversationAsync(string? title = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a conversation by ID.
    /// </summary>
    Task<Conversation?> GetConversationAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all conversations, ordered by most recent first.
    /// </summary>
    Task<IEnumerable<Conversation>> GetConversationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a message to a conversation.
    /// </summary>
    Task AddMessageAsync(Guid conversationId, ChatMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all messages in a conversation.
    /// </summary>
    Task<IEnumerable<ChatMessage>> GetMessagesAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a conversation and all its messages.
    /// </summary>
    Task DeleteConversationAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the title of a conversation.
    /// </summary>
    Task UpdateConversationTitleAsync(Guid id, string title, CancellationToken cancellationToken = default);
}
