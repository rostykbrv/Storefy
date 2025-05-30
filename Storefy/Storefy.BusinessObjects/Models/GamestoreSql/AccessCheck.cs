namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the Access Checking model class.
/// </summary>
public class AccessCheck
{
    /// <summary>
    /// Gets or sets the target page of request.
    /// </summary>
    public string TargetPage { get; set; }

    /// <summary>
    /// Gets or sets the target id of request.
    /// </summary>
    public string? TargetId { get; set; }
}
