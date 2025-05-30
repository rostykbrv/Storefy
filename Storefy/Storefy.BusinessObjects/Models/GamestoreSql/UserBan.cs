namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the User ban model class.
/// </summary>
public class UserBan
{
    /// <summary>
    /// Gets or sets the name or id of the User who is banned.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets or sets the duration of the ban.
    /// </summary>
    public string Duration { get; set; }
}
