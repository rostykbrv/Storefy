using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;

namespace Storefy.Services.Services;

/// <summary>
/// Implementation of the services needed to manipulate and
/// retrieve information for Role entities in the repository.
/// </summary>
/// <inheritdoc cref="IRoleService"/>
public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to be used by the Role repository.</param>
    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Role> CreateNewRole(CreateUpdateRoleDto roleDto)
    {
        var newRole = await _unitOfWork
            .RoleRepository
            .CreateNewRole(roleDto);

        return newRole;
    }

    /// <inheritdoc />
    public async Task<Role> DeleteRole(string id)
    {
        var deletedRole = await _unitOfWork
            .RoleRepository
            .DeleteRole(id);

        return deletedRole;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetAllRoles()
    {
        var roles = await _unitOfWork
            .RoleRepository
            .GetAll();

        return roles;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetPermissions()
    {
        var permissions = await _unitOfWork
            .RoleRepository
            .GetPermissions();

        return permissions;
    }

    /// <inheritdoc />
    public async Task<Role> GetRoleById(string id)
    {
        var role = await _unitOfWork
            .RoleRepository
            .GetRoleById(id);

        return role;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetRolePermissions(string id)
    {
        var rolePermissions = await _unitOfWork
            .RoleRepository
            .GetRolePermissions(id);

        return rolePermissions;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetRolesByUser(string id)
    {
        var rolesByUser = await _unitOfWork.RoleRepository
            .GetRolesByUser(id);

        return rolesByUser;
    }

    /// <inheritdoc />
    public async Task<Role> UpdateRole(CreateUpdateRoleDto roleDto)
    {
        var updateRole = await _unitOfWork
            .RoleRepository
            .UpdateRole(roleDto);

        return updateRole;
    }
}
