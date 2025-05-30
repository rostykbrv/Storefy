using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Services.Data;
using Storefy.Services.Repositories.Gamestore;

namespace Storefy.Tests.Services.Repositories.Gamestore;
public class UserRepositoryTests : IDisposable
{
    private readonly UserRepository _userRepository;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly StorefyDbContext _dbContext;
    private bool _disposed;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StorefyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StorefyDbContext(options);
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _userRepository = new UserRepository(_dbContext, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task CreateUser_NonExistingUser_CreatesNewUserSuccessfully()
    {
        // Arrange
        var roleId = Guid.NewGuid().ToString();
        var role = new Role
        {
            Id = roleId,
            Name = "TestRole",
        };
        var notification = new Notification
        {
            Id = Guid.NewGuid().ToString(),
            Type = "Email",
            UserNotificationType = new List<User>(),
        };

        _dbContext.Notifications.Add(notification);
        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        var userDto = new AddUserDto
        {
            User = new UserDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "TestUser",
            },
            Password = "password",
            Roles = new List<string>()
            {
                role.Id,
            },
            Notification = "76",
        };

        // Act
        var newUser = await _userRepository.CreateUser(userDto);

        // Assert
        Assert.NotNull(newUser);
        Assert.Equal(userDto.User.Name, newUser.Name);
        Assert.Equal(userDto.Password, newUser.Password);
    }

