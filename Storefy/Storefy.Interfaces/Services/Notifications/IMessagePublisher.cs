namespace Storefy.Interfaces.Services.Notifications;

/// <summary>
/// Interface for the Message Service.
/// Defines the service operations related to the notifications.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Method to send message to Azure Topic.
    /// </summary>
    /// <typeparam name="T">Type of send message.</typeparam>
    /// <param name="obj">Name of send object.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task Publish<T>(T obj);

    /// <summary>
    /// Method to send raw string message to Azure Topic.
    /// </summary>
    /// <param name="raw">Raw string message to send.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task Publish(string raw);
}
