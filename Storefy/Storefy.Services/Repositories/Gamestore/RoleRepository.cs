using System.Data;
using Microsoft.EntityFrameworkCore;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Repositories.Gamestore;
using Storefy.Services.Data;

namespace Storefy.Services.Repositories.Gamestore;

/// <summary>
/// Repository to manage the operations of Role entities using a database context.
/// </summary>
/// <inheritdoc cref="IRoleRepository{T}"/>
public class RoleRepository : IRoleRepository<Role>
{
    private readonly StorefyDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The data base context
    /// to be used by the repository.</param>
    public RoleRepository(StorefyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Role> CreateNewRole(CreateUpdateRoleDto roleDto)
    {
        var newRole = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = roleDto.Role.Name,
            RolePermissions = new List<RolePermissions>(),
        };

        foreach (var permission in roleDto.Permissions)
        {
            var permissions = await _dbContext.Permissions
                    .Where(p => p.Name == permission)
                    .FirstOrDefaultAsync();

            var rolePermission = new RolePermissions
            {
                RoleId = roleDto.Role.Id,
                PermissionId = permissions.Id,
            };

            newRole.RolePermissions.Add(rolePermission);
        }

        await _dbContext.AddAsync(newRole);
        await _dbContext.SaveChangesAsync();

        return newRole;
    }

    /// <inheritdoc />
    public async Task<Role> DeleteRole(string id)
    {
        var deletedRole = await _dbContext.Roles
            .FindAsync(id)
            ?? throw new InvalidOperationException("Invalid role id.");
        _dbContext.Roles.Remove(deletedRole);
        await _dbContext.SaveChangesAsync();

        return deletedRole;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetAll()
    {
        var roles = await _dbContext.Roles
            .ToListAsync();

        return roles;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetPermissions()
    {
        var permissions = await _dbContext.Permissions
            .Select(p => p.Name)
            .ToListAsync();

        return permissions;
    }

    /// <inheritdoc />
    public async Task<Role> GetRoleById(string id)
    {
        var selectedRole = await _dbContext.Roles
            .FindAsync(id);

        return selectedRole;
    }

    /// <inheritdoc />
    public async Task<Role> GetRoleByName(string roleName)
    {
        var selectedRole = await _dbContext.Roles
            .Where(r => r.Name == roleName)
            .FirstOrDefaultAsync();

        return selectedRole;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetRolePermissions(string id)
    {
        var rolePermissions = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == id)
            .Select(r => r.Permissions.Name)
            .ToListAsync();

        return rolePermissions;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetRolesByUser(string id)
    {
        var rolesByUser = await _dbContext.Roles
            .Include(r => r.Users)
            .Where(r => r.Users.Any(u => u.Id == id))
            .ToListAsync();

        return rolesByUser;
    }

    /// <inheritdoc />
    public async Task<Role> UpdateRole(CreateUpdateRoleDto roleDto)
    {
        var updatedRole = await _dbContext.Roles
            .Include(r => r.RolePermissions)
            .SingleOrDefaultAsync(r => r.Id == roleDto.Role.Id);

        if (updatedRole != null)
        {
            updatedRole.Name = roleDto.Role.Name;
            updatedRole.RolePermissions = new List<RolePermissions>();

            foreach (var permission in roleDto.Permissions)
            {
                var permissions = await _dbContext.Permissions
                    .Where(p => p.Name == permission)
                    .FirstOrDefaultAsync();

                var rolePermission = new RolePermissions
                {
                    RoleId = updatedRole.Id,
                    PermissionId = permissions.Id,
                };

                updatedRole.RolePermissions.Add(rolePermission);
            }

            await _dbContext.SaveChangesAsync();
            return updatedRole;
        }

        throw new InvalidOperationException();
    }
}
