using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Services;

/// <summary>
/// Interface for the User Service. Defines operations for managing users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gives ban to the user.
    /// </summary>
    /// <param name="name">User name.</param>
    /// <param name="duration">Ban duration.</param>
    /// <returns>Boolean ban user status.</returns>
    Task<BanHistory> BanUser(string name, string duration);

    /// <summary>
    /// Get the ban duration options.
    /// </summary>
    /// <returns>Array with ban duration options.</returns>
    string[] GetBanDuration();

    /// <summary>
    /// Get the all user's notification.
    /// </summary>
    /// <returns>List of returned notifications.</returns>
    Task<IEnumerable<Notification>> GetUsersNotification();

    /// <summary>
    /// Get preferred user's notification.
    /// </summary>
    /// <param name="id">The id of the selected user.</param>
    /// <returns>Selected user's notification.</returns>
    Task<Notification> GetUserNotification(string id);

    /// <summary>
    /// Logs a user into the system.
    /// </summary>
    /// <param name="model">Login information.</param>
    /// <returns>A task representing the asynchronous operation,
    /// result contains the login response.</returns>
    Task<LoginResponce> Login(Model model);

    /// <summary>
    /// Checks if a user has a specific access.
    /// </summary>
    /// <param name="accessCheck">Details about the access to check.</param>
    /// <returns>A task representing the asynchronous operation,
    /// task result is a boolean indicating whether
    /// the user has the specified access.</returns>
    Task<bool> AccessCheckAsync(AccessCheck accessCheck);

    /// <summary>
    /// Retrieves a user by their id.
    /// </summary>
    /// <param name="id">The id of the desired user.</param>
    /// <returns>A task representing the asynchronous operation,
    /// result contains the User with the provided id.</returns>
    Task<User> GetUserById(string id);

    /// <summary>
    /// Retrieves all users in the system.
    /// </summary>
    /// <returns>A task representing the asynchronous operation,
    /// result contains a list of all users.</returns>
    Task<IEnumerable<User>> GetUsers();

    /// <summary>
    /// Adds a new user to the system.
    /// </summary>
    /// <param name="user">A data transfer object containing
    /// the details of the user to add.</param>
    /// <returns>A task representing the asynchronous operation,
    /// the result contains the added user.</returns>
    Task<User> AddUser(AddUserDto user);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">A data transfer object containing
    /// the updated details of the user.</param>
    /// <returns>A task representing the asynchronous operation,
    /// the result contains the updated user.</returns>
    Task<User> UpdateUser(AddUserDto user);

    /// <summary>
    /// Deletes a user from the system.
    /// </summary>
    /// <param name="id">The id of the user to delete.</param>
    /// <returns>A task representing the asynchronous operation,
    /// the result contains the deleted user.</returns>
    Task<User> DeleteUser(string id);
}