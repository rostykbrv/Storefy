using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Services.Services;

namespace Storefy.Tests.Services.Services;
public class RoleServiceTests
{
    private readonly RoleService _roleService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public RoleServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _roleService = new RoleService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateNewRole_AddsNewRoleSuccessfully()
    {
        // Arrange
        var roleDto = new CreateUpdateRoleDto();
        var role = new Role();

        _unitOfWorkMock.Setup(uow => uow.RoleRepository.CreateNewRole(roleDto)).ReturnsAsync(role);

        // Act
        var result = await _roleService.CreateNewRole(roleDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(role, result);
    }

    [Fact]
    public async Task DeleteRole_DeletesRoleSuccessfully()
    {
        // Arrange
        var roleId = "testRoleId";
        var role = new Role();

        _unitOfWorkMock.Setup(uow => uow.RoleRepository.DeleteRole(roleId)).ReturnsAsync(role);

        // Act
        var result = await _roleService.DeleteRole(roleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(role, result);
    }

    [Fact]
    public async Task GetAllRoles_ReturnsAllRoles()
    {
        // Arrange
        var roles = new List<Role>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test1",
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test2",
            },
        };

        _unitOfWorkMock.Setup(uow => uow.RoleRepository.GetAll()).ReturnsAsync(roles);

        // Act
        var result = await _roleService.GetAllRoles();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(roles, result);
    }

    [Fact]
    public async Task GetPermissions_ReturnsAllPermissions()
    {
        // Arrange
        var permissions = new List<string>
        {
            "AddGame",
            "Game",
        };

        _unitOfWorkMock.Setup(uow => uow.RoleRepository.GetPermissions()).ReturnsAsync(permissions);

        // Act
        var result = await _roleService.GetPermissions();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(permissions, result);
    }

    [Fact]
    public async Task GetRoleById_ReturnsRoleForGivenId()
    {
        // Arrange
        var roleId = "testRoleId";
        var role = new Role()
        {
            Id = roleId,
            Name = "Test",
        };

        _unitOfWorkMock.Setup(uow => uow.RoleRepository.GetRoleById(roleId)).ReturnsAsync(role);

        // Act
        var result = await _roleService.GetRoleById(roleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(role, result);
    }

    [Fact]
    public async Task GetRolePermissions_ReturnsPermissionsForGivenRoleId()
    {
        // Arrange
        var roleId = "testRoleId";
        var permissions = new List<string>
        {
            "AddGame",
            "Game",
        };

        _unitOfWorkMock
            .Setup(repo => repo.RoleRepository.GetRolePermissions(roleId))
            .ReturnsAsync(permissions);

        // Act
        var result = await _roleService.GetRolePermissions(roleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(permissions, result);
    }

    [Fact]
    public async Task GetRolesByUser_ReturnsRolesForGivenUserId()
    {
        // Arrange
        var userId = "testUserId";
        var roles = new List<Role>
        {
            new()
            {
                Id = "1",
                Name = "Test1",
            },
            new()
            {
                Id = "2",
                Name = "Test2",
            },
        };

        _unitOfWorkMock
            .Setup(repo => repo.RoleRepository.GetRolesByUser(userId))
            .ReturnsAsync(roles);

        // Act
        var result = await _roleService.GetRolesByUser(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(roles, result);
    }

    [Fact]
    public async Task UpdateRole_UpdatesRoleSuccessfully()
    {
        // Arrange
        var roleDto = new CreateUpdateRoleDto
        {
            Role = new RoleDto
            {
                Id = "1",
                Name = "TestRole",
            },
            Permissions = new[]
            {
                "Game",
                "AddGame",
            },
        };
        var role = new Role
        {
            Id = "1",
            Name = "TestRole",
        };

        _unitOfWorkMock
            .Setup(repo => repo.RoleRepository.UpdateRole(roleDto))
            .ReturnsAsync(role);

        // Act
        var result = await _roleService.UpdateRole(roleDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(role, result);
    }
}
