using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.API.Controllers;

/// <summary>
/// Represents the Game API Controller class.
/// </summary>
[Route("games/")]
[ApiController]
[Authorize(Policy = "Manager")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IBlobService _blobService;
    private readonly ILogger<GameController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameController"/> class.
    /// </summary>
    /// <param name="gameService">The game service to use.</param>
    /// <param name="logger">The logger to use.</param>
    public GameController(
        IGameService gameService,
        ILogger<GameController> logger,
        IBlobService blobService)
    {
        _blobService = blobService;
        _gameService = gameService;
        _logger = logger;
    }

    /// <summary>
    /// HTTP Post method to create a new game.
    /// </summary>
    /// <param name="newGame">Data Transfer Object of the Game that is to be created.</param>
    /// <returns>If successful, it returns the created game, else it returns an error.</returns>
    [HttpPost("new")]
    public async Task<ActionResult<Game>> CreateGame(CreateUpdateGameDto newGame)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Failed to create game due to bad request");

            return BadRequest(ModelState);
        }

        var createdGame = await _gameService.CreateGame(newGame);
        await _blobService
            .UploadImageAsync(newGame.Game.Key, newGame.Image, "web");
        _logger.LogDebug($"Successfully created a new game with id {createdGame.Id}");

        return CreatedAtAction(nameof(CreateGame), createdGame);
    }

    /// <summary>
    /// HTTP Get method to retrieve the detailed description of a game.
    /// </summary>
    /// <param name="gamealias">The unique identifier of the game.</param>
    /// <returns>A game description if successful, else an error.</returns>
    [HttpGet("details/{gamealias}/{languageCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<Game>> GetGameDetailedDescription(string gamealias, string languageCode)
    {
        if (string.IsNullOrEmpty(gamealias))
        {
            _logger.LogWarning("Failed to get game details due to bad request");

            return BadRequest("Game alias should not be empty.");
        }

        var game = await _gameService.GetGameByAlias(gamealias, languageCode);

        if (game == null)
        {
            _logger.LogWarning($"Game with alias '{gamealias}' not found.");

            return NotFound($"Game with alias '{gamealias}' not found.");
        }
        else
        {
            _logger.LogDebug($"Successfully returned game with alias {gamealias}");

            return Ok(game);
        }
    }

    /// <summary>
    /// HTTP Get method to retrieve the image of a game.
    /// </summary>
    /// <param name="gameAlias">The unique identifier of the game.</param>
    /// <returns>Returns the image file for the game.</returns>
    [HttpGet("{gameAlias}/image")]
    [ResponseCache(Duration = 90, Location = ResponseCacheLocation.Client)]
    [AllowAnonymous]
    public async Task<IActionResult> GetGameImage(string gameAlias)
    {
        var stream = await _blobService.GetBlobAsync(gameAlias, "web");

        return stream == null ? NotFound() : File(stream, "image/jpeg");
    }

    /// <summary>
    /// HTTP Get method to get games by id.
    /// </summary>
    /// <param name="id">The unique identifier of the game.</param>
    /// <returns>Game details with a certain id if successful, else an error.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Game>> GetGameById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            _logger.LogWarning("Failed to get game details due to bad request");

            return BadRequest("Game id should not be empty.");
        }

        var game = await _gameService.GetGameById(id);

        if (game == null)
        {
            _logger.LogWarning($"Game with id '{id}' not found.");

            return NotFound($"Game with id '{id}' not found.");
        }
        else
        {
            _logger.LogDebug($"Successfully returned game with id - {id}");

            return Ok(game);
        }
    }

    /// <summary>
    /// HTTP Put method to update a game.
    /// </summary>
    /// <param name="updatedGame">Data Transfer Object of the game that is to be updated.</param>
    /// <returns>The updated game if successful, else an error.</returns>
    [HttpPut("update")]
    public async Task<ActionResult<Game>> UpdateGame(CreateUpdateGameDto updatedGame)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Failed to update game details due to bad request");

            return BadRequest(ModelState);
        }

        var game = await _gameService
            .UpdateGame(updatedGame);
        await _blobService
            .UploadImageAsync(updatedGame.Game.Key, updatedGame.Image, "web");
        _logger.LogDebug($"Successfully updated game with alias {game.Key}");

        return Ok(game);
    }

    /// <summary>
    /// HTTP Delete method to delete a game.
    /// </summary>
    /// <param name="gamealias">The unique identifier of the game to delete.</param>
    /// <returns>No content if successful, else an error.</returns>
    [HttpDelete("remove/{gamealias}")]
    public async Task<ActionResult<Game>> DeleteGame(string gamealias, string languageCode)
    {
        var deletedGame = await _gameService.DeleteGame(gamealias, languageCode);
        _logger.LogDebug($"Successfully deleted game with alias {gamealias}");

        return deletedGame;
    }

    /// <summary>
    /// HTTP Get method to download a game.
    /// </summary>
    /// <param name="gamealias">The unique identifier of the game to download.</param>
    /// <returns>The file with downloaded game if successful, else an error.</returns>
    [HttpGet("{gamealias}/{languageCode}/download")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadGame(string gamealias, string languageCode)
    {
        var fileResult = await _gameService.DownloadGame(gamealias, languageCode);

        return File(fileResult.FileBytes, fileResult.ContentType, fileResult.FileName);
    }

    /// <summary>
    /// HTTP Get method to get all game page result.
    /// </summary>
    /// <param name="languageCode">Parameters for language code.</param>
    /// <param name="filters">Parameters for filtering games.</param>
    /// <returns>Collection of games, total amount of pages
    /// and current page.</returns>
    [HttpGet("language/{languageCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<GamePageResult>> GetAllGames(string languageCode, [FromQuery] FilterOptions filters)
    {
        var games = await _gameService.GetAllGames(filters, languageCode);
        _logger.LogDebug($"Successfully get all games");

        return Ok(games);
    }

    /// <summary>
    /// HTTP Get method to get all games.
    /// </summary>
    /// <returns>Returns collection of all games.</returns>
    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<ActionResult<Game>> GetAll()
    {
        var games = await _gameService
            .GetAll();

        return Ok(games);
    }

    /// <summary>
    /// HTTP Get method to get all sorting options.
    /// </summary>
    /// <returns>Collection of sorting optios for games.</returns>
    [HttpGet("sorting/filter")]
    [AllowAnonymous]
    public IActionResult GetSortingOptions()
    {
        var sortingOptions = _gameService.GetSortingOptions();
        _logger.LogDebug($"Successfully get all sorting options");

        return Ok(sortingOptions);
    }

    /// <summary>
    /// HTTP Get method to get all publication date options.
    /// </summary>
    /// <returns>Collection of date options for filtering games.</returns>
    [HttpGet("date/filter")]
    [AllowAnonymous]
    public IActionResult GetPublishedDateOptions()
    {
        var dateOptions = _gameService.GetPublishedDateOptions();
        _logger.LogDebug($"Successfully get all date filter options");

        return Ok(dateOptions);
    }

    /// <summary>
    /// HTTP Get method to get paging options.
    /// </summary>
    /// <returns>Collection of game paging options.</returns>
    [HttpGet("paging/filter")]
    [AllowAnonymous]
    public IActionResult GetPagingOptions()
    {
        var pagingOptions = _gameService.GetPagingOptions();
        _logger.LogDebug($"Successfully get all paging options");

        return Ok(pagingOptions);
    }

    /// <summary>
    /// HTTP Get method to get games by the genre.
    /// </summary>
    /// <param name="genreId">The unique identifier of the genre.</param>
    /// <returns>All games with a certain genre if successful, else an error.</returns>
    [HttpGet("genre/{genreId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Game>>> GetGamesByGenre(string genreId)
    {
        var games = await _gameService.GetGamesByGenre(genreId);

        if (!games.Any())
        {
            _logger.LogWarning($"No games with this genre - {genreId}");

            return NoContent();
        }
        else
        {
            _logger.LogDebug($"Successfully get all games with genre Id - {genreId}");

            return Ok(games);
        }
    }

    /// <summary>
    /// HTTP Get method to get games by the platform.
    /// </summary>
    /// <param name="platformId">The unique identifier of the platform.</param>
    /// <returns>All games with a certain platform if successful, else an error.</returns>
    [HttpGet("platform/{platformId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Game>>> GetGamesByPlatform(string platformId)
    {
        var games = await _gameService.GetGamesByPlatform(platformId);

        if (!games.Any())
        {
            _logger.LogWarning($"No games with this platform Id - {platformId}");

            return NoContent();
        }
        else
        {
            _logger.LogDebug($"Successfully get all games with platform Id - {platformId}");

            return Ok(games);
        }
    }

    /// <summary>
    /// HTTP Get method to get games by the publisher.
    /// </summary>
    /// <param name="companyName">The name of publisher company.</param>
    /// <returns>All games with a certain publisher if successful, else an error.</returns>
    [HttpGet("publisher/{companyName}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Game>>> GetGamesByPublisher(string companyName)
    {
        var games = await _gameService.GetGamesByPublisher(companyName);

        if (!games.Any())
        {
            _logger.LogWarning($"No games with this publisher - {companyName}");

            return NoContent();
        }
        else
        {
            _logger.LogDebug($"Successfully get all games with this publisher - {companyName}");

            return Ok(games);
        }
    }

    /// <summary>
    /// HTTP Post method to add game to the cart.
    /// </summary>
    /// <param name="gamealias">The alias of game.</param>
    /// <returns>Game details, added to the cart.</returns>
    [HttpGet("{gamealias}/buy")]
    [Authorize(Policy = "User")]
    public async Task<ActionResult<Order>> CreateGameOrder(string gamealias)
    {
        var order = await _gameService.BuyGame(gamealias);
        _logger.LogDebug("Successfully added game to the cart.");

        return Ok(order);
    }

    /// <summary>
    /// HTTP Get method to get all game's comments.
    /// </summary>
    /// <param name="gamealias">The alias of game.</param>
    /// <returns>Collection of comments left to the game.</returns>
    [HttpGet("{gamealias}/comments")]
    [AllowAnonymous]
    public async Task<ObjectResult> GetComments(string gamealias)
    {
        var comments = await _gameService.GetComments(gamealias);
        _logger.LogDebug("Successfully get all comments of a game.");

        return Ok(comments);
    }

    /// <summary>
    /// HTTP Post method to add new comment to the game.
    /// </summary>
    /// <param name="gamealias">The alias of game.</param>
    /// <param name="commentDto">Data Transfer Object of the Comment that is to be created.</param>
    /// <returns>Comment that was created.</returns>
    [HttpPost("{gamealias}/comments")]
    [AllowAnonymous]
    public async Task<ActionResult<Comment>> AddCommentToGame(string gamealias, AddCommentDto commentDto)
    {
        var createdComment = await _gameService.AddCommentToGame(gamealias, commentDto);
        _logger.LogDebug("Successfully added a new comment.");

        return CreatedAtAction(nameof(AddCommentToGame), createdComment);
    }

    /// <summary>
    /// HTTP Delete method to delete comment from game.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment.</param>
    /// <returns>Comment that was deleted.</returns>
    [HttpDelete("{commentId}/comments/remove")]
    [Authorize(Policy = "Moderator")]
    public async Task<ActionResult<Comment>> DeleteComment(string commentId)
    {
        var deletedComment = await _gameService.DeleteComment(commentId);
        _logger.LogDebug("Successfully deleted comment.");

        return Ok(deletedComment);
    }
}
