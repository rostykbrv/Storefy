using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Services;

/// <summary>
/// Interface for the Game Service. Defines operations for managing games.
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Create a new game based on data transfer object.
    /// </summary>
    /// <param name="gameDto">Data transfer object for game creation.</param>
    /// <returns>The asynchronous operation in which task result contains
    /// the created game entity.</returns>
    Task<Game> CreateGame(CreateUpdateGameDto gameDto);

    /// <summary>
    /// Retrieves a game entity by its alias.
    /// </summary>
    /// <param name="gameAlias">Unique short name of the game.</param>
    /// <returns>The asynchronous operation in which task result contains
    /// the retrieved game entity.</returns>
    Task<Game> GetGameByAlias(string gameAlias, string languageCode);

    /// <summary>
    /// Retrieves a game entity by its genre.
    /// </summary>
    /// <param name="genreId">Unique identifier of the genre.</param>
    /// <returns>The asynchronous operation in which task result contains
    /// a collection of games in the specified genre.</returns>
    Task<IEnumerable<Game>> GetGamesByGenre(string genreId);

    /// <summary>
    /// Retrieves a game entity by its platform.
    /// </summary>
    /// <param name="platformId">Unique identifier of the platform.</param>
    /// <returns>The asynchronous operation in which task result contains
    /// a collection of games in the specified platform.</returns>
    Task<IEnumerable<Game>> GetGamesByPlatform(string platformId);

    /// <summary>
    /// Updates an existing game entity with the provided game data transfer object.
    /// </summary>
    /// <param name="gameDto">Data transfer object containing the new game data.</param>
    /// <returns>The asynchronous operation in which task result contains the updated game entity.</returns>
    Task<Game> UpdateGame(CreateUpdateGameDto gameDto);

    /// <summary>
    /// Deletes the specified game entity.
    /// </summary>
    /// <param name="gameAlias">Unique short name of the game to be deleted.</param>
    /// <returns>The asynchronous operation in which task result contains the deleted game entity.</returns>
    Task<Game> DeleteGame(string gameAlias, string languageCode);

    /// <summary>
    /// Retrieves all games, total number of pages and current page.
    /// </summary>
    /// <returns>The asynchronous operation in which task result contains a collection
    /// of all Games, total pages numbers and current page.</returns>
    Task<GamePageResult> GetAllGames(FilterOptions filters, string languageCode);

    /// <summary>
    /// Returns sorting options available for game listings.
    /// </summary>
    /// <returns>An array of sorting options.</returns>
    string[] GetSortingOptions();

    /// <summary>
    /// Returns published date filter options for game listings.
    /// </summary>
    /// <returns>An array of date filter options.</returns>
    string[] GetPublishedDateOptions();

    /// <summary>
    /// Returns paging options for game listings.
    /// </summary>
    /// <returns>An array of integers representing page sizes.</returns>
    int[] GetPagingOptions();

    /// <summary>
    /// Retrieves all existing game entities.
    /// </summary>
    /// <returns>The asynchronous operation in which
    /// task result contains a collection of all games.</returns>

    Task<IEnumerable<Game>> GetAll();

    /// <summary>
    /// Asynchronously retrieves all games published by a specific publisher.
    /// </summary>
    /// <param name="companyName">The name of the publishing company.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is a collection of Game objects published by
    /// the specified publisher.</returns>
    Task<IEnumerable<Game>> GetGamesByPublisher(string companyName);

    /// <summary>
    /// Download txt file with game's information.
    /// </summary>
    /// <param name="gameAlias">Unique short name of the game to be downloaded.</param>
    /// <returns>The asynchronous operation in which task result
    /// contains the downloaded file result entity.</returns>
    Task<FileResult> DownloadGame(string gameAlias, string languageCode);

    /// <summary>
    /// Processes the purchase of a game, creating a new order for the specified game alias.
    /// </summary>
    /// <param name="gameAlias">The game alias for the game being purchased.</param>
    /// <returns>A task representing the asynchronous operation.
    /// Task result contains the created Order object.</returns>
    Task<Order> BuyGame(string gameAlias);

    /// <summary>
    /// Retrieves a game object based on its unique identification.
    /// </summary>
    /// <param name="gameId">The unique identifier of the game to retrieve.</param>
    /// <returns>A task representing the asynchronous operation.
    /// Task result contains the retrieved Game object.</returns>
    Task<Game> GetGameById(string gameId);

    /// <summary>
    /// Retrieves all comments associated with a specific game.
    /// </summary>
    /// <param name="gameAlias">The game alias for the game being selected.</param>
    /// <returns>The task result contains a collection of all
    /// Comments associated with the given game.</returns>
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
