using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.BusinessObjects.Models.Notification;
using Storefy.Interfaces;
using Storefy.Interfaces.Services.Notifications;
using Storefy.Services.Services.Notifications;

namespace Storefy.Tests.Services.Services;
public class NotificationServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMessagePublisher> _messagePublisherMock;
    private readonly NotificationService _notificationService;

    public NotificationServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _messagePublisherMock = new Mock<IMessagePublisher>();
        _notificationService = new NotificationService(
            _httpContextAccessorMock.Object,
            _unitOfWorkMock.Object,
            _messagePublisherMock.Object);
    }

    [Fact]
    public async Task GenerateNotificationAsync_EmailNotification_PublishesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var userNotification = new Notification { Type = "Email" };
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserNotification(userId)).ReturnsAsync(userNotification);

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>()
            {
                new() { Id = "12", Game = new Game { Name = "Game1" }, Quantity = 1 },
            },
        };

        // Act
        await _notificationService.GenerateNotificationAsync(order);

        // Assert
        _messagePublisherMock.Verify(x => x.Publish(It.IsAny<EmailNotification>()), Times.Once);
    }

    [Fact]
    public async Task GenerateNotificationAsync_SmsNotification_PublishesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var userNotification = new Notification { Type = "Sms" };
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserNotification(userId)).ReturnsAsync(userNotification);

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>()
            {
                new() { Id = "12", Game = new Game { Name = "Game1" }, Quantity = 1 },
            },
        };

        // Act
        await _notificationService.GenerateNotificationAsync(order);

        // Assert
        _messagePublisherMock.Verify(x => x.Publish(It.IsAny<SmsNotification>()), Times.Once);
    }

    [Fact]
    public async Task GenerateNotificationAsync_PushNotification_PublishesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var userNotification = new Notification { Type = "Push" };
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserNotification(userId)).ReturnsAsync(userNotification);

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>()
            {
                new() { Id = "12", Game = new Game { Name = "Game1" }, Quantity = 1 },
            },
        };

        // Act
        await _notificationService.GenerateNotificationAsync(order);

        // Assert
        _messagePublisherMock.Verify(x => x.Publish(It.IsAny<PushNotification>()), Times.Once);
    }

    [Fact]
    public async Task GenerateNotificationAsync_InvalidNotificationType_ThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var userNotification = new Notification { Type = "Invalid" };
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserNotification(userId)).ReturnsAsync(userNotification);

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails>()
            {
                new() { Id = "12", Game = new Game { Name = "Game1" }, Quantity = 1 },
            },
        };

        await Assert.ThrowsAsync<InvalidDataException>(() => _notificationService.GenerateNotificationAsync(order));
    }
}
