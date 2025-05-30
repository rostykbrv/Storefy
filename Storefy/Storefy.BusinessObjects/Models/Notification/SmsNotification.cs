namespace Storefy.BusinessObjects.Models.Notification;

/// <summary>
/// Sms notification model.
/// </summary>
public class SmsNotification
{
    /// <summary>
    /// Gets or sets the phone number of notification's sender.
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Gets or sets the body of sms notification.
    /// </summary>
    public string SmsBody { get; set; }
}
