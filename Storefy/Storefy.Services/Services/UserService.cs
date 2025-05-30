using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;

namespace Storefy.Services.Services;

/// <summary>
/// Implementation of the services needed to manipulate
/// and retrieve information for User entities in the repository.
/// </summary>
/// <inheritdoc cref="IUserService"/>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="httpClient">The HttpClient
    /// to be used by the service for HTTP requests.</param>
    /// <param name="config">Provides access to configuration data.</param>
    /// <param name="unitOfWork">The unit of work
    /// to be used by the user repository.</param>
    public UserService(
        HttpClient httpClient,
        IConfiguration config,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _config = config;
        _httpClient = httpClient;
    }

    private static string ApiUrl => "https://localhost:5037/api";

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsers()
    {
        var users = await _unitOfWork.UserRepository
            .GetAll();

        return users;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Notification>> GetUsersNotification()
    {
        var notifications = await _unitOfWork.UserRepository
            .GetAllNotifications();

        return notifications;
    }

    /// <inheritdoc />
    public async Task<Notification> GetUserNotification(string id)
    {
        var selectedNotification = await _unitOfWork.UserRepository
            .GetUserNotification(id);

        return selectedNotification;
    }

    /// <inheritdoc />
    public async Task<User> GetUserById(string id)
    {
        var user = await _unitOfWork.UserRepository
            .GetById(id);

        return user;
    }

    /// <inheritdoc />
    public async Task<BanHistory> BanUser(string name, string duration)
    {
        var userBan = await _unitOfWork.UserRepository
            .BanUser(name, duration);

        return userBan;
    }

    /// <inheritdoc />
    public string[] GetBanDuration()
    {
        var banDurations = new string[]
        {
            "1 hour",
            "1 day",
            "1 week",
            "1 month",
            "permanent",
        };
        return banDurations;
    }

    /// <inheritdoc />
    public async Task<User> UpdateUser(AddUserDto user)
    {
        var updatedUser = await _unitOfWork.UserRepository
            .UpdateUser(user);

        return updatedUser;
    }

    /// <inheritdoc />
    public async Task<User> DeleteUser(string id)
    {
        var deletedUser = await _unitOfWork
            .UserRepository
            .DeleteUser(id);

        return deletedUser;
    }

    /// <inheritdoc />

    public async Task<bool> AccessCheckAsync(AccessCheck accessCheck)
    {
        return await _unitOfWork.UserRepository.CheckAccess(accessCheck);
    }

    /// <inheritdoc />
    public async Task<LoginResponce> Login(Model model)
    {
        var authModel = new AuthDto
        {
            Email = model.Login,
            Password = model.Password,
        };

        return model.InternalAuth switch
        {
            true => await InternalAuthentication(authModel),
            false => await ExternalAuthentication(authModel),
            _ => throw new InvalidOperationException("Invalid InternalAuth value"),
        };
    }

    /// <inheritdoc />
    public async Task<User> AddUser(AddUserDto user)
    {
        var newUser = await _unitOfWork.UserRepository
            .CreateUser(user);

        return newUser;
    }

    private string GenerateJWTToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("JwtSettings:Key").Value!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, string.Join(",", user.Roles.Select(roles => roles.Name))),
        };
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(5),
            signingCredentials: credentials);
        var responce = tokenHandler.WriteToken(token);

        return responce;
    }

    private async Task<LoginResponce> InternalAuthentication(AuthDto model)
    {
        var user = await _unitOfWork.UserRepository.GetByName(model.Email);

        if (user != null && model.Password == user.Password)
        {
            var result = new LoginResponce
            {
                Token = GenerateJWTToken(user),
            };

            return result;
        }

        throw new UnauthorizedAccessException();
    }

    private async Task<LoginResponce> ExternalAuthentication(AuthDto model)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/auth", model);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var externalResponse = JsonConvert
                .DeserializeObject<ExternalUserDto>(content);
            externalResponse.Password = model.Password;

            var user = await AddExternalUser(externalResponse);
            var result = new LoginResponce
            {
                Token = GenerateJWTToken(user),
            };

            return result;
        }

        throw new UnauthorizedAccessException();
    }

    private async Task<User> AddExternalUser(ExternalUserDto model)
    {
        var userName = $"{model.FirstName} {model.LastName}";
        var user = await _unitOfWork
            .UserRepository
            .GetByName(userName);

        if (user == null)
        {
            var roles = await _unitOfWork.RoleRepository
                .GetRoleByName("User");
            var newUser = new AddUserDto
            {
                User = new UserDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = userName,
                },
                Password = model.Password,
                Roles = new List<string>()
                {
                    roles.Id,
                },
            };

            return await _unitOfWork.UserRepository.CreateUser(newUser);
        }

        return user;
    }
}
