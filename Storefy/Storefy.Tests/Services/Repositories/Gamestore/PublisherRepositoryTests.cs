using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Services.Data;
using Storefy.Services.Repositories.Gamestore;

namespace Storefy.Tests.Services.Repositories.Gamestore;
public class PublisherRepositoryTests : IDisposable
{
    private readonly PublisherRepository _publisherRepository;
    private readonly StorefyDbContext _dbContext;
    private bool _disposed;

    public PublisherRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StorefyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StorefyDbContext(options);
        _publisherRepository = new PublisherRepository(_dbContext);
    }

    [Fact]
    public async Task AddPublisher_NewPublisher_ReturnsPublisher()
    {
        // Arrange
        var publisherDto = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = Guid.NewGuid().ToString(),
                CompanyName = "New Publisher",
                Description = "New Publisher Description",
                HomePage = "homepage.com",
            },
        };

        // Act
        var result = await _publisherRepository.AddPublisher(publisherDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(publisherDto.Publisher.CompanyName, result.CompanyName);
        Assert.Equal(publisherDto.Publisher.Description, result.Description);
        Assert.Equal(publisherDto.Publisher.HomePage, result.HomePage);
    }

    [Fact]
    public async Task AddPublisher_PublisherAlreadyExistsAsPlatform_ThrowsInvalidOperationException()
    {
        // Arrange
        var existedPublisher = new Publisher
        {
            Id = "123",
            CompanyName = "ExistedPublisher",
        };

        await _dbContext.Publishers.AddAsync(existedPublisher);
        await _dbContext.SaveChangesAsync();

        var publisherDto = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                CompanyName = existedPublisher.CompanyName,
            },
        };

        // Act
        var exception = await Assert
            .ThrowsAsync<InvalidOperationException>(() => _publisherRepository.AddPublisher(publisherDto));

        // Assert
        Assert.Equal("This publisher already exists!", exception.Message);
    }

    [Fact]
    public async Task DeletePublisher_ValidData()
    {
        // Arrange
        var publisher = new Publisher
        {
            Id = "123",
            CompanyName = "TestPublisher",
            Description = "Description",
            HomePage = "HomePage",
        };
        _dbContext.Publishers.Add(publisher);
        await _dbContext.SaveChangesAsync();

        // Act
        var removedPlatform = await _publisherRepository.Delete(publisher);

        // Assert
        var allPublishers = await _dbContext.Publishers.ToListAsync();
        Assert.DoesNotContain(removedPlatform, allPublishers);
    }

    [Fact]
    public async Task GetAllPublishers_ReturnAllPublishers()
    {
        // Arrange
        var publisher1 = new Publisher
        {
            Id = "123",
            CompanyName = "TestPublisher",
            Description = "Description",
            HomePage = "HomePage",
        };
        var publisher2 = new Publisher
        {
            Id = "124",
            CompanyName = "TestPublisher2",
            Description = "Description2",
            HomePage = "HomePage2",
        };
        _dbContext.Publishers.AddRange(publisher1, publisher2);
        await _dbContext.SaveChangesAsync();

        // Act
        var platforms = await _publisherRepository.GetAll();

        // Assert
        Assert.Equal(2, platforms.Count());
    }

    [Fact]
    public async Task GetPublisherByGame_GameExists_ReturnsPublisherOfGame()
    {
        // Arrange
        var gameAlias = "game1";
        var publisher = new Publisher
        {
            Id = "123",
            CompanyName = "TestPublisher",
            Description = "Description",
            HomePage = "HomePage",
        };
        var game = new Game { Id = "1", Name = "Game1", Key = gameAlias, Publisher = publisher };
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _publisherRepository.GetPublisherByGame(gameAlias);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(publisher.CompanyName, result.CompanyName);
    }

    [Fact]
    public async Task GetPublisherByGame_GameDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistingGameAlias = "nonExistingGame";

        // Act
        var result = await _publisherRepository.GetPublisherByGame(nonExistingGameAlias);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPublisherInformation_WhenPublisherExists_ReturnsPublisher()
    {
        // Arrange
        var publisher = new Publisher
        {
            Id = "123",
            CompanyName = "TestPublisher",
            Description = "Description",
            HomePage = "HomePage",
        };
        _dbContext.Publishers.Add(publisher);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _publisherRepository.GetPublisherInformation(publisher.CompanyName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(publisher.CompanyName, result.CompanyName);
    }

    [Fact]
    public async Task GetPublisherInformation_WhenPublisherDoesNotExist_ThrowsException()
    {
        // Arrange
        var nonExistingCompanyName = "NonExistingPublisher";

        // Act
        var exception = await Assert
            .ThrowsAsync<InvalidOperationException>(() => _publisherRepository.GetPublisherInformation(nonExistingCompanyName));

        // Assert
        Assert.Equal("This publisher doesn't exist", exception.Message);
    }

    [Fact]
    public async Task UpdatePublisher_WhenPublisherExists_UpdatesAndReturnsPublisher()
    {
        // Arrange
        var publisherId = Guid.NewGuid().ToString();
        var publisher = new Publisher
        {
            Id = publisherId,
            CompanyName = "TestPublisher",
            Description = "Description",
            HomePage = "HomePage",
        };
        _dbContext.Publishers.Add(publisher);
        await _dbContext.SaveChangesAsync();

        var updatePublisherDto = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = publisherId,
                CompanyName = "Updated Publisher",
                HomePage = "http://updatedpublisher.com",
                Description = "Updated Description",
            },
        };

        // Act
        var updatedPublisher = await _publisherRepository.UpdatePublisher(updatePublisherDto);

        // Assert
        Assert.NotNull(updatedPublisher);
        Assert.Equal(updatePublisherDto.Publisher.CompanyName, updatedPublisher.CompanyName);
        Assert.Equal(updatePublisherDto.Publisher.HomePage, updatedPublisher.HomePage);
        Assert.Equal(updatePublisherDto.Publisher.Description, updatedPublisher.Description);
    }

    [Fact]
    public async Task UpdatePublisher_WhenPublisherDoesNotExist_ReturnsNull()
    {
        // Arrange
        var updatePublisherDto = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = Guid.NewGuid().ToString(),
                CompanyName = "NonExistingPublisher",
                HomePage = "http://nonexistingpublisher.com",
                Description = "Non existing description",
            },
        };

        // Act
        var exception = await Assert
            .ThrowsAsync<InvalidOperationException>(() => _publisherRepository.UpdatePublisher(updatePublisherDto));

        // Assert
        Assert.Equal("This publisher doesn't exist!", exception.Message);
    }

    [Fact]
    public async Task GetPublisherById_PublisherExists_ReturnsPublisher()
    {
        // Arrange
        var publisherId = Guid.NewGuid().ToString();
        var publisher = new Publisher
        {
            Id = publisherId,
            CompanyName = "TestPublisher",
            Description = "Description",
            HomePage = "HomePage",
        };
        _dbContext.Publishers.Add(publisher);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _publisherRepository.GetPublisherById(publisherId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(publisherId, result.Id);
        Assert.Equal(publisher.CompanyName, result.CompanyName);
    }

    [Fact]
    public async Task GetPublisherById_PublisherDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistingPublisherId = Guid.NewGuid().ToString();

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _publisherRepository.GetPublisherById(nonExistingPublisherId));

        // Assert
        Assert.Equal("This publisher doesn't exist!", exception.Message);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }
    }
}
