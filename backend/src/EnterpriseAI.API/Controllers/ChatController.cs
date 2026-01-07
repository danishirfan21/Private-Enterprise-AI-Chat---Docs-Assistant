using System.Text;
using System.Text.Json;
using EnterpriseAI.API.DTOs.Chat;
using EnterpriseAI.Core.Interfaces;
using EnterpriseAI.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorStore;
    private readonly IChatHistoryService _chatHistoryService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IAIService aiService,
        IEmbeddingService embeddingService,
        IVectorStore vectorStore,
        IChatHistoryService chatHistoryService,
        IConfiguration configuration,
        ILogger<ChatController> logger)
    {
        _aiService = aiService;
        _embeddingService = embeddingService;
        _vectorStore = vectorStore;
        _chatHistoryService = chatHistoryService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Streams a chat completion response with optional RAG context.
    /// </summary>
    [HttpPost("stream")]
    [Produces("text/event-stream")]
    public async Task StreamChat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        try
        {
            // Get or create conversation
            Conversation conversation;
            if (request.ConversationId.HasValue)
            {
                conversation = await _chatHistoryService.GetConversationAsync(request.ConversationId.Value, cancellationToken)
                    ?? await _chatHistoryService.CreateConversationAsync(cancellationToken: cancellationToken);
            }
            else
            {
                conversation = await _chatHistoryService.CreateConversationAsync(cancellationToken: cancellationToken);
            }

            // Send conversation ID
            await SendSseEvent("start", new { conversationId = conversation.Id }, cancellationToken);

            // Add user message to history
            var userMessage = new ChatMessage
            {
                Role = "user",
                Content = request.Message
            };
            await _chatHistoryService.AddMessageAsync(conversation.Id, userMessage, cancellationToken);

            // Prepare messages for AI
            var messages = new List<ChatMessage>();

            // Add system prompt if provided
            if (!string.IsNullOrWhiteSpace(request.SystemPrompt))
            {
                messages.Add(new ChatMessage
                {
                    Role = "system",
                    Content = request.SystemPrompt
                });
            }

            // Add RAG context if enabled
            List<SourceAttribution>? sources = null;
            if (request.UseRAG)
            {
                var ragContext = await BuildRagContextAsync(request.Message, cancellationToken);
                if (!string.IsNullOrWhiteSpace(ragContext.Context))
                {
                    var systemMessage = $"Use the following context to answer the user's question. If the answer is not in the context, say so.\n\nContext:\n{ragContext.Context}";
                    messages.Add(new ChatMessage
                    {
                        Role = "system",
                        Content = systemMessage
                    });

                    sources = ragContext.Sources;

                    // Send context sources
                    await SendSseEvent("context", new { sources }, cancellationToken);
                }
            }

            // Add conversation history
            var history = await _chatHistoryService.GetMessagesAsync(conversation.Id, cancellationToken);
            messages.AddRange(history);

            // Stream the AI response
            var responseBuilder = new StringBuilder();

            await foreach (var token in _aiService.GetStreamingCompletionAsync(
                messages,
                request.ModelName,
                request.Temperature,
                request.MaxTokens,
                cancellationToken))
            {
                responseBuilder.Append(token);
                await SendSseEvent("token", new { content = token }, cancellationToken);
            }

            // Save assistant message to history
            var assistantMessage = new ChatMessage
            {
                Role = "assistant",
                Content = responseBuilder.ToString(),
                Sources = sources
            };
            await _chatHistoryService.AddMessageAsync(conversation.Id, assistantMessage, cancellationToken);

            // Send completion
            await SendSseEvent("done", new { messageId = assistantMessage.Id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during chat streaming");
            await SendSseEvent("error", new { error = ex.Message }, cancellationToken);
        }
    }

    private async Task<(string Context, List<SourceAttribution> Sources)> BuildRagContextAsync(
        string query,
        CancellationToken cancellationToken)
    {
        try
        {
            // Generate query embedding
            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query, cancellationToken: cancellationToken);

            // Search for relevant chunks
            var topK = _configuration.GetValue<int>("VectorStore:TopKResults", 5);
            var threshold = _configuration.GetValue<float>("VectorStore:SimilarityThreshold", 0.7f);

            var searchResults = await _vectorStore.SearchAsync(queryEmbedding, topK, threshold, cancellationToken);
            var results = searchResults.ToList();

            if (!results.Any())
            {
                return (string.Empty, new List<SourceAttribution>());
            }

            _logger.LogInformation("Found {Count} relevant chunks for RAG", results.Count);

            // Build context from results
            var contextBuilder = new StringBuilder();
            var sources = new List<SourceAttribution>();
            var seenDocuments = new HashSet<Guid>();

            foreach (var result in results)
            {
                contextBuilder.AppendLine($"[Relevance: {result.SimilarityScore:F2}]");
                contextBuilder.AppendLine(result.Embedding.Text);
                contextBuilder.AppendLine();

                // Track unique source documents
                if (seenDocuments.Add(result.Embedding.DocumentId))
                {
                    var fileName = result.Embedding.Metadata.TryGetValue("document_name", out var name)
                        ? name.ToString() ?? "Unknown"
                        : "Unknown";

                    sources.Add(new SourceAttribution
                    {
                        DocumentId = result.Embedding.DocumentId,
                        FileName = fileName,
                        RelevanceScore = result.SimilarityScore
                    });
                }
            }

            return (contextBuilder.ToString(), sources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building RAG context");
            return (string.Empty, new List<SourceAttribution>());
        }
    }

    private async Task SendSseEvent(string eventType, object data, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(new { type = eventType, data }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }
}
