using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Services.Data;
using Storefy.Services.Repositories.Gamestore;

namespace Storefy.Tests.Services.Repositories.Gamestore;
public class PlatformRepositoryTests : IDisposable
{
    private readonly PlatformRepository _platformRepository;
    private readonly StorefyDbContext _dbContext;
    private bool _disposed;

    public PlatformRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StorefyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StorefyDbContext(options);
        _platformRepository = new PlatformRepository(_dbContext);
    }

    [Fact]
    public async Task Add_NewPlatform_ReturnsPlatform()
    {
        // Arrange
        var platformDto = new CreateUpdatePlatformDto { Platform = new PlatformDto { Id = "5", Type = "PlayStation" } };

        // Act
        var result = await _platformRepository.Add(platformDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platformDto.Platform.Type, result.Type);
    }

    [Fact]
    public async Task Add_PlatformAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var platform = new Platform { Id = Guid.NewGuid().ToString(), Type = "PlayStation" };
        _dbContext.Platforms.Add(platform);
        await _dbContext.SaveChangesAsync();

        var platformDto = new CreateUpdatePlatformDto { Platform = new PlatformDto { Type = "PlayStation" } };

        // Act and Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _platformRepository.Add(platformDto));

        // Assert
        Assert.Equal("This platform already exist!", exception.Message);
    }

    [Fact]
    public async Task DeletePlatform_DeletePlatform_ValidData()
    {
        // Arrange
        var platform = new Platform { Id = Guid.NewGuid().ToString(), Type = "Console" };
        _dbContext.Platforms.Add(platform);
        await _dbContext.SaveChangesAsync();

        // Act
        var removedPlatform = await _platformRepository.Delete(platform);

        // Assert
        var allPlatforms = await _dbContext.Platforms.ToListAsync();
        Assert.DoesNotContain(removedPlatform, allPlatforms);
    }

    [Fact]
    public async Task GetByIdPlatform_ReturnExistingPlatform_ValidData()
    {
        // Arrange
        var platform = new Platform { Id = "10", Type = "Console" };
        _dbContext.Platforms.Add(platform);
        await _dbContext.SaveChangesAsync();

        // Act
        var existedPlatform = await _platformRepository.GetById(platform.Id);

        // Assert
        Assert.Equal(platform.Id, existedPlatform.Id);
    }

    [Fact]
    public async Task GetById_ThrowsException_When_PlatformDoesNotExist()
    {
        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _platformRepository.GetById("5005"));
    }

    [Fact]
    public async Task GetAllGenre_ReturnAllGenres()
    {
        // Arrange
        var platform1 = new Platform { Id = "3", Type = "Console" };
        var platform2 = new Platform { Id = "4", Type = "Browser" };
        _dbContext.Platforms.AddRange(platform1, platform2);
        await _dbContext.SaveChangesAsync();

        // Act
        var platforms = await _platformRepository.GetAll();

        // Assert
        Assert.Equal(2, platforms.Count());
    }

    [Fact]
    public async Task Update_PlatformExists_UpdatesAndReturnsPlatform()
    {
        // Arrange
        var platformId = Guid.NewGuid().ToString();
        var platform = new Platform { Id = platformId, Type = "PlayStation" };
        _dbContext.Platforms.Add(platform);
        await _dbContext.SaveChangesAsync();
        var platformDto = new CreateUpdatePlatformDto { Platform = new PlatformDto { Id = platformId, Type = "XBox" } };

        // Act
        var updatedPlatform = await _platformRepository.Update(platformDto);

        // Assert
        Assert.NotNull(updatedPlatform);
        Assert.Equal(platformDto.Platform.Type, updatedPlatform.Type);
    }

    [Fact]
    public async Task Update_PlatformDoesNotExist_ThrowsExcetion()
    {
        // Arrange
        var platformDto = new CreateUpdatePlatformDto
        {
            Platform = new PlatformDto
            {
                Id = Guid.NewGuid().ToString(),
                Type = "XBox",
            },
        };

        // Act
        var exception = await Assert
            .ThrowsAsync<InvalidOperationException>(() => _platformRepository.Update(platformDto));

        // Assert
        Assert.Equal("Cannot update this platform.", exception.Message);
    }

    [Fact]
    public async Task GetPlatformsByGameAlias_GameExists_ReturnsPlatforms()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "game1";
        var game = new Game { Id = "123", Name = "GameTest", Key = gameAlias };
        var platform1 = new Platform { Id = Guid.NewGuid().ToString(), Type = "Platform1", Games = new List<Game> { game } };
        var platform2 = new Platform { Id = Guid.NewGuid().ToString(), Type = "Platform2", Games = new List<Game> { game } };
        _dbContext.Platforms.AddRange(new List<Platform> { platform1, platform2 });
        await _dbContext.SaveChangesAsync();

        // Act
        var platforms = await _platformRepository.GetPlatformsByGameAlias(gameAlias, languageCode);

        // Assert
        Assert.Equal(2, platforms.Count());
    }

    [Fact]
    public async Task GetPlatformsByGameAlias_GameExists_ReturnsTranslatedPlatforms()
    {
        // Arrange
        var languageCode = "ua";
        var gameAlias = "game1";
        var game = new Game { Id = "123", Name = "GameTest", Key = gameAlias };
        var platform = new Platform { Id = Guid.NewGuid().ToString(), Type = "Platform1", Games = new List<Game> { game } };
        _dbContext.Platforms.Add(platform);

        var language = new Language
        {
            Id = "50",
            LanguageCode = "ua",
            LanguageName = "Ukranian",
        };
        var platformTranslate = new PlatformTranslation
        {
            Id = "10",
            Language = language,
            LanguageId = language.Id,
            Platform = platform,
            PlatformId = platform.Id,
            Type = "Платформа",
        };

        language.PlatformTranslations = new List<PlatformTranslation>()
        {
            platformTranslate,
        };

        _dbContext.Languages.Add(language);
        _dbContext.PlatformTranslations.Add(platformTranslate);
        await _dbContext.SaveChangesAsync();

        // Act
        var platforms = await _platformRepository.GetPlatformsByGameAlias(gameAlias, languageCode);

        // Assert
        Assert.Single(platforms);
        Assert.Equal(platformTranslate.Type, platform.Type);
    }

    [Fact]
    public async Task GetPlatformsByGameAlias_GameDoesNotExist_ReturnsEmpty()
    {
        // Arrange
        var languageCode = "en";
        var nonExistingGameAlias = "non-existing-game";

        // Act
        var platforms = await _platformRepository.GetPlatformsByGameAlias(nonExistingGameAlias, languageCode);

        // Assert
        Assert.Empty(platforms);
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
