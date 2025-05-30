using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storefy.API.Controllers;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.Tests.Controller;
public class CartControllerTests
{
    private readonly CartController _controller;
    private readonly Mock<ICartService> _cartService;
    private readonly Mock<ILogger<CartController>> _loggerMock;

    public CartControllerTests()
    {
        _cartService = new Mock<ICartService>();
        _loggerMock = new Mock<ILogger<CartController>>();
        _controller = new CartController(_cartService.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task OpenCart_ReturnsOrderDetails()
    {
        // Arrange
        var cartDetails = new List<OrderDetails>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Price = 12,
                Discount = 0,
                CreationDate = DateTime.Now,
                ProductId = "123",
                Quantity = 1,
                ProductName = "Game",
                Game = new Game
                {
                    Id = "123",
                    Name = "Game",
                    Key = "game",
                    UnitInStock = 10,
                    Description = "description",
                    Discontinued = 0,
                    Price = 12,
                },
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Price = 13,
                Discount = 0,
                CreationDate = DateTime.Now,
                ProductId = "555",
                Quantity = 1,
                ProductName = "Super Game",
                Game = new Game
                {
                    Id = "555",
                    Name = "Super Game",
                    Key = "super-game",
                    UnitInStock = 10,
                    Description = "description",
                    Discontinued = 0,
                    Price = 13,
                },
            },
        };

        _cartService.Setup(s => s.GetCartDetails()).ReturnsAsync(cartDetails);

        // Act
        var result = await _controller.GetCartDetails();

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCartDetails = Assert.IsType<List<OrderDetails>>(returnedResult.Value);

        Assert.Equal(cartDetails.Count, returnedCartDetails.Count);
    }
}
