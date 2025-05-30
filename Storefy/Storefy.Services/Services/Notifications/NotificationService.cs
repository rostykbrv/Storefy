using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.BusinessObjects.Models.Notification;
using Storefy.Interfaces;
using Storefy.Interfaces.Services.Notifications;

namespace Storefy.Services.Services.Notifications;

/// <summary>
/// Implementation of the services needed to manipulate and retrieve Notifications.
/// </summary>
/// <inheritdoc cref="INotificationService"/>
public class NotificationService : INotificationService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _messagePublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationService"/> class.
    /// </summary>
    /// <param name="contextAccessor">Provides access to the HTTP context.</param>
    /// <param name="unitOfWork">Represents a unit of work.</param>
    /// <param name="messagePublisher">Service for publishing messages to a message topic.</param>
    public NotificationService(
        IHttpContextAccessor contextAccessor,
        IUnitOfWork unitOfWork,
        IMessagePublisher messagePublisher)
    {
        _contextAccessor = contextAccessor;
        _unitOfWork = unitOfWork;
        _messagePublisher = messagePublisher;
    }

    /// <inheritdoc />
    public async Task GenerateNotificationAsync(Order order)
    {
        var userId = _contextAccessor
            .HttpContext
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var userNotification = await _unitOfWork
            .UserRepository
            .GetUserNotification(userId);

        string gameNames = string
            .Join(", ", order.OrderDetails
            .Select(x => x.Game.Name));
        string orderQuantity = string
            .Join(", ", order.OrderDetails
            .Select(x => x.Quantity));

        switch (userNotification.Type.ToLower())
        {
            case "email":
                string gameImages = string
                    .Join(string.Empty, order.OrderDetails
                    .Select(x => $"<img src=\"{x.Game.ImageUrl}\" width=\"150px\" height=\"150px\"/>"));

                await _messagePublisher.Publish(new EmailNotification
                {
                    MessageBody = @$"<h1>New order!</h1>
                    <h2>Order ID: {order.Id}</h2>
                    {gameImages}
                    <h2>{gameNames}</h2>
                    <h3>Quantity: {orderQuantity}</h3>
                    <h2>Sum: {order.Sum}</h2>",
                });
                break;

            case "sms":
                await _messagePublisher.Publish(new SmsNotification
                {
                    Phone = "+1-800-0321-1234",
                    SmsBody = $"SMS notification, Order Id: {order.Id}" +
                    $"Game: {gameNames} " +
                    $"Quantity: {orderQuantity}",
                });
                break;

            case "push":
                await _messagePublisher.Publish(new PushNotification
                {
                    Body = $"PUSH notification, Order Id: {order.Id}" +
                    $"Game: {gameNames} " +
                    $"Quantity: {orderQuantity}",
                });
                break;

            default:
                throw new InvalidDataException("Invalid notification method");
        }
    }
}
