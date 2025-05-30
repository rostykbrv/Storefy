using System.Text;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Storefy.Interfaces.Services.Notifications;

namespace Storefy.Services.Services.Notifications;

/// <summary>
/// Implementation of the services needed to manipulate Notificactions.
/// </summary>
/// <inheritdoc cref="IMessagePublisher"/>
public class MessagePublisher : IMessagePublisher
{
    private readonly ITopicClient _topicClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePublisher"/> class.
    /// </summary>
    /// <param name="topicClient">Object for interactions with
    /// Bus Service Topic.</param>
    public MessagePublisher(ITopicClient topicClient)
    {
        _topicClient = topicClient;
    }

    /// <inheritdoc />
    public Task Publish<T>(T obj)
    {
        var objText = JsonConvert.SerializeObject(obj);
        var message = new Message(Encoding.UTF8.GetBytes(objText));
        message.UserProperties["messageType"] = typeof(T).Name;

        return _topicClient.SendAsync(message);
    }

    /// <inheritdoc />
    public Task Publish(string raw)
    {
        var message = new Message(Encoding.UTF8.GetBytes(raw));

        return _topicClient.SendAsync(message);
    }
}
