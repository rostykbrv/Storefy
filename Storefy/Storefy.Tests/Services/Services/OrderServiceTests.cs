using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Services.Services;

namespace Storefy.Tests.Services.Services;
public class OrderServiceTests
{
    private readonly OrderService _orderService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public OrderServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _orderService = new OrderService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task MakeOrder_AddsNewOrder()
    {
        // Arrange
        var orderDto = new CreateOrderDto
        {
            Order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = Guid.NewGuid().ToString(),
                OrderDate = DateTime.Now,
                Status = OrderStatus.Open,
            },
            PaymentMethods = new List<PaymentMethod>
            {
                new()
                {
                    Id = "123",
                    Title = "BankTest",
                    Description = "description",
                    ImageUrl = "image",
                },
            },
        };
        _unitOfWorkMock.Setup(uow => uow.OrderRepository.CreateOrder())
            .ReturnsAsync(orderDto);

        // Act
        var newOrder = await _orderService.CreateOrder();

        // Assert
        Assert.NotNull(newOrder);
        Assert.Equal(orderDto, newOrder);
    }

    [Fact]
    public async Task Delete_DeletesGameFromOrderAndReturnsUpdatedOrder()
    {
        // Arrange
        var gameKey = "game-key-1";
        var order = new Order { Id = Guid.NewGuid().ToString() };

        _unitOfWorkMock
            .Setup(uow => uow.OrderRepository.Delete(gameKey))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.Delete(gameKey);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order, result);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsListOfAllOrders()
    {
        // Arrange
        var orders = new List<OrderHistoryDto>
        {
        new() { Id = Guid.NewGuid().ToString() },
        new() { Id = Guid.NewGuid().ToString() },
        };

        _unitOfWorkMock
            .Setup(uow => uow.OrderRepository.GetOrdersHistory(string.Empty, string.Empty))
            .ReturnsAsync(orders);

        var ordersMongo = new List<OrderHistoryDto>
        {
        new() { Id = Guid.NewGuid().ToString(), CustomerId = Guid.NewGuid().ToString(), OrderDate = "date" },
        new() { Id = Guid.NewGuid().ToString(), CustomerId = Guid.NewGuid().ToString(), OrderDate = "date" },
        };

        // Act
        var result = await _orderService.GetAllOrders(string.Empty, string.Empty);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orders.Count + ordersMongo.Count, result.Count());
    }

    [Fact]
    public async Task GetOrder_ReturnsOrderForGivenId()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var order = new Order { Id = orderId };

        _unitOfWorkMock
            .Setup(uow => uow.OrderRepository.GetOrder(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrder(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order, result);
    }

    [Fact]
    public async Task GetOrderDetails_ReturnsListOfOrderDetailsForGivenOrderId()
    {
        // Arrange
        var orderId = "123";
        var orderDetails = new List<OrderDetails>
        {
            new() { Id = Guid.NewGuid().ToString() },
            new() { Id = Guid.NewGuid().ToString() },
        };

        _unitOfWorkMock
            .Setup(uow => uow.OrderRepository.GetOrderDetails(orderId))
            .ReturnsAsync(orderDetails);

        // Act
        var result = await _orderService.GetOrderDetails(orderId);

        // Assert
        var orderDetailsCount = orderDetails.Count;
        Assert.NotNull(result);
        Assert.Equal(orderDetailsCount, result.Count());
    }

    [Fact]
    public async Task AddGameToOrderDetails_AddsGameSuccessfully()
    {
        // Arrange
        var gameId = "testGameId";
        var orderId = "orderId";

        var orderDetails = new OrderDetails
        {
            Id = Guid.NewGuid().ToString(),
            ProductName = "TestProduct",
        };

        _unitOfWorkMock
            .Setup(uow => uow.OrderRepository.AddGameToOrderDetails(gameId, orderId))
            .ReturnsAsync(orderDetails);

        // Act
        var result = await _orderService.AddGameToOrderDetails(gameId, orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderDetails, result);
    }

    [Fact]
    public async Task DeleteDetailsFromOrder_DeletesDetailsSuccessfully()
    {
        // Arrange
        var detailsId = "testDetailsId";
        var orderId = "testOrderId";

        var orderDetails = new OrderDetails
        {
            Id = Guid.NewGuid().ToString(),
            ProductName = "Test",
        };

        _unitOfWorkMock
            .Setup(uow => uow.OrderRepository.DeleteDetailsFromOrder(detailsId, orderId))
            .ReturnsAsync(orderDetails);

        // Act
        var result = await _orderService.DeleteDetailsFromOrder(detailsId, orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderDetails, result);
    }

    [Fact]
    public async Task GetAll_ReturnsAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = "testOrderId1" },
            new() { Id = "testOrderId2" },
        };

        _unitOfWorkMock
            .Setup(uow => uow.OrderRepository.GetAllOrders())
            .ReturnsAsync(orders);

        // Act
        var result = await _orderService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orders, result);
    }

    [Fact]
    public async Task ShipOrder_CanShipOrder()
    {
        // Arrange
        var orderId = "testOrderId";

        var order = new Order
        {
            Id = orderId,
            Status = OrderStatus.Open,
            OrderDate = DateTime.UtcNow,
        };

        _unitOfWorkMock
            .Setup(repo => repo.OrderRepository.ShipOrder(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.ShipOrder(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order, result);
    }

    [Fact]
    public async Task UpdateOrderDetailsQuantity_CanUpdateOrderDetailsQuantity()
    {
        // Arrange
        var orderId = "testOrderId";
        var count = new UpdateQuantityDto
        {
            Count = 5,
        };

        var orderDetails = new OrderDetails
        {
            Id = orderId,
            ProductName = "Test",
        };

        _unitOfWorkMock
            .Setup(repo => repo.OrderRepository.UpdateOrderDetailsQuantity(count, orderId))
            .ReturnsAsync(orderDetails);

        // Act
        var result = await _orderService.UpdateOrderDetailsQuantity(count, orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderDetails, result);
    }
}
