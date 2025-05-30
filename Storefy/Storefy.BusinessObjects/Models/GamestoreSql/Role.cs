using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the Role model class.
/// </summary>
public class Role
{
    /// <summary>
    /// Gets or sets the unique identification number of the Role.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of Role.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the list of Users associated with the Role.
    /// </summary>
    [JsonIgnore]
    public List<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Gets or sets the list of RolePermission associations to this Role.
    /// This enables navigation to all RolePermission
    /// objects this Role is associated with.
    /// </summary>
    [JsonIgnore]
    public List<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();
}
