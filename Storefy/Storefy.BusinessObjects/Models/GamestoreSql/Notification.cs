using System.ComponentModel.DataAnnotations;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Model of type notification.
/// </summary>
public class Notification
{
    /// <summary>
    /// Gets or sets the unique identification number of the Notification.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the Notification.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the users list that use concrete notification type.
    /// </summary>
    public virtual ICollection<User> UserNotificationType { get; set; } = new List<User>();
}
