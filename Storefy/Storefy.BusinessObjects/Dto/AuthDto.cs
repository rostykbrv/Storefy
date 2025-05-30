namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents a data transfer object of authentication.
/// </summary>
public class AuthDto
{
    /// <summary>
    /// Gets or sets the user's email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>
    public string Password { get; set; }
}
