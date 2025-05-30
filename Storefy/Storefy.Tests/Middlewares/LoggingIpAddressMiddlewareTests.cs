using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Storefy.API.Middlewares;

namespace Storefy.Tests.Middlewares;
public class LoggingIpAddressMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<LoggingIpAddressMiddleware>> _loggerMock;
    private readonly DefaultHttpContext _httpContext;

    public LoggingIpAddressMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<LoggingIpAddressMiddleware>>();
        _httpContext = new DefaultHttpContext
        {
            Connection =
            {
                RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1"),
            },
        };
    }

    [Fact]
    public async Task InvokeAsync_LogsIpAddressAndCallsNextMiddleware()
    {
        // Arrange
        var middleware = new LoggingIpAddressMiddleware(_nextMock.Object, _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }
}
