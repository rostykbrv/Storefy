using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Services;

/// <summary>
/// Interface for the Publisher Service.
/// Defines operations for managing publishers.
/// </summary>
public interface IPublisherService
{
    /// <summary>
    /// Retrieves all existing publisher entities.
    /// </summary>
    /// <returns>The asynchronous operation in which task result contains
    /// a collection of all publishers.</returns>
    Task<IEnumerable<Publisher>> GetAllPublishers();

    /// <summary>
    /// Asynchronously retrieves a publisher by game alias.
    /// </summary>
    /// <param name="gameAlias">The alias of the game.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the Publisher object for the specified game.</returns>
    Task<Publisher> GetPublisherByGame(string gameAlias);

    /// <summary>
    /// Asynchronously adds a new publisher based on the information
    /// in the suppliedCreateUpdatePublisherDto.
    /// </summary>
    /// <param name="publisher">A CreateUpdatePublisherDto object containing the
    /// details of the new publisher to be added.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the newly created Publisher object.</returns>
    Task<Publisher> AddPublisher(CreateUpdatePublisherDto publisher);

    /// <summary>
    /// Asynchronously retrieves a publisher information based on the provided company name.
    /// </summary>
    /// <param name="companyName">The name of the company.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the Publisher with the provided company name.</returns>
    Task<Publisher> GetPublisherInfo(string companyName);

    /// <summary>
    /// Asynchronously updates an existing publisher with the information
    /// in the supplied CreateUpdatePublisherDto.
    /// </summary>
    /// <param name="publisher">A CreateUpdatePublisherDto object containing
    /// the updated details of the publisher.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the updated Publisher object.</returns>
    Task<Publisher> UpdatePublisher(CreateUpdatePublisherDto publisher);

    /// <summary>
    /// Asynchronously retrieves a publisher by its unique identifier.
    /// </summary>
    /// <param name="publisherId">The unique identifier of the publisher.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the Publisher object with the specified publisherId.</returns>
    Task<Publisher> GetPublisherById(string publisherId);

    /// <summary>
    /// Asynchronously deletes a publisher using its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the publisher.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the Publisher object that was deleted.</returns>
    Task<Publisher> DeletePublisher(string id);
}
