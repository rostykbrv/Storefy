using Storefy.BusinessObjects.Dto;

namespace Storefy.Interfaces.Repositories.Gamestore;

/// <summary>
/// Interface for the Role Repository. Defines the repository operations for the Role objects.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRoleRepository<T>
    where T : class
{
    /// <summary>
    /// Retrieves all roles in the system.
    /// </summary>
    /// <returns>A list of all roles.</returns>
    Task<IEnumerable<T>> GetAll();

    /// <summary>
    /// Retrieves a role by its name.
    /// </summary>
    /// <param name="roleName">Name of the desired role.</param>
    /// <returns>A role that matches the name provided.</returns>
    Task<T> GetRoleByName(string roleName);

    /// <summary>
    /// Retrieves roles associated with a user.
    /// </summary>
    /// <param name="id">The User's id.</param>
    /// <returns>A list of roles associated with the user.</returns>
    Task<IEnumerable<T>> GetRolesByUser(string id);

    /// <summary>
    /// Retrieves a role by its id.
    /// </summary>
    /// <param name="id">Identifier for the desired role.</param>
    /// <returns>A role that matches the id provided.</returns>
    Task<T> GetRoleById(string id);

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="roleDto">A data transfer object
    /// containing role data.</param>
    /// <returns>The created role.</returns>
    Task<T> CreateNewRole(CreateUpdateRoleDto roleDto);

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    /// <param name="roleDto">A data transfer object
    /// containing updated role data.</param>
    /// <returns>The updated role.</returns>
    Task<T> UpdateRole(CreateUpdateRoleDto roleDto);

    /// <summary>
    /// Deletes a role by its id.
    /// </summary>
    /// <param name="id">The id of the role to be deleted.</param>
    /// <returns>The deleted role.</returns>
    Task<T> DeleteRole(string id);

    /// <summary>
    /// Retrieves permissions associated with a role.
    /// </summary>
    /// <param name="id">The id of the role.</param>
    /// <returns>A list of permissions associated with the role.</returns>
    Task<IEnumerable<string>> GetRolePermissions(string id);

    /// <summary>
    /// Retrieves all permissions in the system.
    /// </summary>
    /// <returns>A list of all permissions.</returns>
    Task<IEnumerable<string>> GetPermissions();
}
