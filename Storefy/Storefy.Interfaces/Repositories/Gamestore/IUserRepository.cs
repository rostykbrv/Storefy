using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Repositories.Gamestore;

/// <summary>
/// Interface for the User Repository. Defines the repository operations for the User objects.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IUserRepository<T>
    where T : class
{
    /// <summary>
    /// Retrieves all Users from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains a collection of all Users.</returns>
    Task<IEnumerable<T>> GetAll();

    /// <summary>
    /// Retrieves all Notifications from the database.
    /// </summary>
    /// <returns>The task result contains a collection of all Notifications.</returns>
    Task<IEnumerable<Notification>> GetAllNotifications();

    /// <summary>
    /// Retrieve user's preferred Notification.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns>The task result contains the notification of the user.</returns>
    Task<Notification> GetUserNotification(string id);

    /// <summary>
    /// Bans a user.
    /// </summary>
    /// <param name="name">The name of the user to ban.</param>
    /// <param name="duration">The duration of the ban.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the ban history of the user.</returns>
    Task<BanHistory> BanUser(string name, string duration);

    /// <summary>
    /// Retrieves a user by his/her id.
    /// </summary>
    /// <param name="id">The id of the user to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the user with matching id.</returns>
    Task<T> GetById(string id);

    /// <summary>
    /// Retrieves a user by his/her name.
    /// </summary>
    /// <param name="name">The name of the user to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the user with matching name.</returns>
    Task<T> GetByName(string name);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="userDto">Data transfer object containing new user data.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the newly created user.</returns>
    Task<T> CreateUser(AddUserDto userDto);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="userDto">Data transfer object containing updated user data.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the updated user.</returns>
    Task<T> UpdateUser(AddUserDto userDto);

    /// <summary>
    /// Deletes an existing user.
    /// </summary>
    /// <param name="id">The id of the user to delete.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the deleted user.</returns>
    Task<T> DeleteUser(string id);

    /// <summary>
    /// Checks if a user has a specific access.
    /// </summary>
    /// <param name="accessCheck">Access check information.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is a boolean indicating whether the user has the access or not.</returns>
    Task<bool> CheckAccess(AccessCheck accessCheck);
}
