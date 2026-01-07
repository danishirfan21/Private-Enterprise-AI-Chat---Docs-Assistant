using EnterpriseAI.API.Middleware;

namespace EnterpriseAI.API.Extensions;

/// <summary>
/// Extension methods for configuring the application middleware pipeline.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Adds global exception handling middleware to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}
