using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the User model class.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier of the User.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the User.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the User's password.
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a list of Roles associated with the User.
    /// </summary>
    public List<Role> Roles { get; set; } = new List<Role>();

    /// <summary>
    /// Gets or sets the unique identifier of related notification.
    /// </summary>
    public string NotificationId { get; set; }

    /// <summary>
    /// Gets or sets the user's notification type.
    /// </summary>
    [JsonIgnore]
    public Notification NotificationType { get; set; }
}
