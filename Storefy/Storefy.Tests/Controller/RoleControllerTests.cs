using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Storefy.API.Controllers;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.Tests.Controller;
public class RoleControllerTests
{
    private readonly RoleController _controller;
    private readonly Mock<IRoleService> _roleService;

    public RoleControllerTests()
    {
        _roleService = new Mock<IRoleService>();
        _controller = new RoleController(_roleService.Object);
    }

    [Fact]
    public async Task GetRoles_ReturnsRolesList()
    {
        // Arrange
        var permissionId = Guid.NewGuid().ToString();
        var roleId = Guid.NewGuid().ToString();
        var permission = new Permissions
        {
            Id = Guid.NewGuid().ToString(),
            Name = "AddGame",
            RolePermissions = new List<RolePermissions>(),
        };
        var role = new Role
        {
            Id = roleId,
            Name = "Admin",
            Users = new List<User>(),
            RolePermissions = new List<RolePermissions>(),
        };
        var rolePermissions = new List<RolePermissions>()
        {
            new()
            {
                PermissionId = permissionId,
                Permissions = permission,
                RoleId = roleId,
                Role = role,
            },
        };
        var roles = new List<Role>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test1",
                RolePermissions = rolePermissions,
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test2",
                RolePermissions = rolePermissions,
            },
        };
        _roleService.Setup(s => s.GetAllRoles()).ReturnsAsync(roles);

        // Act
        var result = await _controller.GetRoles();

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRoles = Assert.IsType<List<Role>>(actionResult.Value);
        Assert.Equal(2, returnedRoles.Count);
    }

    [Fact]
    public async Task GetRolesByUserId_ReturnsRolesList()
    {
        // Arrange
        var role1 = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Admin",
            RolePermissions = new List<RolePermissions>(),
            Users = new List<User>(),
        };
        var role2 = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Manager",
            RolePermissions = new List<RolePermissions>(),
            Users = new List<User>(),
        };
        var rolesByUser = new List<Role>()
        {
            role1,
            role2,
        };
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Jake",
            Password = "password",
            Roles = rolesByUser,
        };

        _roleService.Setup(s => s.GetRolesByUser(user.Id))
            .ReturnsAsync(user.Roles);

        // Act
        var result = await _controller.GetRolesByUser(user.Id);

        // Arrange
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRoles = Assert.IsAssignableFrom<IEnumerable<Role>>(actionResult.Value);
        Assert.Equal(2, returnedRoles.Count());
    }

    [Fact]
    public async Task CreateNewRole_ReturnsCreatedRole()
    {
        // Arrange
        var roleId = Guid.NewGuid().ToString();
        var newRoleDto = new CreateUpdateRoleDto
        {
            Role = new RoleDto
            {
                Id = roleId,
                Name = "Admin",
            },
            Permissions = new string[]
            {
                "AddGame",
                "Game",
                "Games",
            },
        };

        var expectedRole = new Role
        {
            Id = roleId,
            Name = "Admin",
            Users = new List<User>(),
            RolePermissions = new List<RolePermissions>(),
        };

        _roleService.Setup(s => s.CreateNewRole(newRoleDto)).ReturnsAsync(expectedRole);

        // Act
        var result = await _controller.CreateNewRole(newRoleDto);

        // Assert
        var createdActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedRole = Assert.IsType<Role>(createdActionResult.Value);
        Assert.Equal(expectedRole.Id, returnedRole.Id);
        Assert.Equal(expectedRole.Name, returnedRole.Name);
    }

    [Fact]
    public async Task DeleteRole_ReturnsDeletedRole()
    {
        // Arrange
        var id = "1";
        var deletedRole = new Role
        {
            Id = id,
            Name = "RoleName",
        };

        _roleService.Setup(s => s.DeleteRole(id)).ReturnsAsync(deletedRole);

        // Act
        var result = await _controller.DeleteRole(id);

        // Assert
        var okActionResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRole = Assert.IsType<Role>(okActionResult.Value);
        Assert.Equal(deletedRole.Id, returnedRole.Id);
        Assert.Equal(deletedRole.Name, returnedRole.Name);
    }

    [Fact]
    public async Task GetPermissions_ReturnsPermissionsList()
    {
        // Arrange
        var permissions = new List<string>()
        {
            "AddGame",
            "ViewGame",
        };

        _roleService.Setup(s => s.GetPermissions()).ReturnsAsync(permissions);

        // Act
        var result = await _controller.GetPermissions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPermissions = Assert.IsType<List<string>>(okResult.Value);
        Assert.Equal(permissions.Count, returnedPermissions.Count);
    }

    [Fact]
    public async Task GetRoleById_ReturnsSelectedRole()
    {
        // Arrange
        var id = "1";
        var selectedRole = new Role
        {
            Id = id,
            Name = "RoleName",
        };

        _roleService.Setup(s => s.GetRoleById(id)).ReturnsAsync(selectedRole);

        // Act
        var result = await _controller.GetRoleById(id);

        // Assert
        var okActionResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRole = Assert.IsType<Role>(okActionResult.Value);
        Assert.Equal(selectedRole.Id, returnedRole.Id);
        Assert.Equal(selectedRole.Name, returnedRole.Name);
    }

    [Fact]
    public async Task GetRolePermissions_ReturnsRolePermissions()
    {
        // Arrange
        var id = "1";
        var rolePermissions = new List<string> { "permission1", "permission2" };
        _roleService.Setup(s => s.GetRolePermissions(id)).ReturnsAsync(rolePermissions);

        // Act
        var result = await _controller.GetRolePermissions(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPermissions = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
        Assert.Equal(rolePermissions.Count, returnedPermissions.Count());
    }

    [Fact]
    public async Task UpdateRole_ReturnsUpdatedRole()
    {
        // Arrange
        var roleDto = new CreateUpdateRoleDto
        {
            Role = new RoleDto
            {
                Id = "1",
                Name = "Admin",
            },
        };

        var updatedRole = new Role
        {
            Id = "1",
            Name = "RoleName",
        };

        _roleService.Setup(s => s.UpdateRole(roleDto)).ReturnsAsync(updatedRole);

        // Act
        var result = await _controller.UpdateRole(roleDto);

        // Assert
        var okActionResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRole = Assert.IsType<Role>(okActionResult.Value);
        Assert.Equal(updatedRole.Id, returnedRole.Id);
        Assert.Equal(updatedRole.Name, returnedRole.Name);
    }
}
