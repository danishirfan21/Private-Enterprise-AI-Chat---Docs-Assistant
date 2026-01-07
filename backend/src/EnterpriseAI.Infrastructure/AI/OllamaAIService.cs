using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using EnterpriseAI.Core.Exceptions;
using EnterpriseAI.Core.Interfaces;
using EnterpriseAI.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EnterpriseAI.Infrastructure.AI;

/// <summary>
/// Implementation of IAIService using Ollama for chat completions.
/// </summary>
public class OllamaAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaConfig _config;
    private readonly ILogger<OllamaAIService> _logger;

    public OllamaAIService(
        IHttpClientFactory httpClientFactory,
        IOptions<OllamaConfig> config,
        ILogger<OllamaAIService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Ollama");
        _config = config.Value;
        _logger = logger;
    }

    public async IAsyncEnumerable<string> GetStreamingCompletionAsync(
        IEnumerable<ChatMessage> messages,
        string modelName,
        float temperature = 0.7f,
        int maxTokens = 2000,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            model = modelName,
            messages = messages.Select(m => new
            {
                role = m.Role,
                content = m.Content
            }),
            stream = true,
            options = new
            {
                temperature,
                num_predict = maxTokens
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(requestBody)
        };

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to connect to Ollama at {BaseUrl}", _config.BaseUrl);
            throw new OllamaConnectionException($"Cannot connect to Ollama. Ensure it's running at {_config.BaseUrl}", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            response.Dispose();
            throw new OllamaConnectionException($"Ollama request failed: {response.StatusCode} - {errorContent}");
        }

        using (response)
        {
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                using var jsonDoc = JsonDocument.Parse(line);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("message", out var messageElement) &&
                    messageElement.TryGetProperty("content", out var contentElement))
                {
                    var content = contentElement.GetString();
                    if (!string.IsNullOrEmpty(content))
                    {
                        yield return content;
                    }
                }

                if (root.TryGetProperty("done", out var doneElement) && doneElement.GetBoolean())
                {
                    _logger.LogDebug("Streaming completion finished");
                    break;
                }
            }
        }
    }

    public async Task<IEnumerable<ModelInfo>> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new OllamaConnectionException($"Failed to fetch models: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("models", out var modelsElement))
            {
                return Enumerable.Empty<ModelInfo>();
            }

            var models = new List<ModelInfo>();
            foreach (var modelElement in modelsElement.EnumerateArray())
            {
                var model = new ModelInfo();

                if (modelElement.TryGetProperty("name", out var nameElement))
                    model.Name = nameElement.GetString() ?? string.Empty;

                if (modelElement.TryGetProperty("size", out var sizeElement))
                    model.Size = FormatBytes(sizeElement.GetInt64());

                if (modelElement.TryGetProperty("modified_at", out var modifiedElement))
                {
                    if (DateTime.TryParse(modifiedElement.GetString(), out var modified))
                        model.ModifiedAt = modified;
                }

                models.Add(model);
            }

            return models;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to connect to Ollama");
            throw new OllamaConnectionException("Cannot connect to Ollama. Ensure it's running.", ex);
        }
    }

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
