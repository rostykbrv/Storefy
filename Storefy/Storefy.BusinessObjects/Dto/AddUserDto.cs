namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents a data transfer object for creating an user.
/// </summary>
public class AddUserDto
{
    /// <summary>
    /// Gets or sets the user details to be created.
    /// </summary>
    public UserDto User { get; set; }

    /// <summary>
    /// Gets or sets the ids of roles.
    /// </summary>
    public IEnumerable<string> Roles { get; set; }

    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the notification's id.
    /// </summary>
    public string Notification { get; set; }
}
