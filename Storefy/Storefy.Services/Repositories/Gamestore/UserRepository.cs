using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Repositories.Gamestore;
using Storefy.Services.Data;

namespace Storefy.Services.Repositories.Gamestore;

/// <summary>
/// Repository to manage the operations of User entities using a database context.
/// </summary>
/// <inheritdoc cref="IUserRepository{T}"/>
public class UserRepository : IUserRepository<User>
{
    private readonly StorefyDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The data base context
    /// to be used by the repository.</param>
    /// <param name="httpContextAccessor">Provides access to the
    /// HTTP context for the current request.</param>
    public UserRepository(StorefyDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<User> CreateUser(AddUserDto userDto)
    {
        var existingUser = await _dbContext.Users
        .FirstOrDefaultAsync(u => u.Name == userDto.User.Name);
        var notificationType = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Type == "Email");

        if (existingUser == null)
        {
            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = userDto.User.Name,
                Password = userDto.Password,
                NotificationId = notificationType.Id,
                NotificationType = notificationType,
            };

            foreach (var roleId in userDto.Roles)
            {
                var selectedRole = await _dbContext.Roles
                    .FindAsync(roleId);

                if (selectedRole != null)
                {
                    newUser.Roles.Add(selectedRole);
                }
            }

            await _dbContext.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return newUser;
        }

        throw new InvalidOperationException("User with this name already exists.");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Notification>> GetAllNotifications()
    {
        var notifications = await _dbContext.Notifications
            .ToListAsync();

        return notifications;
    }

    /// <inheritdoc />
    public async Task<Notification> GetUserNotification(string id)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id.Equals(id));
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id.Equals(user.NotificationId));

        return notification;
    }

    /// <inheritdoc />
    public async Task<User> DeleteUser(string id)
    {
        var deletedUser = await _dbContext
            .Users
            .FindAsync(id) ?? throw new InvalidOperationException();

        _dbContext.Users.Remove(deletedUser);
        await _dbContext.SaveChangesAsync();

        return deletedUser;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetAll()
    {
        var users = await _dbContext.Users
            .ToListAsync();

        return users;
    }

    /// <inheritdoc />
    public async Task<BanHistory> BanUser(string name, string duration)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Name == name);

        if (user != null)
        {
            var userBan = new BanHistory
            {
                User = user,
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                BanStarted = DateTime.UtcNow,
                BanEnds = duration switch
                {
                    "1 hour" => DateTime.UtcNow.AddHours(1),
                    "1 day" => DateTime.UtcNow.AddDays(1),
                    "1 week" => DateTime.UtcNow.AddDays(7),
                    "1 month" => DateTime.UtcNow.AddMonths(1),
                    "permanent" => DateTime.MaxValue,
                    _ => throw new InvalidDataException("Invalid ban duration."),
                },
            };

            _dbContext.BanHistories.Add(userBan);
            await _dbContext.SaveChangesAsync();

            return userBan;
        }

        throw new ArgumentException("User not found");
    }

    /// <inheritdoc />
    public async Task<User> GetById(string id)
    {
        var user = await _dbContext.Users
            .FindAsync(id);

        return user;
    }

    /// <inheritdoc />
    public async Task<User> GetByName(string name)
    {
        var user = await _dbContext.Users
            .Include(u => u.Roles)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permissions)
            .Where(u => u.Name == name)
            .FirstOrDefaultAsync();

        return user;
    }

    /// <inheritdoc />
    public async Task<User> UpdateUser(AddUserDto userDto)
    {
        var existedUser = await _dbContext.Users
            .Include(u => u.Roles)
            .Where(u => u.Name == userDto.User.Name)
            .FirstOrDefaultAsync();

        if (existedUser != null)
        {
            existedUser.Name = userDto.User.Name;
            existedUser.Password = userDto.Password;
            existedUser.NotificationId = userDto.Notification;
            existedUser.Roles.Clear();

            foreach (var role in userDto.Roles)
            {
                var selectedRole = await _dbContext.Roles
                    .FindAsync(role);

                if (selectedRole != null)
                {
                    existedUser.Roles.Add(selectedRole);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        return existedUser;
    }

    /// <inheritdoc />
    public async Task<bool> CheckAccess(AccessCheck accessCheck)
    {
        var authHeader = _httpContextAccessor.HttpContext.Request
            .Headers["Authorization"]
            .ToString();

        if (string.IsNullOrWhiteSpace(authHeader))
        {
            var readOnlyPages = ReadOnlyGuestPermissions();

            return readOnlyPages.Contains(accessCheck.TargetPage);
        }

        var token = authHeader["Bearer ".Length..];
        var userIdClaim = GetUserIdClaim(token);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return false;
        }

        var user = await _dbContext.Users
            .Include(u => u.Roles)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permissions)
            .FirstOrDefaultAsync(u => u.Id == userIdClaim);

        return user != null && user.Roles
            .Exists(role => role.RolePermissions
            .Exists(rp => rp.Permissions.Name == accessCheck.TargetPage));
    }

    private static string[] ReadOnlyGuestPermissions()
    {
        var readOnlyPermissions = new string[]
        {
            "Games",
            "Game",
            "Genres",
            "Genre",
            "Comments",
            "Publishers",
            "Publisher",
            "Platforms",
            "Platform",
            "AddComment",
            "ReplyComment",
            "QuoteComment",
        };

        return readOnlyPermissions;
    }

    private static string GetUserIdClaim(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var claims = jwtToken.Claims;

        return claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
            .Value;
    }
}
