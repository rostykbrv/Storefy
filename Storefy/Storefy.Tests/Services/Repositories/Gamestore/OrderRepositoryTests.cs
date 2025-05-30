using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Services.Data;
using Storefy.Services.Repositories.Gamestore;

namespace Storefy.Tests.Services.Repositories.Gamestore;
public class OrderRepositoryTests : IDisposable
{
    private readonly OrderRepository _orderRepository;
    private readonly StorefyDbContext _dbContext;
    private bool _disposed;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StorefyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StorefyDbContext(options);
        _orderRepository = new OrderRepository(_dbContext);
    }

    [Fact]
    public async Task GetOrder_ValidOrderId_ReturnOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var order = new Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid().ToString(),
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.GetOrder(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
    }

    [Fact]
    public async Task GetOrder_InvalidOrderId_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidOrderId = Guid.NewGuid().ToString();

        // Act and Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderRepository.GetOrder(invalidOrderId));
        Assert.Equal("This order doesn't exist!", exception.Message);
    }

    [Fact]
    public async Task GetOrdersHistory_DateRangeEmpty_ReturnAllOrders()
    {
        // Arrange
        var paidOrder = new Order
        {
            Id = Guid.NewGuid().ToString(),
            PaidDate = DateTime.Now,
            Status = OrderStatus.Paid,
            CustomerId = Guid.NewGuid().ToString(),
            OrderDate = DateTime.Now.AddDays(-1),
        };

        var openOrder = new Order
        {
            Id = Guid.NewGuid().ToString(),
            PaidDate = null,
            Status = OrderStatus.Open,
            CustomerId = Guid.NewGuid().ToString(),
        };

        _dbContext.Orders.Add(paidOrder);
        _dbContext.Orders.Add(openOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.GetOrdersHistory(string.Empty, string.Empty);

        // Assert
        Assert.Single(result);
        var returnedOrder = result.First();
        Assert.Equal(paidOrder.Id, returnedOrder.Id);
    }

    [Fact]
    public async Task GetOrdersHistory_DateRangeIsNotEmpty_OrdersByDate()
    {
        // Arrange
        var start = "Wed Jan 01 1997 00:00:00 GMT 0200 (за східноєвропейським стандартним часом)";
        var end = "Thu Jan 02 1997 00:00:00 GMT 0200 (за східноєвропейським стандартним часом)";
        var format = "yyyy-MM-ddTHH:mm:ss";
        var date = "1997-01-01T00:00:00";
        var orderedDate = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
        var order = new List<Order>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                PaidDate = null,
                Status = OrderStatus.Open,
                CustomerId = Guid.NewGuid().ToString(),
                ShipAddress = "test",
                ShipCity = "test",
                ShipCountry = "test",
                ShipName = "test",
                ShippedDate = DateTime.Now,
                ShipPostalCode = "test",
                ShipRegion = "test",
                ShipVia = 2,
                Sum = 2,
                EmployeeId = 1,
                Freight = 1,
                OrderDate = orderedDate,
                OrderId = 1,
                RequiredDate = DateTime.Now,
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                PaidDate = null,
                Status = OrderStatus.Open,
                CustomerId = Guid.NewGuid().ToString(),
                ShipAddress = "test",
                ShipCity = "test",
                ShipCountry = "test",
                ShipName = "test",
                ShippedDate = DateTime.Now,
                ShipPostalCode = "test",
                ShipRegion = "test",
                ShipVia = 2,
                Sum = 2,
                EmployeeId = 1,
                Freight = 1,
                OrderDate = orderedDate,
                OrderId = 1,
                RequiredDate = DateTime.Now,
            },
        };

        _dbContext.Orders.AddRange(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.GetOrdersHistory(start, end);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateOrder_ReturnsOrderAndPaymentMethods()
    {
        // Arrange
        var openOrder = new Order
        {
            Id = Guid.NewGuid().ToString(),
            PaidDate = null,
            Status = OrderStatus.Open,
            CustomerId = Guid.NewGuid().ToString(),
        };

        var paymentMethod1 = new PaymentMethod
        {
            Id = Guid.NewGuid().ToString(),
            Title = "TestPayment1",
            Description = "Test description",
            ImageUrl = "image",
        };

        var paymentMethod2 = new PaymentMethod
        {
            Id = Guid.NewGuid().ToString(),
            Title = "TestPayment2",
            Description = "Test description",
            ImageUrl = "image2",
        };

        _dbContext.Orders.Add(openOrder);
        _dbContext.PaymentMethods.Add(paymentMethod1);
        _dbContext.PaymentMethods.Add(paymentMethod2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.CreateOrder();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Order);
        Assert.Equal(openOrder.Id, result.Order.Id);

        Assert.NotNull(result.PaymentMethods);
        Assert.Equal(2, result.PaymentMethods.Count);
    }

    [Fact]
    public async Task Delete_RemovesGameFromOpenOrder()
    {
        // Arrange
        var gameKey = "game-test-1";
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Game Test 1",
            Key = gameKey,
        };

        var orderDetails = new OrderDetails
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = game.Id,
            ProductName = game.Name,
        };

        var openOrder = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            Status = OrderStatus.Open,
            PaidDate = null,
            OrderDetails = new List<OrderDetails> { orderDetails },
        };

        _dbContext.Games.Add(game);
        _dbContext.Orders.Add(openOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.Delete(gameKey);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(openOrder.Id, result.Id);
        Assert.Empty(result.OrderDetails);
    }

    [Fact]
    public async Task GetActiveOrder_ReturnsOpenOrderWithDetails()
    {
        // Arrange
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Key = "game-key-1",
            Name = "Game Test 1",
        };

        var orderDetails = new OrderDetails
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = game.Id,
            Game = game,
            ProductName = game.Name,
        };

        var openOrder = new Order
        {
            Id = Guid.NewGuid().ToString(),
            Status = OrderStatus.Open,
            PaidDate = null,
            OrderDetails = new List<OrderDetails> { orderDetails },
            CustomerId = Guid.NewGuid().ToString(),
        };

        _dbContext.Games.Add(game);
        _dbContext.Orders.Add(openOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.GetActiveOrder();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(openOrder.Id, result.Id);
        Assert.Single(result.OrderDetails);

        var returnedOrderDetails = result.OrderDetails.First();
        Assert.Equal(orderDetails.Id, returnedOrderDetails.Id);
        Assert.Equal(game.Id, returnedOrderDetails.Game.Id);
    }

    [Fact]
    public async Task CompleteOrder_UpdatesOrderStatusAndDecreasesStock()
    {
        // Arrange
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Key = "game-key-1",
            UnitInStock = 5,
            Name = "Game Test 1",
        };

        var orderDetails = new OrderDetails
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = game.Id,
            Game = game,
            ProductName = game.Name,
        };

        var openOrder = new Order
        {
            Id = Guid.NewGuid().ToString(),
            Status = OrderStatus.Open,
            PaidDate = null,
            OrderDetails = new List<OrderDetails> { orderDetails },
            CustomerId = Guid.NewGuid().ToString(),
        };

        _dbContext.Games.Add(game);
        _dbContext.Orders.Add(openOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        await _orderRepository.CompleteOrder();

        // Assert
        var updatedOrder = await _dbContext.Orders.FindAsync(openOrder.Id);

        Assert.NotNull(updatedOrder);
        Assert.Equal(OrderStatus.Paid, updatedOrder.Status);
        Assert.NotNull(updatedOrder.PaidDate);

        var orderedGame = updatedOrder.OrderDetails.First().Game;
        Assert.Equal(4, orderedGame.UnitInStock);
    }

    [Fact]
    public async Task CancelledOrder_UpdatesOrderStatusToCancelled()
    {
        // Arrange
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Key = "game-key-1",
            Name = "Game Test 1",
        };

        var orderDetails = new OrderDetails
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = game.Id,
            Game = game,
            ProductName = game.Name,
        };

        var openOrder = new Order
        {
            Id = Guid.NewGuid().ToString(),
            Status = OrderStatus.Open,
            PaidDate = null,
            OrderDetails = new List<OrderDetails> { orderDetails },
            CustomerId = Guid.NewGuid().ToString(),
        };

        _dbContext.Games.Add(game);
        _dbContext.Orders.Add(openOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        await _orderRepository.CancelledOrder();

        // Assert
        var updatedOrder = await _dbContext.Orders.FindAsync(openOrder.Id);

        Assert.NotNull(updatedOrder);
        Assert.Equal(OrderStatus.Cancelled, updatedOrder.Status);
    }

    [Fact]
    public async Task OrderCheckout_UpdatesOrderStatusToCheckout()
    {
        // Arrange
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Key = "game-key-1",
            Name = "Game Test 1",
        };

        var orderdetails = new OrderDetails
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = game.Id,
            Game = game,
            ProductName = game.Name,
        };

        var openOrder = new Order
        {
            Id = Guid.NewGuid().ToString(),
            Status = OrderStatus.Open,
            PaidDate = null,
            OrderDetails = new List<OrderDetails> { orderdetails },
            CustomerId = Guid.NewGuid().ToString(),
        };

        _dbContext.Games.Add(game);
        _dbContext.Orders.Add(openOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        await _orderRepository.OrderCheckout();

        // Assert
        var updatedOrder = await _dbContext.Orders.FindAsync(openOrder.Id);
        Assert.NotNull(updatedOrder);
        Assert.Equal(OrderStatus.Checkout, updatedOrder.Status);
    }

    [Fact]
    public async Task GetOrderDetails_ReturnsOrderDetailsForGivenOrderId()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();

        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Key = "game-key-1",
            Name = "Game Test 1",
        };

        var orderDetails = new OrderDetails
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = game.Id,
            Game = game,
            ProductName = game.Name,
        };

        var order = new Order
        {
            Id = orderId,
            OrderDetails = new List<OrderDetails> { orderDetails },
            CustomerId = Guid.NewGuid().ToString(),
        };

        _dbContext.Games.Add(game);
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.GetOrderDetails(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        var returnedOrderDetail = result.First();
        Assert.Equal(orderDetails.Id, returnedOrderDetail.Id);
        Assert.Equal(game.Id, returnedOrderDetail.Game.Id);
    }

    [Fact]
    public async Task GetOrders_ReturnsAllOrders()
    {
        // Arrange
        var order1 = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>(),
        };
        var order2 = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>(),
        };

        _dbContext.Orders.Add(order1);
        _dbContext.Orders.Add(order2);
        await _dbContext.SaveChangesAsync();

        // Act
        var orders = await _orderRepository.GetAllOrders();

        // Assert
        Assert.NotNull(orders);
        Assert.Equal(2, orders.Count());
    }

    [Fact]
    public async Task UpdateOrderDetailsQuantity_UpdatesQuantitySuccessfully()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var game = new Game()
        {
            Id = Guid.NewGuid().ToString(),
            Key = "game-key-1",
            Name = "Game Test 1",
        };
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>()
            {
                new()
                {
                    Id = id,
                    ProductName = game.Name,
                    Quantity = 1,
                    Game = game,
                },
            },
        };

        var countDto = new UpdateQuantityDto()
        {
            Count = 5,
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var orderDetails = await _orderRepository.UpdateOrderDetailsQuantity(countDto, id);

        // Assert
        Assert.NotNull(orderDetails);
        Assert.Equal(countDto.Count, orderDetails.Quantity);
    }

    [Fact]
    public async Task AddGameToOrderDetails_ValidGame_OrderDetailsSuccessfullyAdded()
    {
        // Arrange
        var game = new Game
        {
            Id = "game1",
            Name = "Game-1",
            Key = "game-1",
            Price = 10,
        };

        var order = new Order
        {
            Id = "order1",
            CustomerId = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>(),
        };

        _dbContext.Games.Add(game);
        _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.AddGameToOrderDetails(game.Id, order.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(game.Id, result.ProductId);
    }

    [Fact]
    public async Task AddGameToOrderDetails_InvalidGame_ThrowsInvalidDataException()
    {
        // Arrange
        var invalidGameId = "invalidGameId";
        var order = new Order
        {
            Id = "order1",
            CustomerId = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>(),
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act and Assert
        await Assert.ThrowsAsync<InvalidDataException>(()
            => _orderRepository.AddGameToOrderDetails(invalidGameId, order.Id));
    }

    [Fact]
    public async Task AddGameToOrderDetails_InvalidOrder_ThrowsInvalidDataException()
    {
        // Arrange
        var game = new Game
        {
            Id = "game1",
            Name = "Game-1",
            Key = "game-1",
            Price = 10,
        };

        var invalidOrderId = "invalidOrderId";

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        // Act and Assert
        await Assert.ThrowsAsync<InvalidDataException>(()
            => _orderRepository.AddGameToOrderDetails(game.Id, invalidOrderId));
    }

    [Fact]
    public async Task DeleteDetailsFromOrder_ValidOrderAndDetails_OrderDetailsSuccessfullyDeleted()
    {
        // Arrange
        var orderDetail = new OrderDetails
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = Guid.NewGuid().ToString(),
            ProductName = "game",
        };

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>()
            {
                orderDetail,
            },
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.DeleteDetailsFromOrder(orderDetail.Id, order.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderDetail.Id, result.Id);
    }

    [Fact]
    public async Task DeleteDetailsFromOrder_InvalidOrder_ThrowsInvalidDataException()
    {
        // Arrange
        var invalidOrderId = Guid.NewGuid().ToString();

        // Act and Assert
        await Assert.ThrowsAsync<InvalidDataException>(()
            => _orderRepository.DeleteDetailsFromOrder(Guid.NewGuid().ToString(), invalidOrderId));
    }

    [Fact]
    public async Task DeleteDetailsFromOrder_InvalidOrderDetails_ThrowsInvalidDataException()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>(),
            CustomerId = Guid.NewGuid().ToString(),
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act and Assert
        await Assert.ThrowsAsync<InvalidDataException>(()
            => _orderRepository.DeleteDetailsFromOrder(Guid.NewGuid().ToString(), order.Id));
    }

    [Fact]
    public async Task ShipOrder_ValidOrder_OrderSuccessfullyShipped()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>(),
            CustomerId = Guid.NewGuid().ToString(),
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.ShipOrder(order.Id);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ShippedDate);
        Assert.Equal(order.Id, result.Id);
    }

    [Fact]
    public async Task ShipOrder_InvalidOrder_ThrowsInvalidDataException()
    {
        // Arrange
        var invalidOrderId = Guid.NewGuid().ToString();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(()
            => _orderRepository.ShipOrder(invalidOrderId));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }
    }
}
