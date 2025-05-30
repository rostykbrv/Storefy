using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using Storefy.BusinessObjects.Models.Notification;

namespace SendMailService;

/// <summary>
/// Azure function trigger to send the notification from azure's topic.
/// </summary>
public class SendMailTopicTrigger
{
    private readonly IConfiguration _configuration;

    public SendMailTopicTrigger(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [FunctionName("SendMailTopicTrigger")]
    public void Run(
    [ServiceBusTrigger("completedorders", "email-notification", Connection = "connection")]
    string mySbMsg,
    ILogger log)
    {
        log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

        var emailNotification = JsonConvert
            .DeserializeObject<EmailNotification>(mySbMsg);
        var message = new MimeMessage();

        message.From.Add(MailboxAddress.Parse(_configuration
            .GetSection("SmtpClient:From").Value));
        message.To.Add(MailboxAddress.Parse(_configuration
            .GetSection("SmtpClient:To").Value));
        message.Subject = "Order purchased";
        message.Body = new TextPart(TextFormat.Html) { Text = emailNotification.MessageBody };

        var smtp = new SmtpClient();
        smtp.Connect(
            _configuration
            .GetSection("SmtpClient:SmtpHost").Value,
            int.Parse(_configuration.GetSection("SmtpClient:SmtpPort").Value),
            SecureSocketOptions.StartTls);
        smtp.Authenticate(
            _configuration
            .GetSection("SmtpClient:Username").Value,
            _configuration.GetSection("SmtpClient:Password").Value);
        smtp.Send(message);
        smtp.Disconnect(true);

        log.LogInformation($"C# ServiceBus topic end Service with message: {mySbMsg}\n");
    }
}
