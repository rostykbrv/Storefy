using System.Text;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Storefy.BusinessObjects.Models.Notification;
using Storefy.Services.Services.Notifications;

namespace Storefy.Tests.Services.Services;
public class MessagePublisherTests
{
    private readonly MessagePublisher _messagePublisher;
    private readonly Mock<ITopicClient> _topicClient;

    public MessagePublisherTests()
    {
        _topicClient = new Mock<ITopicClient>();
        _messagePublisher = new MessagePublisher(_topicClient.Object);
    }

    [Fact]
    public async Task Publish_EmailNotification_PublishesCorrectly()
    {
        // Arrange
        var emailNotification = new EmailNotification
        {
            MessageBody = "Email notification",
        };
        var notificationJson = JsonConvert.SerializeObject(emailNotification);

        // Act
        await _messagePublisher.Publish(emailNotification);

        // Assert
        _topicClient.Verify(
            x => x
            .SendAsync(It.Is<Message>(m => Encoding.UTF8
            .GetString(m.Body) == notificationJson && m.UserProperties["messageType"]
            .ToString() == emailNotification.GetType().Name)),
            Times.Once);
    }

    [Fact]
    public async Task Publish_SmsNotification_PublishesCorrectly()
    {
        // Arrange
        var smsNotification = new SmsNotification
        {
            Phone = "test phone",
            SmsBody = "Sms notification",
        };
        var notificationJson = JsonConvert.SerializeObject(smsNotification);

        // Act
        await _messagePublisher.Publish(smsNotification);

        // Assert
        _topicClient.Verify(
            x => x
            .SendAsync(It.Is<Message>(m => Encoding.UTF8
            .GetString(m.Body) == notificationJson && m.UserProperties["messageType"]
            .ToString() == smsNotification.GetType().Name)),
            Times.Once);
    }

    [Fact]
    public async Task Publish_PushNotification_PublishesCorrectly()
    {
        // Arrange
        var pushNotification = new PushNotification
        {
            Body = "Push notification",
        };
        var notificationJson = JsonConvert.SerializeObject(pushNotification);

        // Act
        await _messagePublisher.Publish(pushNotification);

        // Assert
        _topicClient.Verify(
            x => x
            .SendAsync(It.Is<Message>(m => Encoding.UTF8
            .GetString(m.Body) == notificationJson && m.UserProperties["messageType"]
            .ToString() == pushNotification.GetType().Name)),
            Times.Once);
    }

    [Fact]
    public async Task Publish_RawString_PublishesCorrectly()
    {
        // Arrange
        var rawString = "Test raw string";

        // Act
        await _messagePublisher.Publish(rawString);

        // Assert
        _topicClient.Verify(
            x =>
            x.SendAsync(It.Is<Message>(m =>
            Encoding.UTF8.GetString(m.Body) == rawString)),
            Times.Once);
    }
}
