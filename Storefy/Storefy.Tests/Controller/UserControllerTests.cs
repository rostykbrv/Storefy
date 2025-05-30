using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storefy.API.Controllers;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.Tests.Controller;
public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<ILogger<UserController>> _loggerMock;

    public UserControllerTests()
    {
        _userService = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<UserController>>();
        _controller = new UserController(_userService.Object, _loggerMock.Object);
    }

    [Fact]
    public void GetBanDurations_ReturnsBanOptions_WhenOk()
    {
        // Arrange
        var banDurations = new string[] { "1 hour", "1 day", "1 week", "1 month", "1 year" };
        _userService.Setup(service => service.GetBanDuration()).Returns(banDurations);

        // Act
        var result = _controller.GetBanDurations();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedArray = Assert.IsType<string[]>(okResult.Value);

        Assert.NotNull(returnedArray);
        Assert.Equal(banDurations, returnedArray);
    }

    [Fact]
    public void BanUser_ThrowsNotImplemented()
    {
        // Arrange
        var banUser = new UserBan
        {
            User = "test-user",
            Duration = "1 hour",
        };

        // Act and Assert
        Assert.ThrowsAsync<NotImplementedException>(() => _controller.BanUser(banUser));
    }

    [Fact]
    public async Task GetAllUsers_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>()
        {
            new()
            {
                Id = "1",
                Name = "Jake",
                Password = "PasswordTest",
                Roles = new List<Role>(),
            },
            new()
            {
                Id = "2",
                Name = "Paul",
                Password = "PasswordTest",
                Roles = new List<Role>(),
            },
        };

        _userService.Setup(s => s.GetUsers()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        Assert.Equal(users.Count, returnedUsers.Count());
    }

    [Fact]
    public async Task CheckAccess_ReturnsCorrect_True()
    {
        // Arrange
        var accessCheck = new AccessCheck
        {
            TargetId = "1",
            TargetPage = "Page",
        };
        _userService.Setup(s => s.AccessCheckAsync(accessCheck)).ReturnsAsync(true);

        // Act
        var result = await _controller.CheckAccess(accessCheck);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var hasAccess = Assert.IsType<bool>(okResult.Value);
        Assert.True(hasAccess);
    }

    [Fact]
    public async Task CheckAccess_ReturnsInCorrect_False()
    {
        // Arrange
        var accessCheck = new AccessCheck
        {
            TargetId = "1",
            TargetPage = "Page",
        };
        _userService.Setup(s => s.AccessCheckAsync(accessCheck)).ReturnsAsync(false);

        // Act
        var result = await _controller.CheckAccess(accessCheck);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var hasAccess = Assert.IsType<bool>(okResult.Value);
        Assert.False(hasAccess);
    }

    [Fact]
    public async Task Login_UserExists_ReturnsLoginResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Model = new Model
            {
                Login = "Login",
                InternalAuth = true,
                Password = "Password",
            },
        };

        var loginResponse = new LoginResponce
        {
            Token = "token",
        };

        _userService.Setup(s => s.Login(loginRequest.Model)).ReturnsAsync(loginResponse);

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLoginResponse = Assert.IsType<LoginResponce>(okResult.Value);
        Assert.NotNull(returnedLoginResponse);
        Assert.Equal("token", returnedLoginResponse.Token);
    }

    [Fact]
    public async Task Login_UserDoesNotExist_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Model = new Model
            {
                Login = "Login",
                InternalAuth = true,
                Password = "Password",
            },
        };

        _userService.Setup(s => s.Login(loginRequest.Model)).Returns(Task.FromResult<LoginResponce>(null));

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task CreateUser_ReturnsNewUser()
    {
        // Arrange
        var addUserDto = new AddUserDto
        {
            User = new UserDto
            {
                Id = "1",
                Name = "Jake",
            },
            Password = "Password",
            Roles = new List<string>(),
        };
        var newUser = new User
        {
            Id = "1",
            Name = "Jake",
            Password = "Password",
            Roles = new List<Role>(),
        };
        _userService.Setup(s => s.AddUser(addUserDto)).ReturnsAsync(newUser);

        // Act
        var result = await _controller.CreateUser(addUserDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedUser = Assert.IsType<User>(createdAtActionResult.Value);
        Assert.Equal(newUser.Name, returnedUser.Name);
        Assert.Equal(newUser.Id, returnedUser.Id);
    }

    [Fact]
    public async Task UpdateUser_ReturnsUpdatedUser()
    {
        // Arrange
        var addUserDto = new AddUserDto
        {
            User = new UserDto
            {
                Id = "1",
                Name = "Jake",
            },
            Password = "Password",
            Roles = new List<string>(),
        };
        var updatedUser = new User
        {
            Id = "1",
            Name = "Jake",
            Password = "Password",
            Roles = new List<Role>(),
        };
        _userService.Setup(s => s.UpdateUser(addUserDto)).ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser(addUserDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal(updatedUser.Name, returnedUser.Name);
        Assert.Equal(updatedUser.Id, returnedUser.Id);
    }

    [Fact]
    public async Task DeleteUser_ReturnsDeletedUser()
    {
        // Arrange
        var id = "1";
        var deletedUser = new User
        {
            Id = id,
            Name = "Jake",
            Password = "Password",
            Roles = new List<Role>(),
        };
        _userService.Setup(s => s.DeleteUser(id)).ReturnsAsync(deletedUser);

        // Act
        var result = await _controller.DeleteUser(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal(deletedUser.Id, returnedUser.Id);
    }

    [Fact]
    public async Task GetUserById_ReturnsUserById()
    {
        // Arrange
        var id = "1";
        var user = new User
        {
            Id = id,
            Name = "Jake",
            Password = "Password",
            Roles = new List<Role>(),
        };
        _userService.Setup(s => s.GetUserById(id)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal(user.Id, returnedUser.Id);
    }

    [Fact]
    public async Task GetNotifications_ReturnsAllNotifications()
    {
        // Arrange
        var notifications = new List<Notification>()
        {
            new()
            {
                Id = "1",
                Type = "Notification 1",
            },
            new()
            {
                Id = "2",
                Type = "Notification 2",
            },
        };

        _userService.Setup(s => s.GetUsersNotification()).ReturnsAsync(notifications);

        // Act
        var result = await _controller.GetNotifications();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedNotifications = Assert.IsAssignableFrom<IEnumerable<Notification>>(okResult.Value);
        Assert.Equal(notifications.Count, returnedNotifications.Count());
    }

    [Fact]
    public async Task GetUserNotification_ReturnsCorrectNotification()
    {
        // Arrange
        var id = "1";
        var notification = new Notification
        {
            Id = id,
            Type = "Notification",
        };

        _userService.Setup(s => s.GetUserNotification(id)).ReturnsAsync(notification);

        // Act
        var result = await _controller.GetUserNotification(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedNotification = Assert.IsType<Notification>(okResult.Value);
        Assert.Equal(notification, returnedNotification);
    }
}