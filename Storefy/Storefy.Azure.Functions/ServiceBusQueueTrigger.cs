using GameStore.BusinessObjects.Models.Notification;
using GameStore.Interfaces.Services.Notifications;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Gamestore.Azure.Functions;

public class ServiceBusQueueTrigger
{
    private readonly IMessagePublisher _messagePublisher;

    public ServiceBusQueueTrigger(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    [FunctionName("ServiceBusQueueTrigger")]
    public void Run([ServiceBusTrigger("completedorders", Connection = "common_services_messagebus")] string messageBody)
    {
        var emailMessage = JsonConvert.DeserializeObject<EmailServiceBusMessage>(messageBody);

        _messagePublisher.SendMessage(emailMessage.Recipient, emailMessage.Sender, emailMessage.MessageBody, emailMessage.Subject);
    }
}
