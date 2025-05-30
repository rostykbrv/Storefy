using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Repositories.Gamestore;

/// <summary>
/// Interface for the Publisher Repository. Defines the repository operations for the Publisher objects.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IPublisherRepository<T>
    where T : class
{
    /// <summary>
    /// Retrieves all Publishers from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains a collection of all Publishers.</returns>
    Task<IEnumerable<T>> GetAll();

    /// <summary>
    /// Asynchronously retrieves the Publisher for a specific game.
    /// </summary>
    /// <param name="gameAlias">The alias of the game.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is the Publisher of the specific game.</returns>
    Task<T> GetPublisherByGame(string gameAlias);

    /// <summary>
    /// Asynchronously adds a new Publisher to the repository.
    /// </summary>
    /// <param name="publisher">The CreateUpdatePublisherDto object that represents the new Publisher to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the Publisher that was added.</returns>
    Task<Publisher> AddPublisher(CreateUpdatePublisherDto publisher);

    /// <summary>
    /// Asynchronously retrieves a Publisher information based on the provided company name.
    /// </summary>
    /// <param name="companyName">The name of the company.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is the Publisher with the provided company name.</returns>
    Task<T> GetPublisherInformation(string companyName);

    /// <summary>
    /// Asynchronously updates an existing Publisher in the repository.
    /// </summary>
    /// <param name="publisher">The CreateUpdatePublisherDto object representing the Publisher to update.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is the Publisher that was updated.</returns>
    Task<Publisher> UpdatePublisher(CreateUpdatePublisherDto publisher);

    /// <summary>
    /// Asynchronously removes the specified entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is the entity that was removed.</returns>
    Task<T> Delete(T entity);

    /// <summary>
    /// Asynchronously retrieves Publisher information based on the provided Id.
    /// </summary>
    /// <param name="id">The Id of the Publisher to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is the Publisher with the provided Id.</returns>
    Task<Publisher> GetPublisherById(string id);
}
