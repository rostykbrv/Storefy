using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Storefy.API.Handlers;

namespace Storefy.Tests.Handler;
public class GlobalExceptionHandlerTests
{
    [Fact]
    public void OnException_HandlesException_CatchErrorStatusCode500()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<GlobalExceptionHandler>>();
        var exceptionHandler = new GlobalExceptionHandler(mockLogger.Object);
        var exception = new Exception("Test exception");
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ControllerActionDescriptor(),
        };
        var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception,
        };

        // Act
        exceptionHandler.OnException(exceptionContext);

        // Assert
        Assert.IsType<StatusCodeResult>(exceptionContext.Result);
        Assert.Equal(500, ((StatusCodeResult)exceptionContext.Result).StatusCode);
    }
}
