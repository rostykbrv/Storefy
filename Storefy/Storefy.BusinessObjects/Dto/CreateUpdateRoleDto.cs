namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents the Data Transfer Object for creating or updating a Role.
/// Contains properties necessary for creating
/// a new role record or updating an existing one.
/// </summary>
public class CreateUpdateRoleDto
{
    /// <summary>
    /// Gets or sets the RoleDto object holding the details of the Role.
    /// </summary>
    public RoleDto Role { get; set; }

    /// <summary>
    /// Gets or sets the permissions names.
    /// </summary>
    public string[] Permissions { get; set; }
}
