using EnterpriseAI.Core.Interfaces;
using EnterpriseAI.Infrastructure.AI;
using EnterpriseAI.Infrastructure.ChatHistory;
using EnterpriseAI.Infrastructure.DocumentProcessing;
using EnterpriseAI.Infrastructure.DocumentProcessing.Extractors;
using EnterpriseAI.Infrastructure.VectorStore;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace EnterpriseAI.API.Extensions;

/// <summary>
/// Extension methods for configuring services in the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services including AI, document processing, and storage services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Ollama settings
        services.Configure<OllamaConfig>(configuration.GetSection("Ollama"));

        // Register AI services
        services.AddSingleton<IAIService, OllamaAIService>();
        services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();

        // Register document processing services
        services.AddSingleton<IDocumentProcessor, DocumentProcessor>();
        services.AddSingleton<ITextChunker, TextChunker>();

        // Register text extractors
        services.AddSingleton<ITextExtractor, PdfTextExtractor>();
        services.AddSingleton<ITextExtractor, DocxTextExtractor>();
        services.AddSingleton<ITextExtractor, TxtTextExtractor>();

        // Register vector store (singleton for in-memory persistence across requests)
        services.AddSingleton<IVectorStore, InMemoryVectorStore>();

        // Register chat history service (singleton for in-memory persistence)
        services.AddSingleton<IChatHistoryService, InMemoryChatHistoryService>();

        // Add FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();

        // Add HttpClient for Ollama communication
        services.AddHttpClient("Ollama", client =>
        {
            var ollamaBaseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
            client.BaseAddress = new Uri(ollamaBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("Ollama:RequestTimeout", 300));
        });

        return services;
    }
}
