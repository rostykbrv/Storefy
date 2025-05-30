using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storefy.API.Controllers;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.Tests.Controller;
public class PlatformControllerTests
{
    private readonly PlatformController _controller;
    private readonly Mock<IPlatformService> _platformService;
    private readonly Mock<ILogger<PlatformController>> _logger;

    public PlatformControllerTests()
    {
        _logger = new Mock<ILogger<PlatformController>>();
        _platformService = new Mock<IPlatformService>();
        _controller = new PlatformController(_platformService.Object, _logger.Object);
    }

    [Fact]
    public async Task CreatePlatform_ValidData_ReturnsCreatedPlatform()
    {
        // Arrange
        var testPlatform = new CreateUpdatePlatformDto
        {
            Platform = new PlatformDto
            {
                Id = "123",
                Type = "PC",
            },
        };

        var expectedPlatform = new Platform
        {
            Id = testPlatform.Platform.Id,
            Type = testPlatform.Platform.Type,
        };

        _platformService.Setup(s => s.AddPlatform(testPlatform)).ReturnsAsync(expectedPlatform);

        // Act
        var result = await _controller.CreatePlatform(testPlatform);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedPlatform = Assert.IsType<Platform>(createdAtResult.Value);
        Assert.Equal(expectedPlatform.Id, returnedPlatform.Id);
        Assert.Equal(expectedPlatform.Type, returnedPlatform.Type);
    }

    [Fact]
    public async Task CreatePlatform_BadModel_ReturnsBadRequest()
    {
        // Arrange
        var testPlatform = new CreateUpdatePlatformDto
        {
            Platform = new PlatformDto
            {
                Type = null,
            },
        };

        // Make the model state invalid
        _controller.ModelState.AddModelError("Type", "Required");

        // Act
        var result = await _controller.CreatePlatform(testPlatform);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPlatform_ValidPlatformId_ReturnsPlatform()
    {
        // Arrange
        var platformId = "123";
        var expectedPlatform = new Platform
        {
            Id = platformId,
            Type = "PC",
        };

        _platformService.Setup(s => s.GetPlatformById(platformId)).ReturnsAsync(expectedPlatform);

        // Act
        var result = await _controller.GetPlatform(platformId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPlatform = Assert.IsType<Platform>(okResult.Value);
        Assert.Equal(expectedPlatform.Id, returnedPlatform.Id);
        Assert.Equal(expectedPlatform.Type, returnedPlatform.Type);
    }

    [Fact]
    public async Task GetPlatform_NoPlatformFound_ReturnsNotFound()
    {
        // Arrange
        var platformId = "123";
        _platformService.Setup(s => s.GetPlatformById(platformId)).Returns(Task.FromResult<Platform>(null));

        // Act
        var result = await _controller.GetPlatform(platformId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdatePlatform_ValidData_ReturnsUpdatedPlatform()
    {
        // Arrange
        var testPlatform = new CreateUpdatePlatformDto
        {
            Platform = new PlatformDto
            {
                Id = "123",
                Type = "PC",
            },
        };

        var expectedPlatform = new Platform
        {
            Id = testPlatform.Platform.Id,
            Type = testPlatform.Platform.Type,
        };

        _platformService.Setup(s => s.UpdatePlatform(testPlatform)).ReturnsAsync(expectedPlatform);

        // Act
        var result = await _controller.UpdatePlatform(testPlatform);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedPlatform = Assert.IsType<Platform>(createdAtResult.Value);
        Assert.Equal(expectedPlatform.Id, returnedPlatform.Id);
        Assert.Equal(expectedPlatform.Type, returnedPlatform.Type);
    }

    [Fact]
    public async Task UpdatePlatform_BadModelState_ReturnsBadRequest()
    {
        // Arrange
        var testPlatform = new CreateUpdatePlatformDto
        {
            Platform = new PlatformDto
            {
                Type = null,
            },
        };

        _controller.ModelState.AddModelError("Type", "Required");

        // Act
        var result = await _controller.UpdatePlatform(testPlatform);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeletePlatform_ValidPlatformId_DeletesPlatformAndReturnsIt()
    {
        // Arrange
        var platformId = "123";
        var expectedPlatform = new Platform
        {
            Id = platformId,
            Type = "PC",
        };

        _platformService.Setup(s => s.DeletePlatform(platformId)).ReturnsAsync(expectedPlatform);

        // Act
        var result = await _controller.DeletePlatform(platformId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPlatform = Assert.IsType<Platform>(okResult.Value);
        Assert.Equal(expectedPlatform.Id, returnedPlatform.Id);
        Assert.Equal(expectedPlatform.Type, returnedPlatform.Type);
    }

    [Fact]
    public async Task DeletePlatform_InvalidPlatformId_ReturnsNotFound()
    {
        // Arrange
        var platformId = "123";
        _platformService.Setup(s => s.DeletePlatform(platformId)).Returns(Task.FromResult<Platform>(null));

        // Act
        var result = await _controller.DeletePlatform(platformId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAllPlatforms_ReturnsAllPlatforms()
    {
        // Arrange
        var platforms = new List<Platform>
    {
        new()
        {
            Id = "1",
            Type = "PC",
            Games = new List<Game>
            {
                new() { Id = "1", Name = "Test game 1", Key = "test-game-1" },
                new() { Id = "3", Name = "Test game 3", Key = "test-game-3" },
            },
        },
        new()
        {
            Id = "2",
            Type = "Console",
            Games = new List<Game>
            {
                new() { Id = "2", Name = "Test game 2", Key = "test-game-2" },
            },
        },
    };

        _platformService.Setup(s => s.GetAllPlatforms()).ReturnsAsync(platforms);

        // Act
        var result = await _controller.GetAllPlatforms();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPlatforms = Assert.IsType<List<Platform>>(okResult.Value);
        Assert.Equal(2, returnedPlatforms.Count);
    }

    [Fact]
    public async Task GetPlatformsByGame_ValidGameAlias_ReturnsPlatforms()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "game1";
        var expectedPlatforms = new List<Platform>
    {
        new()
        {
            Id = "1",
            Type = "PC",
        },
        new()
        {
            Id = "2",
            Type = "Console",
        },
    };

        _platformService.Setup(s => s.GetPlatformsByGame(gameAlias, languageCode)).ReturnsAsync(expectedPlatforms);

        // Act
        var result = await _controller.GetPlatformsByGame(gameAlias, languageCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPlatforms = Assert.IsType<List<Platform>>(okResult.Value);
        Assert.Equal(expectedPlatforms.Count, returnedPlatforms.Count);
    }

    [Fact]
    public async Task GetPlatformsByGame_InValidGameAlias_ReturnsNotFound()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "game1";
        _platformService.Setup(s => s.GetPlatformsByGame(gameAlias, languageCode)).ReturnsAsync((List<Platform>)null);

        // Act
        var result = await _controller.GetPlatformsByGame(gameAlias, languageCode);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
}
