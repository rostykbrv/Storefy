using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Services;

/// <summary>
/// Interface for the Genre Service. Defines operations for managing genres.
/// </summary>
public interface IGenreService
{
    /// <summary>
    /// Create a new genre based on data transfer object.
    /// </summary>
    /// <param name="genreDto">Data transfer object for genre creation.</param>
    /// <returns>The asynchronous operation in which task result contains the created genre entity.</returns>
    Task<Genre> AddGenre(CreateUpdateGenreDto genreDto);

    /// <summary>
    ///  Retrieves a genre entity by its id.
    /// </summary>
    /// <param name="genreId">Unique identifier of the genre.</param>
    /// <returns>The asynchronous operation in which task result contains a specified genre.</returns>
    Task<Genre> GetGenreById(string genreId);

    /// <summary>
    /// Retrieves all genres.
    /// </summary>
    /// <returns>The asynchronous operation in which task result contains a collection of genres.</returns>
    Task<IEnumerable<Genre>> GetAllGenres();

    /// <summary>
    /// Updates an existing genre entity with the provided genre data transfer object.
    /// </summary>
    /// <param name="genreDto">Data transfer object for genre updates.</param>
    /// <returns>The asynchronous operation in which task result contains the updated genre entity.</returns>
    Task<Genre> UpdateGenre(CreateUpdateGenreDto genreDto);

    /// <summary>
    /// Asynchronously retrieves all genres that have a specific parent genre.
    /// </summary>
    /// <param name="parentGenreId">The unique identifier of the parent genre.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is a collection of Genre objects that have the specified parent genre.</returns>
    Task<IEnumerable<Genre>> GetGenresByParentGenre(string parentGenreId);

    /// <summary>
    /// Deletes the specified genre entity.
    /// </summary>
    /// <param name="id">Unique identifier of the deleted genre.</param>
    /// <returns>The asynchronous operation in which task result contains the deleted genre entity.</returns>
    Task<Genre> DeleteGenre(string id);

    /// <summary>
    /// Asynchronously retrieves all genres that a specific game belongs to.
    /// </summary>
    /// <param name="gameAlias">The alias of the game.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is a collection of Genre objects that the specified game belongs to.</returns>
    Task<IEnumerable<Genre>> GetGenresByGameAlias(string gameAlias, string languageCode);
}
