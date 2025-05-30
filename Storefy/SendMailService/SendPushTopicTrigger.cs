using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SendMailService;

public class SendPushTopicTrigger
{
    [FunctionName("SendPushTopicTrigger")]
    public static void Run(
    [ServiceBusTrigger("completedorders", "push-notification", Connection = "connection")]
    string mySbMsg,
    ILogger log)
    {
        log.LogInformation($"{mySbMsg}\n");
    }
}
