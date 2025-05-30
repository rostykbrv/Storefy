using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Storefy.API.Middlewares;

namespace Storefy.Tests.Middlewares;
public class ElapsedTimeLoggingMiddlewareTests
{
    private readonly Mock<ILogger<ElapsedTimeLoggingMiddleware>> _loggerMock;
    private readonly Mock<HttpContext> _contextMock;
    private ElapsedTimeLoggingMiddleware _middleware;

    public ElapsedTimeLoggingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ElapsedTimeLoggingMiddleware>>();
        _contextMock = new Mock<HttpContext>();
        _middleware = new ElapsedTimeLoggingMiddleware((innerHttpContext) => Task.CompletedTask, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_CallsNextMiddleware()
    {
        // Arrange
        var nextCalled = false;
        _middleware = new ElapsedTimeLoggingMiddleware(
            (innerHttpContext) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            _loggerMock.Object);

        // Act
        await _middleware.InvokeAsync(_contextMock.Object);

        // Assert
        Assert.True(nextCalled);
    }
}
