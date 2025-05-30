using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.API.Controllers;

/// <summary>
/// Represents the User API Controller class.
/// </summary>
[Route("users/")]
[ApiController]
[Authorize(Policy = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">Service to perform operations on Cart.</param>
    /// <param name="logger">Logger service for logging.</param>
    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// HTTP Post method to ban user.
    /// </summary>
    /// <returns>Banned username and duration.</returns>
    [HttpPost("comments/ban")]
    [Authorize(Policy = "Moderator")]
    public async Task<ActionResult<BanHistory>> BanUser(UserBan ban)
    {
        var userBan = await _userService.BanUser(ban.User, ban.Duration);

        return Ok(userBan);
    }

    /// <summary>
    /// HTTP Get methods to get possible ban duration options.
    /// </summary>
    /// <returns>Array with ban duration options.</returns>
    [HttpGet("banDurations")]
    [Authorize(Policy = "Moderator")]
    public IActionResult GetBanDurations()
    {
        var banDurations = _userService.GetBanDuration();
        _logger.LogDebug("Successfully got ban duration options.");

        return Ok(banDurations);
    }

    /// <summary>
    /// HTTP Get method to retrieve all the users in the system.
    /// </summary>
    /// <returns>A list of all users.</returns>
    [HttpGet]
    public async Task<ActionResult<User>> GetAllUsers()
    {
        var users = await _userService.GetUsers();

        return Ok(users);
    }

    /// <summary>
    /// HTTP Post method to Checks if a user has a specific access.
    /// </summary>
    /// <param name="accessCheck">The access check details.</param>
    /// <returns>A boolean value indicating whether the access
    /// check passed or not.</returns>
    [HttpPost("access")]
    [AllowAnonymous]
    public async Task<ActionResult> CheckAccess(AccessCheck accessCheck)
    {
        var hasAccess = await _userService.AccessCheckAsync(accessCheck);

        return hasAccess ? Ok(true) : (ActionResult)Ok(false);
    }

    /// <summary>
    /// HTTP Post method to authenticates a user and returns the login response.
    /// </summary>
    /// <param name="model">The user's login credentials.</param>
    /// <returns>The login response containing the JSON Web Token.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponce>> Login(LoginRequest model)
    {
        var user = await _userService.Login(model.Model);

        return user != null ? Ok(user) : Unauthorized();
    }

    /// <summary>
    /// HTTP Post method to creates a new user.
    /// </summary>
    /// <param name="user">The new user details.</param>
    /// <returns>The created new user details.</returns>
    [HttpPost("create")]
    public async Task<ActionResult<User>> CreateUser(AddUserDto user)
    {
        var newUser = await _userService
            .AddUser(user);

        return CreatedAtAction(nameof(CreateUser), newUser);
    }

    /// <summary>
    /// HTTP Put method to update an existing user.
    /// </summary>
    /// <param name="user">The updated user details.</param>
    /// <returns>The new updated user.</returns>
    [HttpPut("update")]
    public async Task<ActionResult<User>> UpdateUser(AddUserDto user)
    {
        var updatedUser = await _userService
            .UpdateUser(user);

        return Ok(updatedUser);
    }

    /// <summary>
    /// HTTP Delete method to deletes a user.
    /// </summary>
    /// <param name="id">The ID of the user to be deleted.</param>
    /// <returns>The details of the deleted user.</returns>
    [HttpDelete("remove/{id}")]
    public async Task<ActionResult<User>> DeleteUser(string id)
    {
        var deletedUser = await _userService
            .DeleteUser(id);

        return Ok(deletedUser);
    }

    /// <summary>
    /// HTTP Get method to retrie a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The User details if found.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(string id)
    {
        var user = await _userService.GetUserById(id);

        return Ok(user);
    }

    /// <summary>
    /// HTTP Get method to retrive all user's notifications.
    /// </summary>
    /// <returns>The List of user's notifications.</returns>
    [HttpGet("notifications")]
    public async Task<ActionResult<Notification>> GetNotifications()
    {
        var notifications = await _userService
            .GetUsersNotification();

        return Ok(notifications);
    }

    /// <summary>
    /// HTTP Get method to retrive user's notification type.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The user's Notification details.</returns>
    [HttpGet("{id}/notifications")]
    public async Task<ActionResult<Notification>> GetUserNotification(string id)
    {
        var selectedNotification = await _userService
            .GetUserNotification(id);

        return Ok(selectedNotification);
    }
}
