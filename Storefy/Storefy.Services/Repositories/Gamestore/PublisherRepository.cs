using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Repositories.Gamestore;
using Storefy.Services.Data;

namespace Storefy.Services.Repositories.Gamestore;

/// <summary>
/// Repository to manage the operations of Publisher entities using a database context.
/// </summary>
/// <inheritdoc cref="IPublisherRepository{T}"/>
public class PublisherRepository : IPublisherRepository<Publisher>
{
    private readonly StorefyDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The data base context
    /// to be used by the repository.</param>
    public PublisherRepository(StorefyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Publisher> AddPublisher(CreateUpdatePublisherDto publisher)
    {
        var publisherExist = await _dbContext.Publishers
            .AnyAsync(p => p.CompanyName == publisher.Publisher.CompanyName);

        if (!publisherExist)
        {
            var createdPublisher = new Publisher
            {
                Id = Guid.NewGuid().ToString(),
                CompanyName = publisher.Publisher.CompanyName,
                Description = publisher.Publisher.Description,
                HomePage = publisher.Publisher.HomePage,
            };

            await _dbContext.AddAsync(createdPublisher);
            await _dbContext.SaveChangesAsync();

            return createdPublisher;
        }

        throw new InvalidOperationException("This publisher already exists!");
    }

    /// <inheritdoc />
    public async Task<Publisher> Delete(Publisher entity)
    {
        _dbContext.Publishers.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Publisher>> GetAll()
    {
        return await _dbContext.Publishers.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Publisher> GetPublisherByGame(string gameAlias)
    {
        var selectedPublisher = await _dbContext.Games
            .Where(g => g.Key.Equals(gameAlias))
            .Select(g => g.Publisher)
            .FirstOrDefaultAsync();

        return selectedPublisher;
    }

    /// <inheritdoc />
    public async Task<Publisher> GetPublisherInformation(string companyName)
    {
        var selectedPublisher = await _dbContext.Publishers
            .Where(p => p.CompanyName.Equals(companyName))
            .FirstOrDefaultAsync();

        return selectedPublisher ??
            throw new InvalidOperationException("This publisher doesn't exist");
    }

    /// <inheritdoc />
    public async Task<Publisher> UpdatePublisher(CreateUpdatePublisherDto publisher)
    {
        var updatedPublisher = await _dbContext.Publishers
            .FindAsync(publisher.Publisher.Id);

        if (updatedPublisher != null)
        {
            var outdatedPublisher = new Publisher
            {
                Id = updatedPublisher.Id,
                SupplierId = updatedPublisher.SupplierId,
                Address = updatedPublisher.Address,
                City = updatedPublisher.City,
                CompanyName = updatedPublisher.CompanyName,
                ContactName = updatedPublisher.ContactName,
                ContactTitle = updatedPublisher.ContactTitle,
                Country = updatedPublisher.Country,
                Description = updatedPublisher.Description,
                Fax = updatedPublisher.Fax,
                Games = updatedPublisher.Games,
                HomePage = updatedPublisher.HomePage,
                Phone = updatedPublisher.Phone,
                Region = updatedPublisher.Region,
            };
            updatedPublisher.CompanyName = publisher.Publisher.CompanyName;
            updatedPublisher.HomePage = publisher.Publisher.HomePage;
            updatedPublisher.Description = publisher.Publisher.Description;
            await _dbContext.SaveChangesAsync();
            Console.WriteLine(outdatedPublisher);
            return updatedPublisher;
        }

        throw new InvalidOperationException("This publisher doesn't exist!");
    }

    /// <inheritdoc />
    public async Task<Publisher> GetPublisherById(string id)
    {
        var publisher = await _dbContext.Publishers.FirstOrDefaultAsync(p => p.Id == id);

        return publisher ??
            throw new InvalidOperationException("This publisher doesn't exist!");
    }
}
