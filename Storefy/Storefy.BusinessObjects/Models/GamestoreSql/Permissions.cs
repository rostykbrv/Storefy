namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the Permisions model class.
/// </summary>
public class Permissions
{
    /// <summary>
    /// Gets or sets the unique identification of the Permission.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the Permission's name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the list of RolePermission associations to this Permission.
    /// This enables navigation to all RolePermission objects this Permission is associated with.
    /// </summary>
    public List<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();
}
