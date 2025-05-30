namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the RolePermissions model class.
/// </summary>
public class RolePermissions
{
    /// <summary>
    /// Gets or sets the Role's unique identifier.
    /// The identifier for the Role that
    /// this RolePermissions is associated with.
    /// </summary>
    public string RoleId { get; set; }

    /// <summary>
    /// Gets or sets the Role.
    /// This is a navigation property to access
    /// associated Role from RolePermissions object.
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// Gets or sets the Permission's unique identifier.
    /// The identifier for the Permission that
    /// this RolePermissions is associated with.
    /// </summary>
    public string PermissionId { get; set; }

    /// <summary>
    /// Gets or sets the Permissions.
    /// This is a navigation property to access
    /// associated Permissions from RolePermissions object.
    /// </summary>
    public Permissions Permissions { get; set; }
}
