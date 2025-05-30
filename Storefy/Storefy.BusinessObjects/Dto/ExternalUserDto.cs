namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents the Data Transfer Object for external User.
/// This class is used for data transfer operations
/// related to a external User.
/// </summary>
public class ExternalUserDto
{
    /// <summary>
    /// Gets or sets the external user's email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the external user's First Name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the external user's Last Name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the external user's password.
    /// </summary>
    public string Password { get; set; }
}
