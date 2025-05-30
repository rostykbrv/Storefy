using Microsoft.AspNetCore.Http;
using Storefy.API.Middlewares;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.Tests.Middlewares;

public class GameCountMiddlewareTests
{
    private readonly Mock<IGameService> _gameServiceMock;
    private readonly HttpContext _httpContext;
    private readonly Mock<RequestDelegate> _requestDelegateMock;

    public GameCountMiddlewareTests()
    {
        _gameServiceMock = new Mock<IGameService>();
        _requestDelegateMock = new Mock<RequestDelegate>();

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(x => x.GetService(typeof(IGameService))).Returns(_gameServiceMock.Object);

        _httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProviderMock.Object,
        };
    }

    [Fact]
    public async Task InvokeAsync_AddsGamesCountHeaderToResponse()
    {
        // Arrange
        var gamesList = new List<Game>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Key = "test",
        },
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test2",
            Key = "test2",
        },
    };
        _gameServiceMock.Setup(service => service.GetAll()).ReturnsAsync(gamesList);

        var middleware = new GameCountMiddleware(_requestDelegateMock.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        var gamesCountHeader = _httpContext.Response.Headers["x-total-numbers-of-games"].ToString();
        Assert.Equal(gamesList.Count.ToString(), gamesCountHeader);
    }
}