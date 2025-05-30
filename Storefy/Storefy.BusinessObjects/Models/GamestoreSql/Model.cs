namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the login model class.
/// </summary>
public class Model
{
    /// <summary>
    /// Gets or sets the user's login.
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating
    /// whether gets or sets the type of authentication.
    /// </summary>
    public bool? InternalAuth { get; set; }
}
