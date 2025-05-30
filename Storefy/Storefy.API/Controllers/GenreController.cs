using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.API.Controllers;

/// <summary>
/// Represents the Genre API Controller class.
/// </summary>
[Route("genres/")]
[ApiController]
[Authorize(Policy = "Manager")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly ILogger<GenreController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreController"/> class.
    /// </summary>
    /// <param name="genreService">Service to perform operations on Genre.</param>
    /// <param name="logger">Logger service for logging.</param>
    public GenreController(IGenreService genreService, ILogger<GenreController> logger)
    {
        _genreService = genreService;
        _logger = logger;
    }

    /// <summary>
    /// HTTP post method to create new genre.
    /// </summary>
    /// <param name="genre">Data Transfer Object of the Genre that is to be created.</param>
    /// <returns>If successful, it returns the created genre, else it returns an error.</returns>
    [HttpPost("new")]
    public async Task<ActionResult<Genre>> CreateGenre(CreateUpdateGenreDto genre)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Failed to create genre due to bad request");

            return BadRequest(ModelState);
        }

        var createdGenre = await _genreService.AddGenre(genre);

        if (createdGenre != null)
        {
            _logger.LogDebug($"Successfully created a new genre with id {createdGenre.Id}");
        }

        return CreatedAtAction(nameof(CreateGenre), createdGenre);
    }

    /// <summary>
    /// HTTP Get method to retrieve a specific genre by its ID.
    /// </summary>
    /// <param name="genreId">The unique identifier of the genre.</param>
    /// <returns>If successful, it returns the requested genre, else it returns an error.</returns>
    [HttpGet("{genreId}")]
    [AllowAnonymous]
    public async Task<ActionResult<Genre>> GetGenre(string genreId)
    {
        var genre = await _genreService.GetGenreById(genreId);

        if (genre == null)
        {
            _logger.LogWarning($"Genre with id {genreId} was not found");

            return NotFound();
        }
        else
        {
            _logger.LogDebug($"Successfully retrieved Genre with id {genreId}");

            return Ok(genre);
        }
    }

    /// <summary>
    /// HTTP GET method to retrieve genres based on their parent genre's ID.
    /// </summary>
    /// <param name="parentGenreId">The unique identifier of the parent genre.</param>
    /// <returns>
    /// HTTP response with the list of genres that have the specified parent genre if successful.
    /// </returns>
    [HttpGet("parentGenre/{parentGenreId}")]
    [AllowAnonymous]
    public async Task<ActionResult<Genre>> GetGenresByParentGenre(string parentGenreId)
    {
        var subgenres = await _genreService.GetGenresByParentGenre(parentGenreId);

        if (subgenres == null)
        {
            _logger.LogWarning($"Genre neseted by id {parentGenreId} was not found");

            return NotFound();
        }
        else
        {
            _logger.LogDebug($"Successfully retrieved Genres with of parent genre {parentGenreId}");

            return Ok(subgenres);
        }
    }

    /// <summary>
    /// HTTP Put method to update a specific genre.
    /// </summary>
    /// <param name="genreDto">Data Transfer Object containing the new genre detail.</param>
    /// <returns>The updated genre if successful, else it returns an error.</returns>
    [HttpPut("update")]
    public async Task<ActionResult<Genre>> UpdateGenre(CreateUpdateGenreDto genreDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning($"Failed to update genre due to bad request, Name: {genreDto.Genre.Name}");

            return BadRequest(ModelState);
        }

        var updatedGenre = await _genreService.UpdateGenre(genreDto);
        _logger.LogDebug($"Successfully updated the genre with name {genreDto.Genre.Name}");

        return CreatedAtAction(nameof(UpdateGenre), updatedGenre);
    }

    /// <summary>
    /// HTTP Delete method to delete a specific genre.
    /// </summary>
    /// <param name="id">The unique identifier of the genre to delete.</param>
    /// <returns>No content if successful, else an error.</returns>
    [HttpDelete("remove/{id}")]
    public async Task<ActionResult<Genre>> DeleteGenre(string id)
    {
        var genre = await _genreService.DeleteGenre(id);

        if (genre == null)
        {
            _logger.LogWarning($"Genre with id {id} was not found");

            return NotFound();
        }
        else
        {
            _logger.LogDebug($"Successfully deleted Genre with id {id}");

            return Ok(genre);
        }
    }

    /// <summary>
    /// HTTP Get request to retrieve all genres.
    /// </summary>
    /// <returns>If successful, it returns all genres, else it returns an error.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<Genre>> GetAllGenres()
    {
        var genres = await _genreService.GetAllGenres();
        _logger.LogDebug($"Successfully retrieved all genres");

        return Ok(genres);
    }

    /// <summary>
    /// HTTP GET method to retrieve genres associated with a particular game specified by its alias.
    /// </summary>
    /// <param name="gamealias">The unique alias of the game.</param>
    /// <returns>
    /// HTTP response with the list of genres that are associated with the specified game if successful.
    /// </returns>
    [HttpGet("games/{gamealias}/{languageCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenresByGame(string gamealias, string languageCode)
    {
        var genres = await _genreService.GetGenresByGameAlias(gamealias, languageCode);

        _logger.LogDebug($"Successfully retrieved Genres of game - {gamealias}");

        return Ok(genres);
    }
}
