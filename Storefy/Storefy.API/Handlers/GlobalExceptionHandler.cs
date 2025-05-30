using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Storefy.API.Handlers;

/// <summary>
/// Represents the Global Exception handler.
/// </summary>
/// <inheritdoc cref="IExceptionFilter"/>
public class GlobalExceptionHandler : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
    /// </summary>
    /// <param name="logger">The ILogger instance for logging exceptions.</param>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the exceptions thrown in the application.
    /// </summary>
    /// <param name="context">The ExceptionContext containing information about the thrown exception.</param>
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception.Message);
        context.Result = new StatusCodeResult(500);
    }
}
