using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.API.Controllers;

/// <summary>
/// Represents the Publisher API Controller class.
/// </summary>
[Route("publishers/")]
[ApiController]
[Authorize(Policy = "Manager")]
public class PublisherController : ControllerBase
{
    private readonly IPublisherService _publisherService;
    private readonly ILogger<PublisherController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherController"/> class.
    /// </summary>
    /// <param name="publisherService">Service to perform operations on Publisher.</param>
    /// <param name="logger">Logger service for logging.</param>
    public PublisherController(IPublisherService publisherService, ILogger<PublisherController> logger)
    {
        _publisherService = publisherService;
        _logger = logger;
    }

    /// <summary>
    /// HTTP Get method to retrieve all publishers.
    /// </summary>
    /// <returns>If successful, it returns all publishers, else it returns an error.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<Publisher>> GetAllPublishers()
    {
        var publishers = await _publisherService.GetAllPublishers();
        _logger.LogDebug($"Successfully returned all publishers.");

        return Ok(publishers);
    }

    /// <summary>
    /// HTTP GET method to retrieve publisher associated with a particular game specified by its alias.
    /// </summary>
    /// <param name="gamealias">The unique alias of the game.</param>
    /// <returns>
    /// HTTP response with the list of publishers that are associated with the specified game if successful.
    /// </returns>
    [HttpGet("games/{gamealias}")]
    [AllowAnonymous]
    public async Task<ActionResult<Publisher>> GetPublisherByGameKey(string gamealias)
    {
        var publisher = await _publisherService.GetPublisherByGame(gamealias);
        _logger.LogDebug($"Successfully returned publisher of game - {gamealias}");

        return Ok(publisher);
    }

    /// <summary>
    /// HTTP post method to create a new publisher.
    /// </summary>
    /// <param name="publisher">Data Transfer Object of the Publisher that is to be created.</param>
    /// <returns>If successful, it returns the created publisher, else it returns an error.</returns>
    [HttpPost("new")]
    public async Task<ActionResult<Publisher>> CreatePublisher(CreateUpdatePublisherDto publisher)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newPublisher = await _publisherService.AddPublisher(publisher);
        _logger.LogDebug($"Successfully created publisher - {publisher.Publisher.CompanyName}");

        return CreatedAtAction(nameof(CreatePublisher), newPublisher);
    }

    /// <summary>
    /// HTTP Get method to retrieve a specific publisher by its company name.
    /// </summary>
    /// <param name="companyName">The unique identifier of the publisher.</param>
    /// <returns>If successful, it returns the requested publisher, else it returns an error.</returns>
    [HttpGet("{companyName}")]
    [AllowAnonymous]
    public async Task<ActionResult<Publisher>> GetPublisherInformation(string companyName)
    {
        var returnedPublisher = await _publisherService.GetPublisherInfo(companyName);

        if (returnedPublisher == null)
        {
            _logger.LogWarning($"Publisher '{companyName}' not found.");

            return NotFound();
        }
        else
        {
            _logger.LogDebug($"Successfully returned publisher {companyName}");

            return Ok(returnedPublisher);
        }
    }

    /// <summary>
    /// HTTP Put method to update a specific publisher.
    /// </summary>
    /// <param name="publisher">Data Transfer Object containing the new publisher detail to be updated.</param>
    /// <returns>The updated publisher if successful, else it returns an error.</returns>
    [HttpPut("update")]
    public async Task<ActionResult<Publisher>> UpdatePublisherInformation(CreateUpdatePublisherDto publisher)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedPublisher = await _publisherService.UpdatePublisher(publisher);
        _logger.LogDebug($"Successfully updated publisher");

        return Ok(updatedPublisher);
    }

    /// <summary>
    /// HTTP Delete method to delete a specific publisher.
    /// </summary>
    /// <param name="id">The unique identifier of the publisher to delete.</param>
    /// <returns>No content if successful, else an error.</returns>
    [HttpDelete("remove/{id}")]
    public async Task<ActionResult<Publisher>> DeletePublisher(string id)
    {
        var publisher = await _publisherService.DeletePublisher(id);
        _logger.LogDebug($"Successfully deleted publisher - {id}");

        return Ok(publisher);
    }
}
