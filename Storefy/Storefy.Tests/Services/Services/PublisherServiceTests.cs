using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Services.Services;

namespace Storefy.Tests.Services.Services;
public class PublisherServiceTests
{
    private readonly PublisherService _publisherService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public PublisherServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _publisherService = new PublisherService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddPublisher_AddsSuccess()
    {
        // Arrange
        var publisherDto = new CreateUpdatePublisherDto { Publisher = new PublisherDto { Id = Guid.NewGuid().ToString(), CompanyName = "TestCompany" } };
        _unitOfWorkMock.Setup(uow => uow.PublisherRepository.AddPublisher(publisherDto))
            .ReturnsAsync(new Publisher { Id = publisherDto.Publisher.Id, CompanyName = publisherDto.Publisher.CompanyName });

        // Act
        var newPublisher = await _publisherService.AddPublisher(publisherDto);

        // Assert
        Assert.NotNull(newPublisher);
        Assert.Equal(publisherDto.Publisher.CompanyName, newPublisher.CompanyName);
    }

    [Fact]
    public async Task GetAllPublishers_ReturnsAllPublishers()
    {
        // Arrange
        var publishers = new List<Publisher>
        {
            new() { Id = "1", CompanyName = "Company1" },
            new() { Id = "2", CompanyName = "Company2" },
            new() { Id = "3", CompanyName = "Company3" },
        };
        _unitOfWorkMock.Setup(uow => uow.PublisherRepository.GetAll())
            .ReturnsAsync(publishers);

        // Act
        var result = await _publisherService.GetAllPublishers();

        // Assert
        var returnedPublishers = publishers.Count;
        Assert.NotNull(result);
        Assert.Equal(returnedPublishers, result.Count());
    }

    [Fact]
    public async Task GetPublisherByGame_ReturnsPublisherWithGameAlias()
    {
        var gameAlias = "game1";
        var publisher = new Publisher { Id = "1", CompanyName = "Company1" };

        _unitOfWorkMock.Setup(uow => uow.PublisherRepository.GetPublisherByGame(gameAlias))
            .ReturnsAsync(publisher);

        var result = await _publisherService.GetPublisherByGame(gameAlias);

        Assert.NotNull(result);
        Assert.Equal(publisher, result);
    }

    [Fact]
    public async Task GetPublisherByGame_ReturnsPublisherFromNoSqlWhenNotFoundInSql()
    {
        // Arrange
        var gameAlias = "game1";

        _unitOfWorkMock
            .Setup(uow => uow.PublisherRepository.GetPublisherByGame(gameAlias))
            .ReturnsAsync(() => null);

        // Act
        var result = await _publisherService.GetPublisherByGame(gameAlias);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPublisherByGame_ReturnsNullPublisherNotExist()
    {
        // Arrange
        var gameAlias = "game1";
        _unitOfWorkMock
            .Setup(uow => uow.PublisherRepository.GetPublisherByGame(gameAlias))
            .ReturnsAsync(() => null);

        // Act
        var result = await _publisherService.GetPublisherByGame(gameAlias);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPublisherInfo_ReturnsPublisherWithSpecifiedCompanyName()
    {
        var companyName = "Company1";
        var publisher = new Publisher { Id = "1", CompanyName = companyName };

        _unitOfWorkMock.Setup(uow => uow.PublisherRepository.GetPublisherInformation(companyName))
            .ReturnsAsync(publisher);

        var result = await _publisherService.GetPublisherInfo(companyName);

        Assert.NotNull(result);
        Assert.Equal(publisher, result);
    }

    [Fact]
    public async Task GetPublisherInfo_ReturnsPublisherFromSupplierWhenNotFoundInPublisher()
    {
        // Arrange
        var companyName = "Company2";

        _unitOfWorkMock
            .Setup(uow => uow.PublisherRepository.GetPublisherInformation(companyName))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _publisherService.GetPublisherInfo(companyName);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdatePublisher_UpdatesThePublisherInformation()
    {
        var publisherDto = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = "1",
                CompanyName = "TestCompany",
            },
        };
        _unitOfWorkMock.Setup(uow => uow.PublisherRepository.UpdatePublisher(publisherDto))
            .ReturnsAsync(new Publisher { Id = publisherDto.Publisher.Id, CompanyName = publisherDto.Publisher.CompanyName });

        var updatedPublisher = await _publisherService.UpdatePublisher(publisherDto);

        Assert.NotNull(updatedPublisher);
        Assert.Equal(publisherDto.Publisher.CompanyName, updatedPublisher.CompanyName);
    }

    [Fact]
    public async Task UpdatePublisher_UpdatesSupplierAndAddsNewPublisher()
    {
        // Arrange
        var publisherDto = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = "2",
                CompanyName = "TestCompany2",
            },
        };

        var newPublisher = new Publisher { Id = publisherDto.Publisher.Id, CompanyName = publisherDto.Publisher.CompanyName };

        _unitOfWorkMock.Setup(uow => uow.PublisherRepository.UpdatePublisher(publisherDto)).ThrowsAsync(new Exception());

        // Act
        var result = await _publisherService.UpdatePublisher(publisherDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newPublisher, result);
    }

    [Fact]
    public async Task UpdatePublisher_ThrowsInvalidOperationException_WhenPublisherNotFoundInBothRepositories()
    {
        // Arrange
        var publisherDto = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = "3",
                CompanyName = "TestCompany3",
            },
        };

        _unitOfWorkMock
            .Setup(uow => uow.PublisherRepository.UpdatePublisher(publisherDto))
            .ThrowsAsync(new Exception());

        // Act
        var exception = await Record.ExceptionAsync(() => _publisherService.UpdatePublisher(publisherDto));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task DeletePublisher_DeletesPublisherWithSpecifiedId()
    {
        // Arrange
        var publisherId = "1";
        var publisher = new Publisher { Id = publisherId, CompanyName = "Company1" };

        var games = new List<Game>
        {
            new() { Id = "game1", Name = "Game 1", PublisherId = publisherId },
            new() { Id = "game2", Name = "Game 2", PublisherId = publisherId },
        };

        _unitOfWorkMock.Setup(uow => uow.PublisherRepository.GetPublisherById(publisherId))
            .ReturnsAsync(publisher);

        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetGamesByPublisher(publisher.CompanyName))
            .ReturnsAsync(games);

        _unitOfWorkMock.Setup(uow => uow.PublisherRepository.Delete(publisher))
            .ReturnsAsync(publisher);

        // Act
        var deletedPublisher = await _publisherService.DeletePublisher(publisherId);

        // Assert
        Assert.NotNull(deletedPublisher);
        Assert.Equal(publisher.Id, deletedPublisher.Id);
    }

    [Fact]
    public async Task GetPublisherById_ReturnsPublisherWithMatchingId()
    {
        var publisherId = "1";
        var publisher = new Publisher { Id = publisherId, CompanyName = "Company1" };
        _unitOfWorkMock.Setup(uow => uow.PublisherRepository.GetPublisherById(publisherId))
            .ReturnsAsync(publisher);

        var result = await _publisherService.GetPublisherById(publisherId);

        Assert.NotNull(result);
        Assert.Equal(publisherId, result.Id);
    }
}
