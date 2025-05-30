using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Services.Notifications;

/// <summary>
/// Interface for Notification Service.
/// Defines the service operations related to the notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Asynchronously generates a notification for a given order.
    /// </summary>
    /// <param name="order">The order for which to generate a notification.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task GenerateNotificationAsync(Order order);
}
