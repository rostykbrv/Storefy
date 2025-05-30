using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storefy.API.Controllers;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.Tests.Controller;
public class PublisherControllerTests
{
    private readonly PublisherController _controller;
    private readonly Mock<IPublisherService> _publisherService;
    private readonly Mock<ILogger<PublisherController>> _loggerMock;

    public PublisherControllerTests()
    {
        _loggerMock = new Mock<ILogger<PublisherController>>();
        _publisherService = new Mock<IPublisherService>();
        _controller = new PublisherController(_publisherService.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllPublishers_ReturnsAllPublishers()
    {
        // Arrange
        var publishers = new List<Publisher>
    {
        new()
        {
            Id = "1",
            CompanyName = "Publisher1",
            Description = "Description1",
            HomePage = "HomePage1",
            Games = new List<Game>
            {
                new() { Id = "1", Name = "TestGame", Key = "testgame" },
            },
        },
        new()
        {
            Id = "2",
            CompanyName = "Publisher2",
            Description = "Description2",
            HomePage = "HomePage2",
            Games = new List<Game>
            {
                new() { Id = "2", Name = "TestGame2", Key = "testgame2" },
            },
        },
    };

        _publisherService.Setup(s => s.GetAllPublishers()).ReturnsAsync(publishers);

        // Act
        var result = await _controller.GetAllPublishers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPublishers = Assert.IsType<List<Publisher>>(okResult.Value);
        Assert.Equal(publishers.Count, returnedPublishers.Count);
    }

    [Fact]
    public async Task GetPublisherByGameKey_ValidGameAlias_ReturnsPublisher()
    {
        // Arrange
        var key = "game1";
        var expectedPublisher = new Publisher
        {
            Id = "123",
            CompanyName = "Test Company",
            Description = "Test Description",
            HomePage = "www.test.com",
            Games = new List<Game>
            {
                new() { Id = "1", Name = "TestGame", Key = "testgame" },
            },
        };

        _publisherService.Setup(s => s.GetPublisherByGame(key)).ReturnsAsync(expectedPublisher);

        // Act
        var result = await _controller.GetPublisherByGameKey(key);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPublisher = Assert.IsType<Publisher>(okResult.Value);
        Assert.Equal(expectedPublisher.Id, returnedPublisher.Id);
        Assert.Equal(expectedPublisher.CompanyName, returnedPublisher.CompanyName);
        Assert.Equal(expectedPublisher.Description, returnedPublisher.Description);
        Assert.Equal(expectedPublisher.HomePage, returnedPublisher.HomePage);
        Assert.Equal(expectedPublisher.Games.Count, returnedPublisher.Games.Count);
    }

    [Fact]
    public async Task CreatePublisher_ValidData_ReturnsCreatedPublisher()
    {
        // Arrange
        var testPublisher = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = "123",
                CompanyName = "Test",
                Description = "Test Description",
                HomePage = "www.test.com",
            },
        };

        var expectedPublisher = new Publisher
        {
            Id = testPublisher.Publisher.Id,
            CompanyName = testPublisher.Publisher.CompanyName,
            Description = testPublisher.Publisher.Description,
            HomePage = testPublisher.Publisher.HomePage,
        };

        _publisherService.Setup(s => s.AddPublisher(testPublisher)).ReturnsAsync(expectedPublisher);

        // Act
        var result = await _controller.CreatePublisher(testPublisher);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedPublisher = Assert.IsType<Publisher>(createdAtResult.Value);
        Assert.Equal(expectedPublisher.Id, returnedPublisher.Id);
        Assert.Equal(expectedPublisher.CompanyName, returnedPublisher.CompanyName);
        Assert.Equal(expectedPublisher.Description, returnedPublisher.Description);
        Assert.Equal(expectedPublisher.HomePage, returnedPublisher.HomePage);
    }

