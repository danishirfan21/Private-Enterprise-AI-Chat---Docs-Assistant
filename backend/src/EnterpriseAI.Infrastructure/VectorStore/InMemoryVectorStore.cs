using System.Collections.Concurrent;
using EnterpriseAI.Core.Interfaces;
using EnterpriseAI.Core.Models;
using Microsoft.Extensions.Logging;

namespace EnterpriseAI.Infrastructure.VectorStore;

/// <summary>
/// In-memory vector store using concurrent collections for thread-safe operations.
/// Implements cosine similarity search for semantic retrieval.
/// </summary>
public class InMemoryVectorStore : IVectorStore
{
    private readonly ConcurrentDictionary<Guid, VectorEmbedding> _embeddings = new();
    private readonly ILogger<InMemoryVectorStore> _logger;

    public InMemoryVectorStore(ILogger<InMemoryVectorStore> logger)
    {
        _logger = logger;
    }

    public Task AddAsync(VectorEmbedding embedding, CancellationToken cancellationToken = default)
    {
        if (embedding == null)
        {
            throw new ArgumentNullException(nameof(embedding));
        }

        if (embedding.Vector.Length == 0)
        {
            throw new ArgumentException("Embedding vector cannot be empty", nameof(embedding));
        }

        _embeddings[embedding.Id] = embedding;
        _logger.LogDebug("Added embedding {EmbeddingId} to vector store", embedding.Id);

        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<VectorEmbedding> embeddings, CancellationToken cancellationToken = default)
    {
        var embeddingList = embeddings?.ToList() ?? throw new ArgumentNullException(nameof(embeddings));

        foreach (var embedding in embeddingList)
        {
            _embeddings[embedding.Id] = embedding;
        }

        _logger.LogInformation("Added {Count} embeddings to vector store", embeddingList.Count);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<SearchResult>> SearchAsync(
        float[] queryVector,
        int topK = 5,
        float similarityThreshold = 0.7f,
        CancellationToken cancellationToken = default)
    {
        if (queryVector == null || queryVector.Length == 0)
        {
            throw new ArgumentException("Query vector cannot be null or empty", nameof(queryVector));
        }

        if (_embeddings.IsEmpty)
        {
            _logger.LogDebug("Vector store is empty, returning no results");
            return Task.FromResult(Enumerable.Empty<SearchResult>());
        }

        var results = new List<SearchResult>();

        foreach (var embedding in _embeddings.Values)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var similarity = VectorSimilarity.CosineSimilarity(queryVector, embedding.Vector);

                if (similarity >= similarityThreshold)
                {
                    results.Add(new SearchResult
                    {
                        Embedding = embedding,
                        SimilarityScore = similarity
                    });
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Failed to calculate similarity for embedding {EmbeddingId}", embedding.Id);
                continue;
            }
        }

        // Sort by similarity score (highest first) and take top K
        var topResults = results
            .OrderByDescending(r => r.SimilarityScore)
            .Take(topK)
            .ToList();

        _logger.LogInformation(
            "Found {ResultCount} results above threshold {Threshold} from {TotalCount} embeddings (returning top {TopK})",
            results.Count, similarityThreshold, _embeddings.Count, topK);

        return Task.FromResult<IEnumerable<SearchResult>>(topResults);
    }

    public Task RemoveByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var toRemove = _embeddings.Values
            .Where(e => e.DocumentId == documentId)
            .Select(e => e.Id)
            .ToList();

        foreach (var id in toRemove)
        {
            _embeddings.TryRemove(id, out _);
        }

        _logger.LogInformation("Removed {Count} embeddings for document {DocumentId}", toRemove.Count, documentId);

        return Task.CompletedTask;
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_embeddings.Count);
    }
}
