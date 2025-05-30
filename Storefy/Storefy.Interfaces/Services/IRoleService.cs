using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Services;

/// <summary>
/// Interface for the Role Service. Defines the service operations related to Roles.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Retrieves all roles from the system.
    /// </summary>
    /// <returns>A task that when resolved, returns
    /// a list of all roles.</returns>
    Task<IEnumerable<Role>> GetAllRoles();

    /// <summary>
    /// Retrieves all roles associated with a specific user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns>A task that when resolved, returns a list
    /// of roles associated with the user.</returns>
    Task<IEnumerable<Role>> GetRolesByUser(string id);

    /// <summary>
    /// Retrieves a specific role by its id.
    /// </summary>
    /// <param name="id">The id of the role to retrieve.</param>
    /// <returns>A task that when resolved,
    /// returns the role with the provided id.</returns>
    Task<Role> GetRoleById(string id);

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="roleDto">The details of the role to create.</param>
    /// <returns>A task that when resolved, returns the created role.</returns>
    Task<Role> CreateNewRole(CreateUpdateRoleDto roleDto);

    /// <summary>
    /// Updates a specific role.
    /// </summary>
    /// <param name="roleDto">The details of the role to update.</param>
    /// <returns>A task that when resolved, returns the updated role.</returns>
    Task<Role> UpdateRole(CreateUpdateRoleDto roleDto);

    /// <summary>
    /// Deletes a specific role by its id.
    /// </summary>
    /// <param name="id">The id of the role to delete.</param>
    /// <returns>A task that when resolved, returns the deleted role.</returns>
    Task<Role> DeleteRole(string id);

    /// <summary>
    /// Retrieves all permissions of a specific role by its id.
    /// </summary>
    /// <param name="id">The id of the role.</param>
    /// <returns>A task that when resolved, returns a list
    /// of permissions associated with the role.</returns>
    Task<IEnumerable<string>> GetRolePermissions(string id);

    /// <summary>
    /// Retrieves all permissions from the system.
    /// </summary>
    /// <returns>A task that when resolved, returns a list
    /// of all permissions in the system.</returns>
    Task<IEnumerable<string>> GetPermissions();
}
