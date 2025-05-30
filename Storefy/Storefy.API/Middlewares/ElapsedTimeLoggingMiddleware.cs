using System.Diagnostics;

namespace Storefy.API.Middlewares;

/// <summary>
/// Middleware measures and logs the time taken to handle a request.
/// </summary>
public class ElapsedTimeLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ElapsedTimeLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElapsedTimeLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The delegate to the next middleware in the pipeline.</param>
    /// <param name="logger">The logger to use.</param>
    public ElapsedTimeLoggingMiddleware(RequestDelegate next, ILogger<ElapsedTimeLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// The middleware pipeline Invoke method. It measures and logs the request handling time.
    /// </summary>
    /// <param name="context">The HttpContext for the current request.</param>
    /// <returns>An asynchronous Task representing the completion of request handling.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        await _next(context);
        stopWatch.Stop();
        _logger.LogInformation($"Elapsed request handling time: {stopWatch.Elapsed}");
    }
}
