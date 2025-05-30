using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Storefy.API.Controllers;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.Tests.Controller;
public class OrderControllerTests
{
    private readonly OrderController _controller;
    private readonly Mock<IOrderService> _orderService;
    private readonly Mock<ILogger<OrderController>> _loggerMock;
    private readonly Mock<IMemoryCache> _cacheMock;

    public OrderControllerTests()
    {
        _cacheMock = new Mock<IMemoryCache>();
        _orderService = new Mock<IOrderService>();
        _loggerMock = new Mock<ILogger<OrderController>>();
        _controller = new OrderController(_orderService.Object, _loggerMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsAllOrders()
    {
        // Arrange
        var orders = new List<Order>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
        },
        new()
        {
            Id = Guid.NewGuid().ToString(),
        },
    };

        var orderHistoryDtos = orders
            .Select(order => new OrderHistoryDto
            {
                Id = order.Id,
            })
            .ToList();

        _orderService.Setup(s => s.GetAllOrders(string.Empty, string.Empty))
            .ReturnsAsync(orderHistoryDtos);

        // Act
        var result = await _controller.GetAllOrders(string.Empty, string.Empty);

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrders = Assert.IsType<List<OrderHistoryDto>>(returnedResult.Value);
        Assert.Equal(orderHistoryDtos.Count, returnedOrders.Count);
    }

