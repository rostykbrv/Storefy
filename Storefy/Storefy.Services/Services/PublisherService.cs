using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;

namespace Storefy.Services.Services;

/// <summary>
/// Implementation of the services needed to manipulate and
/// retrieve information for Publisher entities in the repository.
/// </summary>
/// <inheritdoc cref="IPublisherService"/>
public class PublisherService : IPublisherService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to be used by the publisher repository.</param>
    public PublisherService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Publisher> AddPublisher(CreateUpdatePublisherDto publisher)
    {
        var newPublisher = await _unitOfWork.PublisherRepository
                .AddPublisher(publisher);

        return newPublisher;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Publisher>> GetAllPublishers()
    {
        var publishersSql = await _unitOfWork.PublisherRepository
            .GetAll();
        var returnedPublishers = new List<Publisher>();
        returnedPublishers.AddRange(publishersSql);

        return returnedPublishers;
    }

    /// <inheritdoc />
    public async Task<Publisher> GetPublisherByGame(string gameAlias)
    {
        var returnedPublisher = await _unitOfWork.PublisherRepository.GetPublisherByGame(gameAlias);

        return returnedPublisher;
    }

    /// <inheritdoc />
    public async Task<Publisher> GetPublisherInfo(string companyName)
    {
        var returnedPublisher = await _unitOfWork.PublisherRepository.GetPublisherInformation(companyName);

        return returnedPublisher;
    }

    /// <inheritdoc />
    public async Task<Publisher> UpdatePublisher(CreateUpdatePublisherDto publisher)
    {
        try
        {
            var updatedPublisher = await _unitOfWork.PublisherRepository.UpdatePublisher(publisher);

            return updatedPublisher;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        throw new InvalidOperationException("Cannot update selected publisher.");
    }

    /// <inheritdoc />
    public async Task<Publisher> DeletePublisher(string id)
    {
        var deletedPublisher = await _unitOfWork.PublisherRepository
            .GetPublisherById(id);
        var publishersGames = await _unitOfWork.GameRepository
            .GetGamesByPublisher(deletedPublisher.CompanyName);

        foreach (var game in publishersGames)
        {
            game.PublisherId = null;
        }

        var result = await _unitOfWork.PublisherRepository.Delete(deletedPublisher);

        return result;
    }

    /// <inheritdoc />
    public async Task<Publisher> GetPublisherById(string publisherId)
    {
        var selectedPublisher = await _unitOfWork.PublisherRepository
            .GetPublisherById(publisherId);

        return selectedPublisher;
    }
}