    [Fact]
    public async Task CreatePublisher_BadModel_ReturnsBadRequest()
    {
        // Arrange
        var testPublisher = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = "123",
                Description = "Test Description",
                HomePage = "www.test.com",
            },
        };

        _controller.ModelState.AddModelError("CompanyName", "Required");

        // Act
        var result = await _controller.CreatePublisher(testPublisher);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPublisherInformation_ValidCompanyName_ReturnsPublisher()
    {
        // Arrange
        var companyName = "Test";
        var expectedPublisher = new Publisher
        {
            Id = "123",
            CompanyName = companyName,
            Description = "Test Description",
            HomePage = "www.test.com",
            Games = new List<Game>
            {
                new() { Id = "1", Name = "Game", Key = "game", PublisherId = "123" },
            },
        };

        _publisherService.Setup(s => s.GetPublisherInfo(companyName)).ReturnsAsync(expectedPublisher);

        // Act
        var result = await _controller.GetPublisherInformation(companyName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPublisher = Assert.IsType<Publisher>(okResult.Value);
        Assert.Equal(expectedPublisher.Id, returnedPublisher.Id);
        Assert.Equal(expectedPublisher.CompanyName, returnedPublisher.CompanyName);
        Assert.Equal(expectedPublisher.Description, returnedPublisher.Description);
        Assert.Equal(expectedPublisher.HomePage, returnedPublisher.HomePage);
        Assert.Equal(expectedPublisher.Games.Count, returnedPublisher.Games.Count);
    }

    [Fact]
    public async Task GetPublisherInformation_NoPublisherFound_ReturnsNotFound()
    {
        // Arrange
        var companyName = "Test";
        _publisherService.Setup(s => s.GetPublisherInfo(companyName)).Returns(Task.FromResult<Publisher>(null));

        // Act
        var result = await _controller.GetPublisherInformation(companyName);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdatePublisherInformation_ValidData_ReturnsUpdatedPublisher()
    {
        // Arrange
        var testPublisher = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = "123",
                CompanyName = "Updated",
                Description = "Updated Description",
                HomePage = "www.updated.com",
            },
        };

        var expectedPublisher = new Publisher
        {
            Id = testPublisher.Publisher.Id,
            CompanyName = testPublisher.Publisher.CompanyName,
            Description = testPublisher.Publisher.Description,
            HomePage = testPublisher.Publisher.HomePage,
        };

        _publisherService.Setup(s => s.UpdatePublisher(testPublisher)).ReturnsAsync(expectedPublisher);

        // Act
        var result = await _controller.UpdatePublisherInformation(testPublisher);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPublisher = Assert.IsType<Publisher>(okResult.Value);
        Assert.Equal(expectedPublisher.Id, returnedPublisher.Id);
        Assert.Equal(expectedPublisher.CompanyName, returnedPublisher.CompanyName);
        Assert.Equal(expectedPublisher.Description, returnedPublisher.Description);
        Assert.Equal(expectedPublisher.HomePage, returnedPublisher.HomePage);
    }

    [Fact]
    public async Task UpdatePublisherInformation_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var testPublisher = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = "123",
                Description = "Updated Description",
                HomePage = "www.updated.com",
            },
        };

        // Make the model state invalid
        _controller.ModelState.AddModelError("CompanyName", "Required");

        // Act
        var result = await _controller.UpdatePublisherInformation(testPublisher);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeletePublisher_ValidId_ReturnsDeletedPublisher()
    {
        // Arrange
        var testId = "123";
        var expectedPublisher = new Publisher
        {
            Id = testId,
            CompanyName = "Test",
            Description = "Test Description",
            HomePage = "www.test.com",
            Games = new List<Game>
            {
                new() { Id = "1", Name = "Test Game", Key = "test-game" },
            },
        };

        _publisherService.Setup(s => s.DeletePublisher(testId)).ReturnsAsync(expectedPublisher);

        // Act
        var result = await _controller.DeletePublisher(testId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPublisher = Assert.IsType<Publisher>(okResult.Value);

        Assert.Equal(expectedPublisher.Id, returnedPublisher.Id);
        Assert.Equal(expectedPublisher.CompanyName, returnedPublisher.CompanyName);
        Assert.Equal(expectedPublisher.Description, returnedPublisher.Description);
        Assert.Equal(expectedPublisher.HomePage, returnedPublisher.HomePage);
        Assert.Equal(expectedPublisher.Games.Count, returnedPublisher.Games.Count);
    }
}
