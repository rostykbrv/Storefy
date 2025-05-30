using Storefy.BusinessObjects.Dto;

namespace Storefy.Interfaces.Repositories.Gamestore;

/// <summary>
/// Interface for the Platform Repository. Defines the repository operations for the Platform objects.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IPlatformRepository<T>
    where T : class
{
    /// <summary>
    /// Adds a new Platform based on the provided Platform Data Transfer Object(DTO).
    /// </summary>
    /// <param name="platformDto">Platform DTO containing platform information to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Platform added.</returns>
    Task<T> Add(CreateUpdatePlatformDto platformDto);

    /// <summary>
    /// Returns a Platform based on the provided PlatformId.
    /// </summary>
    /// <param name="id">Platform identifier to fetch the Platform.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the Platform fetched.</returns>
    Task<T> GetById(string id);

    /// <summary>
    /// Returns all available Platforms.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the collection of Platforms.</returns>
    Task<IEnumerable<T>> GetAll();

    /// <summary>
    /// Updates an existing Platform based on the provided PlatformId and Platform Data Transfer Object(DTO).
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the updated Platform.</returns>
    Task<T> Update(CreateUpdatePlatformDto platformDto);

    /// <summary>
    /// Deletes the specified object from the repository.
    /// </summary>
    /// <param name="entity">Object to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deleted object.</returns>
    Task<T> Delete(T entity);

    /// <summary>
    /// Asynchronously retrieves all platforms that a specific game is compatible with.
    /// </summary>
    /// <param name="gameAlias">A string representing the alias of the game.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains an enumerable collection of Platforms for the specified game.</returns>
    Task<IEnumerable<T>> GetPlatformsByGameAlias(string gameAlias, string languageCode);
}
