namespace Storefy.API.Middlewares;

/// <summary>
/// Middleware for logging the IP address of incoming requests.
/// </summary>
public class LoggingIpAddressMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingIpAddressMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingIpAddressMiddleware"/> class.
    /// </summary>
    /// <param name="next">The delegate to the next middleware in the pipeline.</param>
    /// <param name="logger">The logger to use.</param>
    public LoggingIpAddressMiddleware(RequestDelegate next, ILogger<LoggingIpAddressMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// The middleware pipeline Invoke method. Logs the IP address of the incoming request.
    /// </summary>
    /// <param name="context">The HttpContext for the current request.</param>
    /// <returns>An asynchronous Task representing the completion of request handling.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress.ToString();
        _logger.LogInformation($"IP address of request - {ipAddress}");
        await _next(context);
    }
}
