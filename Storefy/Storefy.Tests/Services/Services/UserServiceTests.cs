using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;
using Storefy.Services.Services;

namespace Storefy.Tests.Services.Services;
public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;

    public UserServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _mockConfiguration = new Mock<IConfiguration>();
        var configurationSection = new Mock<IConfigurationSection>();
        configurationSection.Setup(a => a.Value).Returns("ajvnfjcnzjdhzdkwidjtnskzxcvbnmgk");
        _mockConfiguration.Setup(a => a.GetSection("JwtSettings:Key")).Returns(configurationSection.Object);

        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserService = new Mock<IUserService>();
        _userService = new UserService(httpClient, _mockConfiguration.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public void GetBanDuration_ReturnsExpectedDurations()
    {
        // Arrange
        var expectedBanDurations = new string[]
        {
            "1 hour",
            "1 day",
            "1 week",
            "1 month",
            "permanent",
        };

        // Act
        var resultBanDurations = _userService.GetBanDuration();

        // Assert
        Assert.Equal(expectedBanDurations, resultBanDurations);
    }

    [Fact]
    public async Task BanUser_ReturnsBanHistoryd()
    {
        // Arrange
        var name = "testName";
        var duration = "1 week";

        var expectedBanHistory = new BanHistory
        {
            Id = "1",
            UserId = "15",
            BanStarted = DateTime.UtcNow,
            BanEnds = DateTime.UtcNow.AddDays(7),
        };

        _mockUnitOfWork.Setup(uow => uow.UserRepository.BanUser(name, duration))
            .ReturnsAsync(expectedBanHistory);

        // Act
        var result = await _userService.BanUser(name, duration);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedBanHistory.UserId, result.UserId);
        Assert.Equal(expectedBanHistory.BanEnds, result.BanEnds);
        Assert.Equal(expectedBanHistory.BanStarted, result.BanStarted);
    }

    [Fact]
    public async Task GetUsers_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = "1",
                Name = "TestUser1",
            },
            new()
            {
                Id = "2",
                Name = "TestUser2",
            },
        };

        _mockUnitOfWork
            .Setup(uow => uow.UserRepository.GetAll())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsers();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(users, result);
    }

    [Fact]
    public async Task GetUserById_ReturnsUserForGivenId()
    {
        // Arrange
        var userId = "testUserId";
        var user = new User
        {
            Id = userId,
            Name = "TestUser",
        };

        _mockUnitOfWork
            .Setup(uow => uow.UserRepository.GetById(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserById(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task UpdateUser_UpdatesUserSuccessfully()
    {
        // Arrange
        var userDto = new AddUserDto
        {
            User = new UserDto
            {
                Id = "1",
                Name = "NewUser",
            },
            Password = "password",
            Roles = new string[]
            {
                "Admin",
            },
        };
        var user = new User
        {
            Id = "1",
            Name = "NewUser",
            Password = "password",
            Roles = new List<Role>()
            {
                new()
                {
                    Id = "1",
                    Name = "Admin",
                },
            },
        };

        _mockUnitOfWork
            .Setup(uow => uow.UserRepository.UpdateUser(userDto))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.UpdateUser(userDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task DeleteUser_DeletesUserSuccessfully()
    {
        // Arrange
        var userId = "testUserId";
        var user = new User
        {
            Id = userId,
            Name = "TestUser",
            Password = "Test",
        };

        _mockUnitOfWork
            .Setup(uow => uow.UserRepository.DeleteUser(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.DeleteUser(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task AccessCheckAsync_ChecksAccessSuccessfully()
    {
        // Arrange
        var accessCheck = new AccessCheck
        {
            TargetId = "1",
            TargetPage = "GamePage",
        };

        _mockUnitOfWork
            .Setup(uow => uow.UserRepository.CheckAccess(accessCheck))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.AccessCheckAsync(accessCheck);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AccessCheckAsync_ChecksAccessFalse()
    {
        // Arrange
        var accessCheck = new AccessCheck
        {
            TargetId = "1",
            TargetPage = "GamePage",
        };

        _mockUnitOfWork
            .Setup(uow => uow.UserRepository.CheckAccess(accessCheck))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.AccessCheckAsync(accessCheck);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Login_InternalAuth_Incorrect_ThrowsException()
    {
        // Arrange
        var model = new Model
        {
            Login = "test@gmail.com",
            Password = "password",
            InternalAuth = null,
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.Login(model));
    }

    [Fact]
    public async Task Login_InternalLogin_ReturnsLoginResponce()
    {
        // Arrange
        var model = new Model
        {
            Login = "test@gmail.com",
            Password = "password",
            InternalAuth = true,
        };

        var user = new User
        {
            Id = "1",
            Name = "Jake",
            Password = "password",
            Roles = new List<Role>()
            {
                new()
                {
                    Id = "5",
                    Name = "User",
                },
            },
        };

        var loginResponse = new LoginResponce
        {
            Token = "testToken",
        };

        _mockUnitOfWork.Setup(uow => uow.UserRepository.GetByName(model.Login))
            .ReturnsAsync(user);
        _mockUserService.Setup(s => s.Login(model))
            .ReturnsAsync(loginResponse);

        // Act
        var result = await _userService.Login(model);

        // Assert
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task Login_InternalLogin_IncorrectPassword_ThrownException()
    {
        // Arrange
        var model = new Model
        {
            Login = "test@gmail.com",
            Password = "incorrect",
            InternalAuth = true,
        };

        _mockUnitOfWork.Setup(uow => uow.UserRepository.GetByName(model.Login))
            .Returns(Task.FromResult<User>(null));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.Login(model));
    }

    [Fact]
    public async Task Login_ExternalLogin_NewUser_ReturnsLoginResponce()
    {
        // Arrange
        var model = new Model
        {
            Login = "test@gmail.com",
            Password = "password",
            InternalAuth = false,
        };

        var user = new User
        {
            Id = "1",
            Name = "Jake",
            Password = "password",
            Roles = new List<Role>()
            {
                new()
                {
                    Id = "5",
                    Name = "User",
                },
            },
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(/*lang=json,strict*/"{\"email\":\"test3@test3.com\",\"firstName\":\"Logan\",\"lastName\":\"Paul\"}", Encoding.UTF8, "application/json"),
            });

        _mockUnitOfWork.Setup(uow => uow.UserRepository.GetByName(It.IsAny<string>()))
            .Returns(Task.FromResult<User>(null));

        _mockUnitOfWork.Setup(uow => uow.RoleRepository.GetRoleByName("User"))
            .ReturnsAsync(new Role() { Id = "1", Name = "User" });

        _mockUnitOfWork.Setup(uow => uow.UserRepository.CreateUser(It.IsAny<AddUserDto>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.Login(model);

        // Assert
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task Login_ExternalLogin_UnsuccessfulResponce_ThrowsException()
    {
        // Arrange
        var model = new Model
        {
            Login = "test@gmail.com",
            Password = "password",
            InternalAuth = false,
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Forbidden));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.Login(model));
    }

    [Fact]
    public async Task Login_ExternalLogin_UserExists_ReturnsLoginResponce()
    {
        // Arrange
        var model = new Model
        {
            Login = "test@gmail.com",
            Password = "password",
            InternalAuth = false,
        };

        var user = new User
        {
            Id = "1",
            Name = "Jake",
            Password = "password",
            Roles = new List<Role>()
            {
                new()
                {
                    Id = "5",
                    Name = "User",
                },
            },
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(/*lang=json,strict*/"{\"email\":\"test3@test3.com\",\"firstName\":\"Logan\",\"lastName\":\"Paul\"}", Encoding.UTF8, "application/json"),
            });

        _mockUnitOfWork.Setup(uow => uow.UserRepository.GetByName(It.IsAny<string>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.Login(model);

        // Assert
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task AddUser_ReturnsUser()
    {
        // Arrange
        var addUserDto = new AddUserDto
        {
            User = new UserDto
            {
                Id = "1",
                Name = "Jake",
            },
            Password = "password",
            Roles = new List<string>()
            {
                "5",
            },
        };

        var expectedUser = new User
        {
            Id = "1",
            Name = "Jake",
            Password = "password",
            Roles = new List<Role>()
            {
                new() { Id = "5", Name = "User" },
            },
        };

        _mockUnitOfWork.Setup(uow => uow.UserRepository.CreateUser(addUserDto)).ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.AddUser(addUserDto);

        // Assert
        Assert.Equal(expectedUser, result);
    }

    [Fact]
    public async Task GetUsersNotification_ReturnsAllNotifications()
    {
        // Arrange
        var notifications = new List<Notification>()
        {
            new()
            {
                Id = "1",
                Type = "Email",
                UserNotificationType = new List<User>(),
            },
            new()
            {
                Id = "2",
                Type = "SMS",
                UserNotificationType = new List<User>(),
            },
        };

        _mockUnitOfWork.Setup(x => x.UserRepository.GetAllNotifications()).ReturnsAsync(notifications);

        // Act
        var result = await _userService.GetUsersNotification();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notifications.Count, result.Count());
    }

    [Fact]
    public async Task GetUserNotification_ReturnsCorrectNotification()
    {
        // Arrange
        var id = "1";
        var notification = new Notification
        {
            Id = id,
            Type = "Email",
            UserNotificationType = new List<User>(),
        };

        _mockUnitOfWork.Setup(x => x.UserRepository.GetUserNotification(id)).ReturnsAsync(notification);

        // Act
        var result = await _userService.GetUserNotification(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notification, result);
    }
}