    [Fact]
    public async Task GetOrderDetails_OrderExist_ReturnOrderDetails()
    {
        // Arrange
        var userId = "456";
        var orderId = "123";
        var orderDetails = new List<OrderDetails>()
        {
            new()
            {
                Id = "222",
                ProductName = "Test",
                Price = 12,
                CreationDate = DateTime.Now,
                Discount = 0,
                ProductId = Guid.NewGuid().ToString(),
                Quantity = 10,
                Game = new Game(),
            },
        };

        _orderService.Setup(s => s.GetOrderDetails(orderId)).ReturnsAsync(orderDetails);
        var user = new ClaimsPrincipal(new ClaimsIdentity(
         new Claim[]
         {
            new(ClaimTypes.NameIdentifier, userId),
         }));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user },
        };

        var cacheEntry = Mock.Of<ICacheEntry>();
        _cacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);

        // Act
        var result = await _controller.GetOrderDetails(orderId);

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrder = Assert.IsType<List<OrderDetails>>(returnedResult.Value);

        Assert.Equal(orderDetails[0].Id, returnedOrder[0].Id);
    }

    [Fact]
    public async Task GetOrderDetails_OrderNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = "456";
        var orderId = "invalid-order";
        _orderService.Setup(s => s.GetOrderDetails(orderId)).Returns(Task.FromResult<IEnumerable<OrderDetails>>(null));
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        new Claim[]
        {
            new(ClaimTypes.NameIdentifier, userId),
        }));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user },
        };

        var cacheEntry = Mock.Of<ICacheEntry>();
        _cacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);

        // Act
        var result = await _controller.GetOrderDetails(orderId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetOrder_OrderFound_ReturnOrder()
    {
        // Arrange
        var orderId = "123";
        var order = new Order
        {
            Id = "123",
            Status = OrderStatus.Open,
            PaidDate = DateTime.Now,
            OrderDate = DateTime.Now,
            CustomerId = Guid.NewGuid().ToString(),
            Sum = 32,
            OrderDetails = new List<OrderDetails>(),
        };

        _orderService.Setup(s => s.GetOrder(orderId)).ReturnsAsync(order);

        // Act
        var result = await _controller.GetOrder(orderId);

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrder = Assert.IsType<Order>(returnedResult.Value);

        Assert.Equal(order.Id, returnedOrder.Id);
        Assert.Equal(order.Sum, returnedOrder.Sum);
        Assert.Equal(order.PaidDate, returnedOrder.PaidDate);
        Assert.Equal(order.OrderDetails.Count, returnedOrder.OrderDetails.Count);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsOrders()
    {
        // Arrange
        var orders = new List<Order>()
        {
            new()
            {
                Id = "123",
                Status = OrderStatus.Open,
                PaidDate = DateTime.Now,
                OrderDate = DateTime.Now,
                ShipAddress = "test",
                ShipCity = "test",
                ShipCountry = "test",
                ShipName = "test",
                ShippedDate = null,
                CustomerId = Guid.NewGuid().ToString(),
                Sum = 32,
                OrderDetails = new List<OrderDetails>(),
            },
            new()
            {
                Id = "144",
                Status = OrderStatus.Open,
                PaidDate = DateTime.Now,
                OrderDate = DateTime.Now,
                ShipAddress = "test",
                ShipCity = "test",
                ShipCountry = "test",
                ShipName = "test",
                ShippedDate = null,
                CustomerId = Guid.NewGuid().ToString(),
                Sum = 32,
                OrderDetails = new List<OrderDetails>(),
            },
        };

        _orderService.Setup(s => s.GetAll()).ReturnsAsync(orders);

        // Act
        var result = await _controller.GetOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrders = Assert.IsType<List<Order>>(okResult.Value);
        Assert.Equal(2, returnedOrders.Count);
    }

    [Fact]
    public async Task GetOrder_OrderNotFound_ReturnsNotFound()
    {
        // Arrange
        var orderId = "invalid-order";
        _orderService.Setup(s => s.GetOrder(orderId)).Returns(Task.FromResult<Order>(null));

        // Act
        var result = await _controller.GetOrder(orderId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task MakeOrder_ReturnedCreateOrderDto()
    {
        // Arrange
        var order = new Order
        {
            Id = "123",
            Status = OrderStatus.Open,
            PaidDate = DateTime.Now,
            OrderDate = DateTime.Now,
            CustomerId = Guid.NewGuid().ToString(),
            Sum = 32,
            OrderDetails = new List<OrderDetails>(),
        };

        var createdOrder = new CreateOrderDto
        {
            Order = order,
            PaymentMethods = new List<PaymentMethod>(),
        };

        _orderService.Setup(s => s.CreateOrder()).ReturnsAsync(createdOrder);

        // Act
        var result = await _controller.MakeOrderInfo();

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrder = Assert.IsType<CreateOrderDto>(returnedResult.Value);

        Assert.Equal(createdOrder.Order.CustomerId, returnedOrder.Order.CustomerId);
    }

    [Fact]
    public async Task DeleteOrder_ReturnsDeletedOrder()
    {
        var gameKey = "game-test";
        var order = new Order
        {
            Id = "123",
            OrderDate = DateTime.Now,
            Sum = 50,
            OrderDetails = new List<OrderDetails>(),
        };
        var orderDetails = new OrderDetails
        {
            Id = "1234",
            Game = new Game
            {
                Id = "2222",
                Key = gameKey,
            },
        };
        order.OrderDetails.Add(orderDetails);

        _orderService.Setup(s => s.Delete(gameKey)).ReturnsAsync(order);

        // Act
        var result = await _controller.DeleteOrder(gameKey);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrder = Assert.IsType<Order>(okResult.Value);

        Assert.Equal(order.Id, returnedOrder.Id);
        Assert.Equal(order.OrderDetails.Count, returnedOrder.OrderDetails.Count);
    }

    [Fact]
    public async Task UpdateOrderDetailsQuantity_ReturnsUpdatedOrderDetails()
    {
        // Arrange
        var id = "123";
        var count = new UpdateQuantityDto { Count = 5 };
        var orderDetails = new OrderDetails
        {
            Id = id,
            Quantity = count.Count,
            Game = new Game(),
        };
        _orderService.Setup(s => s.UpdateOrderDetailsQuantity(count, id)).ReturnsAsync(orderDetails);

        // Act
        var result = await _controller.UpdateOrderDetailsQuantity(count, id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrderDetails = Assert.IsType<OrderDetails>(okResult.Value);
        Assert.Equal(orderDetails.Id, returnedOrderDetails.Id);
        Assert.Equal(orderDetails.Quantity, returnedOrderDetails.Quantity);
    }

    [Fact]
    public async Task RemoveOrderDetails_ReturnsRemovedOrderDetails()
    {
        // Arrange
        var orderid = Guid.NewGuid().ToString();
        var userId = "456";
        var expectedId = (object)orderid;
        var orderDetails = new OrderDetails
        {
            Id = "222",
            ProductName = "Test",
            Price = 12,
            CreationDate = DateTime.Now,
            Discount = 0,
            ProductId = Guid.NewGuid().ToString(),
            Quantity = 10,
            Game = new Game(),
        };

        _orderService.Setup(s => s.DeleteDetailsFromOrder(orderDetails.Id, orderid)).ReturnsAsync(orderDetails);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
         new Claim[]
         {
            new(ClaimTypes.NameIdentifier, userId),
         }));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user },
        };

        _cacheMock
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedId))
            .Returns(true);

        // Act
        var result = await _controller.RemoveOrderDetails(orderDetails.Id);

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrderDetails = Assert.IsType<OrderDetails>(returnedResult.Value);
        Assert.Equal(orderDetails.Id, returnedOrderDetails.Id);
    }

    [Fact]
    public async Task AddGameToOrderDetails_ReturnsAddedGameToOrderDetails()
    {
        // Arrange
        var orderid = Guid.NewGuid().ToString();
        var userId = "456";
        var gameId = "gameId";
        var expectedOrderId = (object)orderid;
        var orderDetails = new OrderDetails
        {
            Id = "222",
            ProductName = "Test",
            Price = 12,
            CreationDate = DateTime.Now,
            Discount = 0,
            ProductId = Guid.NewGuid().ToString(),
            Quantity = 10,
            Game = new Game(),
        };

        _orderService.Setup(s => s.AddGameToOrderDetails(gameId, orderid)).ReturnsAsync(orderDetails);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
         new Claim[]
         {
        new(ClaimTypes.NameIdentifier, userId),
         }));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user },
        };

        _cacheMock
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedOrderId))
            .Returns(true);

        // Act
        var result = await _controller.AddGameToOrderDetails(gameId);

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrderDetails = Assert.IsType<OrderDetails>(returnedResult.Value);
        Assert.Equal(orderDetails.Id, returnedOrderDetails.Id);
    }

    [Fact]
    public async Task ShipOrder_ReturnsShippedOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var unshippedOrder = new Order
        {
            Id = orderId,
            OrderDetails = new List<OrderDetails>(),
            ShippedDate = null,
        };
        var shippedOrder = new Order
        {
            Id = orderId,
            OrderDetails = new List<OrderDetails>(),
            ShippedDate = DateTime.Today,
        };

        _orderService.Setup(s => s.ShipOrder(unshippedOrder.Id)).ReturnsAsync(shippedOrder);

        // Act
        var result = await _controller.ShipOrder(orderId);

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrder = Assert.IsType<Order>(returnedResult.Value);
        Assert.Equal(shippedOrder.Id, returnedOrder.Id);
        Assert.Equal(DateTime.Today, returnedOrder.ShippedDate);
    }
}