    [Fact]
    public async Task CreateUser_ExistingUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Existing User",
            Password = "password",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var userDto = new AddUserDto
        {
            User = new UserDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Existing User",
            },
            Password = "password",
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
        _userRepository.CreateUser(userDto));
    }

    [Fact]
    public async Task DeleteUser_ExistingUser_DeletedSuccessfully()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "User1",
            Password = "password",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var deletedUser = await _userRepository.DeleteUser(user.Id);

        // Assert
        Assert.Equal(user.Id, deletedUser.Id);
    }

    [Fact]
    public async Task DeleteUser_NonExistingUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistingUserId = Guid.NewGuid().ToString();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => _userRepository.DeleteUser(nonExistingUserId));
    }

    [Fact]
    public async Task GetAllUsers_ReturnsAllUsers()
    {
        // Arrange
        var user1 = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "User1",
            Password = "test",
            NotificationId = "76",
        };
        var user2 = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "User2",
            Password = "test",
            NotificationId = "76",
        };

        _dbContext.Users.AddRange(user1, user2);
        await _dbContext.SaveChangesAsync();

        // Act
        var users = await _userRepository.GetAll();

        // Assert
        Assert.NotNull(users);
        Assert.Equal(2, users.Count());
        Assert.Contains(users, u => u.Id == user1.Id);
        Assert.Contains(users, u => u.Id == user2.Id);
    }

    [Fact]
    public async Task BanUser_ExistingUser_BansSuccessfully()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Existing User",
            Password = "test",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var banHistory = await _userRepository.BanUser(user.Name, "1 day");

        // Assert
        Assert.NotNull(banHistory);
        Assert.Equal(user.Id, banHistory.UserId);
        Assert.Equal(DateTime.UtcNow.AddDays(1).Date, banHistory.BanEnds.Date);
    }

    [Fact]
    public async Task BanUser_NonExistingUser_ThrowsArgumentException()
    {
        // Arrange
        var nonExistingUserName = "NonExistingUser";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(()
            => _userRepository.BanUser(nonExistingUserName, "1 day"));
    }

    [Fact]
    public async Task BanUser_InvalidBanDuration_ThrowsInvalidDataException()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Existing User",
            Password = "test",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(()
            => _userRepository.BanUser(user.Name, "InvalidBanDuration"));
    }

    [Fact]
    public async Task BanUser_ExistingUser_BanForHour_Successfully()
    {
        // Arrange
        var banDuration = "1 hour";
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Existing User",
            Password = "test",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var banHistory = await _userRepository.BanUser(user.Name, banDuration);

        // Assert
        Assert.NotNull(banHistory);
        Assert.Equal(user.Id, banHistory.UserId);
        Assert.Equal(DateTime.UtcNow.AddHours(1).Date, banHistory.BanEnds.Date);
    }

    [Fact]
    public async Task BanUser_ExistingUser_BanForDay_Successfully()
    {
        // Arrange
        var banDuration = "1 day";
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Existing User",
            Password = "test",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var banHistory = await _userRepository.BanUser(user.Name, banDuration);

        // Assert
        Assert.NotNull(banHistory);
        Assert.Equal(user.Id, banHistory.UserId);
        Assert.Equal(DateTime.UtcNow.AddDays(1).Date, banHistory.BanEnds.Date);
    }

    [Fact]
    public async Task BanUser_ExistingUser_BanForWeek_Successfully()
    {
        // Arrange
        var banDuration = "1 week";
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Existing User",
            Password = "test",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var banHistory = await _userRepository.BanUser(user.Name, banDuration);

        // Assert
        Assert.NotNull(banHistory);
        Assert.Equal(user.Id, banHistory.UserId);
        Assert.Equal(DateTime.UtcNow.AddDays(7).Date, banHistory.BanEnds.Date);
    }

    [Fact]
    public async Task BanUser_ExistingUser_BanForMonth_Successfully()
    {
        // Arrange
        var banDuration = "1 month";
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Existing User",
            Password = "test",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var banHistory = await _userRepository.BanUser(user.Name, banDuration);

        // Assert
        Assert.NotNull(banHistory);
        Assert.Equal(user.Id, banHistory.UserId);
        Assert.Equal(DateTime.UtcNow.AddMonths(1).Date, banHistory.BanEnds.Date);
    }

    [Fact]
    public async Task BanUser_ExistingUser_BanPermanent_Successfully()
    {
        // Arrange
        var banDuration = "permanent";
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Existing User",
            Password = "test",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var banHistory = await _userRepository.BanUser(user.Name, banDuration);

        // Assert
        Assert.NotNull(banHistory);
        Assert.Equal(user.Id, banHistory.UserId);
        Assert.Equal(DateTime.MaxValue, banHistory.BanEnds);
    }

    [Fact]
    public async Task GetById_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "User1",
            Password = "test",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var returnedUser = await _userRepository.GetById(user.Id);

        // Assert
        Assert.NotNull(returnedUser);
        Assert.Equal(user.Id, returnedUser.Id);
    }

    [Fact]
    public async Task GetById_NonExistingUser_ReturnsNull()
    {
        // Arrange
        var nonExistingUserId = Guid.NewGuid().ToString();

        // Act
        var returnedUser = await _userRepository.GetById(nonExistingUserId);

        // Assert
        Assert.Null(returnedUser);
    }

    [Fact]
    public async Task GetByName_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ExistingUser",
            Password = "Test",
            NotificationId = "76",
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var returnedUser = await _userRepository.GetByName(user.Name);

        // Assert
        Assert.NotNull(returnedUser);
        Assert.Equal(user.Name, returnedUser.Name);
    }

    [Fact]
    public async Task GetByName_NonExistingUser_ReturnsNull()
    {
        // Arrange
        var nonExistingUserName = "NonExistingUser";

        // Act
        var returnedUser = await _userRepository.GetByName(nonExistingUserName);

        // Assert
        Assert.Null(returnedUser);
    }

    [Fact]
    public async Task UpdateUser_ExistingUser_UpdatesSuccessfully()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "role1",
        };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ExistingUser",
            Password = "test",
            NotificationId = "76",
            Roles = new List<Role>
            {
                role,
            },
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var userDto = new AddUserDto
        {
            User = new UserDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "ExistingUser",
            },
            Password = "newPassword",
            Roles = new List<string>
            {
                role.Id,
            },
        };

        // Act
        var updatedUser = await _userRepository.UpdateUser(userDto);

        // Assert
        Assert.NotNull(updatedUser);
        Assert.Equal(userDto.User.Name, updatedUser.Name);
        Assert.Equal(userDto.Password, updatedUser.Password);
        Assert.Contains(updatedUser.Roles, r => r.Id == role.Id);
    }

    [Fact]
    public async Task UpdateUser_NonExistingUser_ReturnsNull()
    {
        // Arrange
        var userDto = new AddUserDto
        {
            User = new UserDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "NonExistingUser",
            },
            Password = "password",
        };

        // Act
        var updatedUser = await _userRepository.UpdateUser(userDto);

        // Assert
        Assert.Null(updatedUser);
    }

    [Fact]
    public async Task CheckAccess_AuthorizedUserWithRoleAccess_ReturnsTrue()
    {
        // Arrange
        var targetPage = "TargetPage";
        var userId = "26254d17-90da-47d1-b6bd-aae7d28c43f5";
        var user = new User
        {
            Id = userId,
            Name = "UserTest",
            Password = "test",
            NotificationId = "76",
            Roles = new List<Role>
            {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin",
                RolePermissions = new List<RolePermissions>
                {
                    new()
                    {
                        PermissionId = Guid.NewGuid().ToString(),
                        Permissions = new Permissions
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = targetPage,
                        },
                    },
                },
            },
            },
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var mockPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthentication"));
        _httpContextAccessorMock.Setup(x => x.HttpContext.User).Returns(mockPrincipal);

        var fakeAuthHeader = $"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2" +
            $"NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZ" +
            $"mllciI6IjI2MjU0ZDE3LTkwZGEtNDdkMS1iNmJkLWFhZTdkMjhjNDNmNSIsImh0dHA6Ly9zY2h" +
            $"lbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJMb2dhbiB" +
            $"QYXVsIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsY" +
            $"Wltcy9yb2xlIjoiQWRtaW5pc3RyYXRvciIsImV4cCI6MTcwODU1MzAwN30.VmejqUXG9eRCVtCV-IYS1A3VShqgfqFWyJWuZEmNvOU";
        _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Headers["Authorization"]).Returns(fakeAuthHeader);

        var accessCheck = new AccessCheck
        {
            TargetPage = targetPage,
        };

        // Act
        var hasAccess = await _userRepository.CheckAccess(accessCheck);

        // Assert
        Assert.True(hasAccess);
    }

    [Fact]
    public async Task CheckAccess_NoAuthorized_TargetPageAccessDenied_ReturnFalse()
    {
        // Arrange
        var targetPage = "SomeOtherPage";

        var accessCheck = new AccessCheck
        {
            TargetPage = targetPage,
        };
        _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Headers["Authorization"]).Returns(string.Empty);

        // Act
        var hasAccess = await _userRepository.CheckAccess(accessCheck);

        // Assert
        Assert.False(hasAccess);
    }

    [Fact]
    public async Task CheckAccess_NoAuthorized_TargetPageAccessOk_ReturnTrue()
    {
        // Arrange
        var targetPage = "Games";

        var accessCheck = new AccessCheck
        {
            TargetPage = targetPage,
        };
        _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Headers["Authorization"]).Returns(string.Empty);

        // Act
        var hasAccess = await _userRepository.CheckAccess(accessCheck);

        // Assert
        Assert.True(hasAccess);
    }

    [Fact]
    public async Task CheckAccess_ValidJwtTokenWithoutNameIdentifierClaim_ReturnsFalse()
    {
        // Arrange
        var targetPage = "TargetPage";
        var accessCheck = new AccessCheck
        {
            TargetPage = targetPage,
        };

        // Create a fake token without ClaimTypes.NameIdentifier
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcd16gkdtSecretabcd16gkdtSecret"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);
        var tokenHandler = new JwtSecurityTokenHandler();
        var fakeValidToken = tokenHandler.WriteToken(token);

        var authHeader = $"Bearer {fakeValidToken}";
        _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Headers["Authorization"]).Returns(authHeader);

        // Act
        var hasAccess = await _userRepository.CheckAccess(accessCheck);

        // Assert
        Assert.False(hasAccess);
    }

    [Fact]
    public async Task CheckAccess_ValidJwtTokenButUserNotFound_ReturnsFalse()
    {
        // Arrange
        var targetPage = "TargetPage";
        var accessCheck = new AccessCheck
        {
            TargetPage = targetPage,
        };

        // Create a fake token with a non-existing user Id
        var nonExistentUserId = "non-existent-user-id";
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, nonExistentUserId) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcd16gkdtSecretabcd16gkdtSecret"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);
        var tokenHandler = new JwtSecurityTokenHandler();
        var fakeToken = tokenHandler.WriteToken(token);

        var authHeader = $"Bearer {fakeToken}";
        _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Headers["Authorization"]).Returns(authHeader);

        // Act
        var hasAccess = await _userRepository.CheckAccess(accessCheck);

        // Assert
        Assert.False(hasAccess);
    }

    [Fact]
    public async Task GetAllNotifications_ReturnsAllNotifications()
    {
        // Arrange
        var notifications = new List<Notification>()
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Type = "Email",
                UserNotificationType = new List<User>(),
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Type = "SMS",
                UserNotificationType = new List<User>(),
            },
        };

        _dbContext.Notifications.AddRange(notifications);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetAllNotifications();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notifications.Count, result.Count());
    }

    [Fact]
    public async Task GetUserNotification_ReturnsCorrectNotification()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var notification = new Notification
        {
            Id = Guid.NewGuid().ToString(),
            Type = "Email",
            UserNotificationType = new List<User>(),
        };
        var user = new User
        {
            Id = id,
            Name = "TestUser",
            Password = "password",
            NotificationId = notification.Id,
        };

        _dbContext.Notifications.Add(notification);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetUserNotification(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notification.Id, result.Id);
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
