using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Repositories.Gamestore;

/// <summary>
/// Interface for the Game Repository. Defines the repository operations for the Game objects.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IGameRepository<T>
    where T : class
{
    /// <summary>
    /// Adds a new Game encoded in the provided GameDto.
    /// </summary>
    /// <param name="gameDto">Data transfer object containing data for the new game.</param>
    /// <returns>A task that represents the asynchronous save operation.
    /// The task result contains the added Game object.</returns>
    Task<T> Add(CreateUpdateGameDto gameDto);

    /// <summary>
    /// Deletes the specified object from the repository.
    /// </summary>
    /// <param name="entity">Object to be deleted.</param>
    /// <returns>A task that represents the asynchronous save operation.
    /// The task result contains the deleted object.</returns>
    Task<T> Delete(T entity);

    /// <summary>
    /// Retrieves all games, total number of pages and current page.
    /// </summary>
    /// <param name="filters">Parameters for filtering games.</param>
    /// <returns>Asynchronous peration, result contains a collection
    /// of all Games, total pages numbers and current page.</returns>
    Task<GamePageResult> GetAllGames(FilterOptions filters, string languageCode);

    /// <summary>
    /// Retrieves all Games from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains a collection of all Games.</returns>
    Task<IEnumerable<T>> GetAll();

    /// <summary>
    /// Updates a Game object with the data encoded in the given GameDto.
    /// </summary>
    /// <param name="updatedGame">Data transfer object containing the new data for the Game.</param>
    /// <returns>A task that represents the asynchronous save operation.
    /// he task result contains the updated Game object.</returns>
    Task<T> Update(CreateUpdateGameDto updatedGame);

    /// <summary>
    /// Retrieves a Game based on its alias.
    /// </summary>
    /// <param name="alias">Unique short name for the game to retrieve.</param>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains the retrieved Game object.</returns>
    Task<T> GetByAlias(string alias, string languageCode);

    /// <summary>
    /// Retrieves all games associated with a specific genre.
    /// </summary>
    /// <param name="genreId">Unique identifier of the genre.</param>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains a collection of all Games associated with the given genre.</returns>
    Task<IEnumerable<T>> GetGamesByGenre(string genreId);

    /// <summary>
    /// Retrieves all games associated with a specific platform.
    /// </summary>
    /// <param name="platformId">Unique identifier of the platform.</param>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains a collection of all Games associated with the given platform.</returns>
    Task<IEnumerable<T>> GetGamesByPlatform(string platformId);

    /// <summary>
    /// Retrieves all games associated with a specific publisher.
    /// </summary>
    /// <param name="companyName">Company name of the publisher.</param>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains a collection of all Games associated with the given publisher.</returns>
    Task<IEnumerable<T>> GetGamesByPublisher(string companyName);

    /// <summary>
    /// Checks if a given alias already exists within the repository.
    /// </summary>
    /// <param name="alias">Alias to check for existence.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the boolean result of this check.</returns>
    Task<bool> AliasExists(string alias);

    /// <summary>
    /// Processes the purchase of a game, creating a new order for the game with specified game alias.
    /// </summary>
    /// <param name="gameAlias">The game alias for the game being purchased.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the created Order object.</returns>
    Task<Order> BuyGame(string gameAlias);

    /// <summary>
    /// Retrieves a Game based on its unique identification.
    /// </summary>
    /// <param name="id">Unique identifier of the game to retrieve.</param>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains the retrieved Game object.</returns>
    Task<T> GetById(string id);

    /// <summary>
    /// Retrieves all comments associated with a specific game.
    /// </summary>
    /// <param name="gameAlias">The game alias for the game being selected.</param>
    /// <returns>The task result contains a collection of all Comments associated with the given game.</returns>
    Task<IEnumerable<Comment>> GetComments(string gameAlias);

    /// <summary>
    /// Add a comment to the game.
    /// </summary>
    /// <param name="gameAlias">The game alias for the game being selected.</param>
    /// <param name="commentDto">Data transfer object containing the new data for the Comment.</param>
    /// <returns>The task result contains the new Comment object.</returns>
    Task<Comment> AddCommentToGame(string gameAlias, AddCommentDto commentDto);

    /// <summary>
    /// Deletes the specified comment.
    /// </summary>
    /// <param name="commentId">Unique identifier of the comment to delete.</param>
    /// <returns>The task result contains the deleted Comment object.</returns>
    Task<Comment> DeleteComment(string commentId);
}
