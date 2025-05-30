using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Services.Services;

namespace Storefy.Tests.Services.Services;
public class PlatformServiceTests
{
    private readonly PlatformService _platformService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public PlatformServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _platformService = new PlatformService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddPlatform_AddsNewPlatform()
    {
        // Arrange
        var platformDto = new CreateUpdatePlatformDto
        {
            Platform = new PlatformDto
            {
                Id = "1",
                Type = "Mobile",
            },
        };
        _unitOfWorkMock.Setup(uow => uow.PlatformRepository.Add(platformDto))
            .ReturnsAsync(new Platform { Id = platformDto.Platform.Id, Type = platformDto.Platform.Type });

        // Act
        var newPlatform = await _platformService.AddPlatform(platformDto);

        // Assert
        Assert.NotNull(newPlatform);
        Assert.Equal(platformDto.Platform.Type, newPlatform.Type);
    }

    [Fact]
    public async Task GetAllPlatforms_ReturnsListOfAllPlatforms()
    {
        // Arrange
        var platforms = new List<Platform>
            {
                new() { Id = "1", Type = "PC" },
                new() { Id = "2", Type = "PlayStation" },
                new() { Id = "3", Type = "Xbox" },
            };
        _unitOfWorkMock.Setup(uow => uow.PlatformRepository.GetAll())
            .ReturnsAsync(platforms);

        // Act
        var result = await _platformService.GetAllPlatforms();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platforms.Count, result.Count());
    }

    [Fact]
    public async Task GetPlatformById_ReturnsPlatformWithMatchingId()
    {
        // Arrange
        var platformId = "1";
        var platform = new Platform { Id = platformId, Type = "PC" };
        _unitOfWorkMock.Setup(uow => uow.PlatformRepository.GetById(platformId))
            .ReturnsAsync(platform);

        // Act
        var result = await _platformService.GetPlatformById(platformId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platformId, result.Id);
    }

    [Fact]
    public async Task UpdatePlatform_UpdatesPlatformInformation()
    {
        // Arrange
        var platformDto = new CreateUpdatePlatformDto
        {
            Platform = new PlatformDto
            {
                Id = "1",
                Type = "Updated platform",
            },
        };
        _unitOfWorkMock.Setup(uow => uow.PlatformRepository.Update(platformDto))
            .ReturnsAsync(new Platform { Id = platformDto.Platform.Id, Type = platformDto.Platform.Type });

        // Act
        var updatedPlatform = await _platformService.UpdatePlatform(platformDto);

        // Assert
        Assert.NotNull(updatedPlatform);
        Assert.Equal(platformDto.Platform.Type, updatedPlatform.Type);
        Assert.Equal(platformDto.Platform.Id, updatedPlatform.Id);
    }

    [Fact]
    public async Task DeletePlatform_DeletesPlatformWithSpecifiedId()
    {
        // Arrange
        var platformId = "1";
        var platform = new Platform { Id = platformId, Type = "PC" };
        _unitOfWorkMock.Setup(uow => uow.PlatformRepository.GetById(platformId))
            .ReturnsAsync(platform);
        _unitOfWorkMock.Setup(uow => uow.PlatformRepository.Delete(platform))
            .ReturnsAsync(platform);

        // Act
        var deletedPlatform = await _platformService.DeletePlatform(platformId);

        // Assert
        Assert.NotNull(deletedPlatform);
        Assert.Equal(platformId, deletedPlatform.Id);
    }

    [Fact]
    public async Task GetPlatformsByGame_ReturnsPlatformWithSpecifiedGame()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "test-game";
        var platforms = new List<Platform>
        {
            new()
            {
                Id = "1",
                Type = "Mobile",
                Games = new List<Game>
                {
                    new()
                    {
                        Id = "5",
                        Name = "Test game",
                        Key = "test-game",
                    },
                },
            },
            new()
            {
                Id = "2",
                Type = "Dekstop",
                Games = new List<Game>
                {
                    new()
                    {
                        Id = "4",
                        Name = "Test game",
                        Key = "test-game",
                    },
                },
            },
        };
        _unitOfWorkMock.Setup(uow => uow.PlatformRepository.GetPlatformsByGameAlias(gameAlias, languageCode))
            .ReturnsAsync(platforms);

        // Act
        var result = await _platformService.GetPlatformsByGame(gameAlias, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platforms.Count, result.Count());
        Assert.Equal(result.First().Games.Count, result.First().Games.Count);
    }
}