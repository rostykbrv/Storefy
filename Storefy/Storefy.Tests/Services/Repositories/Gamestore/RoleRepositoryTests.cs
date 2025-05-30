using Microsoft.EntityFrameworkCore;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Services.Data;
using Storefy.Services.Repositories.Gamestore;

namespace Storefy.Tests.Services.Repositories.Gamestore;
public class RoleRepositoryTests : IDisposable
{
    private readonly RoleRepository _roleRepository;
    private readonly StorefyDbContext _dbContext;
    private bool _disposed;

    public RoleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StorefyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StorefyDbContext(options);
        _roleRepository = new RoleRepository(_dbContext);
    }

    [Fact]
    public async Task CreateNewRole_ValidRoleDto_NewRoleCreatedSuccessfully()
    {
        // Arrange
        var permission1 = new Permissions { Id = Guid.NewGuid().ToString(), Name = "permission1" };
        var permission2 = new Permissions { Id = Guid.NewGuid().ToString(), Name = "permission2" };

        _dbContext.Permissions.Add(permission1);
        _dbContext.Permissions.Add(permission2);
        await _dbContext.SaveChangesAsync();

        var roleDto = new CreateUpdateRoleDto
        {
            Role = new RoleDto
            {
                Id = "1",
                Name = "Role1",
            },
            Permissions = new[] { "permission1", "permission2" },
        };

        // Act
        var result = await _roleRepository.CreateNewRole(roleDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(roleDto.Role.Name, result.Name);
        Assert.Equal(2, result.RolePermissions.Count);
    }

    [Fact]
    public async Task DeleteRole_ExistingRole_DeletedSuccessfully()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "role1",
        };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        // Act
        var deletedRole = await _roleRepository.DeleteRole(role.Id);

        // Assert
        Assert.Equal(role.Id, deletedRole.Id);
    }

    [Fact]
    public async Task DeleteRole_NonExistingRole_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistingRoleId = Guid.NewGuid().ToString();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => _roleRepository.DeleteRole(nonExistingRoleId));
    }

    [Fact]
    public async Task GetAll_ReturnsAllRoles()
    {
        // Arrange
        var role1 = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Role1",
        };
        var role2 = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Role2",
        };

        _dbContext.Roles.AddRange(role1, role2);
        await _dbContext.SaveChangesAsync();

        // Act
        var roles = await _roleRepository.GetAll();

        // Assert
        Assert.NotNull(roles);
        Assert.Equal(2, roles.Count());
        Assert.Contains(roles, r => r.Id == role1.Id);
        Assert.Contains(roles, r => r.Id == role2.Id);
    }

    [Fact]
    public async Task GetPermissions_ReturnsAllPermissions()
    {
        // Arrange
        var permission1 = new Permissions { Id = Guid.NewGuid().ToString(), Name = "permission1" };
        var permission2 = new Permissions { Id = Guid.NewGuid().ToString(), Name = "permission2" };

        _dbContext.Permissions.AddRange(permission1, permission2);
        await _dbContext.SaveChangesAsync();

        // Act
        var permissions = await _roleRepository.GetPermissions();

        // Assert
        Assert.NotNull(permissions);
        Assert.Equal(2, permissions.Count());
        Assert.Contains(permissions, p => p == permission1.Name);
        Assert.Contains(permissions, p => p == permission2.Name);
    }

    [Fact]
    public async Task GetRoleById_ExistingRole_ReturnsRole()
    {
        // Arrange
        var role = new Role { Id = Guid.NewGuid().ToString(), Name = "role1" };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        // Act
        var returnedRole = await _roleRepository.GetRoleById(role.Id);

        // Assert
        Assert.NotNull(returnedRole);
        Assert.Equal(role.Id, returnedRole.Id);
    }

    [Fact]
    public async Task GetRoleById_NonExistingRole_ReturnsNull()
    {
        // Arrange
        var nonExistingRoleId = Guid.NewGuid().ToString();

        // Act
        var returnedRole = await _roleRepository.GetRoleById(nonExistingRoleId);

        // Assert
        Assert.Null(returnedRole);
    }

    [Fact]
    public async Task GetRoleByName_ExistingRole_ReturnsRole()
    {
        // Arrange
        var role = new Role { Id = Guid.NewGuid().ToString(), Name = "role1" };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        // Act
        var returnedRole = await _roleRepository.GetRoleByName(role.Name);

        // Assert
        Assert.NotNull(returnedRole);
        Assert.Equal(role.Name, returnedRole.Name);
    }

    [Fact]
    public async Task GetRoleByName_NonExistingRole_ReturnsNull()
    {
        // Arrange
        var nonExistingRoleName = "nonExistingRole";

        // Act
        var returnedRole = await _roleRepository.GetRoleByName(nonExistingRoleName);

        // Assert
        Assert.Null(returnedRole);
    }

    [Fact]
    public async Task GetRolePermissions_ReturnsAllRolePermissions()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "role1",
        };

        var permission1 = new Permissions
        {
            Id = Guid.NewGuid().ToString(),
            Name = "permission1",
        };
        var permission2 = new Permissions
        {
            Id = Guid.NewGuid().ToString(),
            Name = "permission2",
        };

        var rolePermission1 = new RolePermissions { RoleId = role.Id, Permissions = permission1 };
        var rolePermission2 = new RolePermissions { RoleId = role.Id, Permissions = permission2 };

        role.RolePermissions = new List<RolePermissions> { rolePermission1, rolePermission2 };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        // Act
        var rolePermissions = await _roleRepository.GetRolePermissions(role.Id);

        // Assert
        Assert.NotNull(rolePermissions);
        Assert.Equal(2, rolePermissions.Count());
        Assert.Contains("permission1", rolePermissions);
        Assert.Contains("permission2", rolePermissions);
    }

    [Fact]
    public async Task GetRolesByUser_ReturnsAllRolesByUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "User",
            Password = "password",
            NotificationId = "76",
        };

        var role1 = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Role1",
            Users = new List<User> { user },
        };

        var role2 = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Role2",
            Users = new List<User> { user },
        };

        _dbContext.Add(user);
        _dbContext.AddRange(role1, role2);
        await _dbContext.SaveChangesAsync();

        // Act
        var roles = await _roleRepository.GetRolesByUser(user.Id);

        // Assert
        Assert.NotNull(roles);
        Assert.Equal(2, roles.Count());
        Assert.Contains(roles, r => r.Id == role1.Id);
        Assert.Contains(roles, r => r.Id == role2.Id);
    }

    [Fact]
    public async Task UpdateRole_ExistingRole_UpdatesSuccessfully()
    {
        // Arrange
        var permissionName1 = "permission1";
        var permissionName2 = "permission2";
        var permission1 = new Permissions
        {
            Id = Guid.NewGuid().ToString(),
            Name = permissionName1,
        };
        var permission2 = new Permissions
        {
            Id = Guid.NewGuid().ToString(),
            Name = permissionName2,
        };
        _dbContext.Permissions.AddRange(permission1, permission2);

        var role = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "role1",
        };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        var roleDto = new CreateUpdateRoleDto
        {
            Role = new RoleDto
            {
                Id = role.Id,
                Name = "newRoleName",
            },
            Permissions = new[]
            {
                permissionName1,
                permissionName2,
            },
        };

        // Act
        var result = await _roleRepository.UpdateRole(roleDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(roleDto.Role.Name, result.Name);
    }

    [Fact]
    public async Task UpdateRole_NonExistingRole_ThrowsInvalidOperationException()
    {
        // Arrange
        var roleDto = new CreateUpdateRoleDto
        {
            Role = new RoleDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "nonExistingRole",
            },
            Permissions = new[]
            {
                "somePermission",
            },
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => _roleRepository.UpdateRole(roleDto));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }
    }
}
