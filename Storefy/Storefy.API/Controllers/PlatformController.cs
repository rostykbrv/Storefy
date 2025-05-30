using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.API.Controllers;

/// <summary>
/// Represents the Platform API Controller class.
/// </summary>
[Route("platforms/")]
[ApiController]
[Authorize(Policy = "Manager")]
public class PlatformController : ControllerBase
{
    private readonly IPlatformService _platformService;
    private readonly ILogger<PlatformController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformController"/> class.
    /// </summary>
    /// <param name="platformService">Service to perform operations on Platform.</param>
    /// <param name="logger">Logger service for logging.</param>
    public PlatformController(IPlatformService platformService, ILogger<PlatformController> logger)
    {
        _platformService = platformService;
        _logger = logger;
    }

    /// <summary>
    /// HTTP Post method to add a new platform.
    /// </summary>
    /// <param name="platform">Data Transfer Object of the Platform to be added.</param>
    /// <returns>If successful, it returns the added platform, else it returns an error.</returns>
    [HttpPost("new")]
    public async Task<ActionResult<Platform>> CreatePlatform(CreateUpdatePlatformDto platform)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Failed to create new platform due to bad request");

            return BadRequest(ModelState);
        }

        var createdPlatform = await _platformService.AddPlatform(platform);
        _logger.LogDebug($"Successfully created a new platform with id {createdPlatform.Id}");

        return CreatedAtAction(nameof(CreatePlatform), createdPlatform);
    }

    /// <summary>
    /// HTTP Get method to fetch a specific platform by its ID.
    /// </summary>
    /// <param name="platformId">The unique identifier of the platform.</param>
    /// <returns>If successful, it returns the requested platform, else it returns an error.</returns>
    [HttpGet("{platformId}")]
    [AllowAnonymous]
    public async Task<ActionResult<Platform>> GetPlatform(string platformId)
    {
        var platform = await _platformService.GetPlatformById(platformId);

        if (platform == null)
        {
            _logger.LogWarning($"Platform  '{platformId}' not found.");

            return NotFound();
        }
        else
        {
            _logger.LogDebug($"Successfully fetched platform with id {platformId}");

            return Ok(platform);
        }
    }

    /// <summary>
    /// HTTP Put method to update a specific platform.
    /// </summary>
    /// <param name="platformDto">Data Transfer Object of the Platform to be updated.</param>
    /// <returns>If successful, it returns the updated platform, else it returns an error.</returns>
    [HttpPut("update")]
    public async Task<ActionResult<Platform>> UpdatePlatform(CreateUpdatePlatformDto platformDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Failed to create new platform due to bad request");

            return BadRequest(ModelState);
        }

        var updatedPlatform = await _platformService.UpdatePlatform(platformDto);
        _logger.LogDebug($"Successfully updated the platform with id {platformDto.Platform.Id}");

        return CreatedAtAction(nameof(UpdatePlatform), updatedPlatform);
    }

    /// <summary>
    /// HTTP Delete method to delete a specific platform.
    /// </summary>
    /// <param name="id">The unique identifier of the platform to be deleted.</param>
    /// <returns>If successful, it returns the deleted platform, else it returns an error.</returns>
    [HttpDelete("remove/{id}")]
    public async Task<ActionResult<Platform>> DeletePlatform(string id)
    {
        var platform = await _platformService.DeletePlatform(id);

        if (platform == null)
        {
            _logger.LogWarning($"Platform with id {id} was not found");

            return NotFound();
        }
        else
        {
            _logger.LogDebug($"Successfully deleted Platform with id {id}");

            return Ok(platform);
        }
    }

    /// <summary>
    /// HTTP Get method to fetch all platforms.
    /// </summary>
    /// <returns>If successful, it returns all platforms, else it returns an error.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<Platform>> GetAllPlatforms()
    {
        var platforms = await _platformService.GetAllPlatforms();
        _logger.LogDebug($"Successfully retrieved all platforms");

        return Ok(platforms);
    }

    /// <summary>
    /// HTTP GET method to retrieve platforms associated with a particular game specified by its alias.
    /// </summary>
    /// <param name="gamealias">The unique alias of the game.</param>
    /// <returns>
    /// HTTP response with the list of platforms that are associated with the specified game if successful.
    /// </returns>
    [HttpGet("games/{gamealias}/{languageCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Genre>>> GetPlatformsByGame(string gamealias, string languageCode)
    {
        var platforms = await _platformService.GetPlatformsByGame(gamealias, languageCode);

        if (platforms == null)
        {
            _logger.LogWarning($"Game's platform '{gamealias}' not found.");

            return NotFound();
        }
        else
        {
            _logger.LogDebug($"Successfully returned game's - {gamealias} platforms");

            return Ok(platforms);
        }
    }
}
