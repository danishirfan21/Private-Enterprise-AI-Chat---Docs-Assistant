using System.Net.Http.Json;
using System.Text.Json;
using EnterpriseAI.Core.Exceptions;
using EnterpriseAI.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EnterpriseAI.Infrastructure.AI;

/// <summary>
/// Implementation of IEmbeddingService using Ollama for text embeddings.
/// </summary>
public class OllamaEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaConfig _config;
    private readonly ILogger<OllamaEmbeddingService> _logger;

    public OllamaEmbeddingService(
        IHttpClientFactory httpClientFactory,
        IOptions<OllamaConfig> config,
        ILogger<OllamaEmbeddingService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Ollama");
        _config = config.Value;
        _logger = logger;
    }

    public async Task<float[]> GenerateEmbeddingAsync(
        string text,
        string? modelName = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be empty", nameof(text));
        }

        var model = modelName ?? _config.DefaultEmbeddingModel;

        try
        {
            var requestBody = new
            {
                model,
                prompt = text
            };

            var response = await _httpClient.PostAsJsonAsync("/api/embeddings", requestBody, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new OllamaConnectionException($"Ollama embedding request failed: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("embedding", out var embeddingElement))
            {
                throw new OllamaConnectionException("Embedding response missing 'embedding' property");
            }

            var embedding = new List<float>();
            foreach (var element in embeddingElement.EnumerateArray())
            {
                embedding.Add((float)element.GetDouble());
            }

            _logger.LogDebug("Generated embedding with {Dimensions} dimensions", embedding.Count);
            return embedding.ToArray();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to connect to Ollama for embeddings");
            throw new OllamaConnectionException($"Cannot connect to Ollama at {_config.BaseUrl}", ex);
        }
    }

    public async Task<IEnumerable<float[]>> GenerateEmbeddingsAsync(
        IEnumerable<string> texts,
        string? modelName = null,
        CancellationToken cancellationToken = default)
    {
        var textList = texts.ToList();
        if (!textList.Any())
        {
            return Enumerable.Empty<float[]>();
        }

        _logger.LogInformation("Generating embeddings for {Count} texts", textList.Count);

        var embeddings = new List<float[]>();

        // Process in batches to avoid overwhelming the API
        const int batchSize = 10;
        for (int i = 0; i < textList.Count; i += batchSize)
        {
            var batch = textList.Skip(i).Take(batchSize);
            var batchTasks = batch.Select(text => GenerateEmbeddingAsync(text, modelName, cancellationToken));
            var batchResults = await Task.WhenAll(batchTasks);
            embeddings.AddRange(batchResults);

            _logger.LogDebug("Processed batch {BatchNumber}/{TotalBatches}",
                (i / batchSize) + 1,
                (textList.Count + batchSize - 1) / batchSize);
        }

        return embeddings;
    }
}
