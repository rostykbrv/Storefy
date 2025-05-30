using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Repositories.Gamestore;

/// <summary>
/// Interface for the Genre Repository. Defines the repository operations for the Genre objects.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IGenreRepository<T>
    where T : class
{
    /// <summary>
    /// Adds a new Genre.
    /// </summary>
    /// <param name="genreDto">Genre DTO containing genre information to add.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the Genre added.</returns>
    Task<Genre> Add(CreateUpdateGenreDto genreDto);

    /// <summary>
    /// Checks if a specific genre exists in the repository.
    /// </summary>
    /// <param name="genreName">The name of the genre.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a boolean value representing the existence of the genre.</returns>
    Task<bool> GenreExists(string genreName);

    /// <summary>
    /// Returns a Genre based on the provided GenreId.
    /// </summary>
    /// <param name="id">Genre identifier to fetch the Genre.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Genre fetched.</returns>
    Task<T> GetById(string id);

    /// <summary>
    /// Returns all available Genres.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of Genres.</returns>
    Task<IEnumerable<T>> GetAll();

    /// <summary>
    /// Updates an existing Genre based on the provided GenreId and Genre Data Transfer Object(DTO).
    /// </summary>
    /// <param name="genreDto">Genre DTO containg the details to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated Genre.</returns>
    Task<Genre> Update(CreateUpdateGenreDto genreDto);

    /// <summary>
    /// Asynchronously retrieves all genres that have a specific parent genre from the repository.
    /// </summary>
    /// <param name="parentGenreId">A string representing the identifier of the parent genre.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains an enumerable collection of Genres that match the specified parent genre identifier.</returns>
    Task<IEnumerable<Genre>> GetGenresByParentGenre(string parentGenreId);

    /// <summary>
    /// Deletes the specified object from the repository.
    /// </summary>
    /// <param name="entity">Object to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the deleted object.</returns>
    Task<T> Delete(T entity);

    /// <summary>
    /// Asynchronously retrieves all genres that a specific game belongs to.
    /// </summary>
    /// <param name="gameAlias">A string representing the alias of the game.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains an enumerable collection of Genres for the specified game.</returns>
    Task<IEnumerable<T>> GetGenresByGame(string gameAlias, string languageCode);
}
