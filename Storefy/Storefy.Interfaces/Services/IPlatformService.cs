using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Services;

/// <summary>
/// Interface for the Platform Service. Defines operations for managing platforms.
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Create a new platform based on data transfer object.
    /// </summary>
    /// <param name="platformDto">Data transfer object for platform creation.</param>
    /// <returns>The asynchronous operation in which task result contains the created platform entity.</returns>
    Task<Platform> AddPlatform(CreateUpdatePlatformDto platformDto);

    /// <summary>
    /// Retrieves a platform entity by its id.
    /// </summary>
    /// <param name="platformId">Unique identifier of the platform.</param>
    /// <returns>The asynchronous operation in which task result contains a specified platform.</returns>
    Task<Platform> GetPlatformById(string platformId);

    /// <summary>
    /// Retrieves all platforms.
    /// </summary>
    /// <returns>The asynchronous operation in which task result contains a collection of platforms.</returns>
    Task<IEnumerable<Platform>> GetAllPlatforms();

    /// <summary>
    /// Updates an existing platform entity with the provided genre data transfer object.
    /// </summary>
    /// <param name="platformDto">Data transfer object for platform update.</param>
    /// <returns>The asynchronous operation in which task result contains the updated platform entity.</returns>
    Task<Platform> UpdatePlatform(CreateUpdatePlatformDto platformDto);

    /// <summary>
    /// Deletes the specified platform entity.
    /// </summary>
    /// <param name="id">Unique identifier of the deleted platform.</param>
    /// <returns>The asynchronous operation in which task result contains the deleted platform entity.</returns>
    Task<Platform> DeletePlatform(string id);

    /// <summary>
    /// Asynchronously retrieves all platforms that a specific game is compatible with.
    /// </summary>
    /// <param name="gameAlias">The alias of the game.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is a collection of Platform objects that the specified game is compatible with.</returns>
    Task<IEnumerable<Platform>> GetPlatformsByGame(string gameAlias, string languageCode);
}
