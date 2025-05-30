using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SendMailService;

public class SendSmsTopicTrigger
{
    [FunctionName("SendSmsTopicTrigger")]
    public static void Run(
    [ServiceBusTrigger("completedorders", "sms-notification", Connection = "connection")]
    string mySbMsg,
    ILogger log)
    {
        log.LogInformation($"{mySbMsg}\n");
    }
}
