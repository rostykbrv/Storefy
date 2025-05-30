using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Services.Data;
using Storefy.Services.Repositories.Gamestore;

namespace Storefy.Tests.Services.Repositories.Gamestore;
public class GameRepositoryTests : IDisposable
{
    private readonly GameRepository _gameRepository;
    private readonly StorefyDbContext _dbContext;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private bool _disposed;

    public GameRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StorefyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _dbContext = new StorefyDbContext(options);

        _gameRepository = new GameRepository(_dbContext, _mockHttpContextAccessor.Object);

        _dbContext.Platforms.Add(new Platform
        {
            Id = "10",
            Type = "TestPlatform",
        });

        _dbContext.Genres.Add(new Genre
        {
            Id = "1",
            CategoryId = "12",
            Name = "TestGenre",
            Description = "Test genre description",
            ParentGenre = null,
            ParentGenreId = null,
            Picture = "Genre picture",
        });

        _dbContext.Publishers.Add(new Publisher
        {
            Id = "2",
            SupplierId = "1234",
            CompanyName = "TestPublisher",
            Description = "Description",
            HomePage = "HomePage",
            Address = "Address",
            City = "City",
            ContactName = "PublisherName",
            ContactTitle = "Title",
            Country = "USA",
            Fax = "Fax",
            Phone = "Phone",
            Region = "Region",
        });

        _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task AddGame_ValidData()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Name = "Test 1",
                Description = "Description 1",
                Key = "Key 1",
                Price = 100,
                UnitInStock = 50,
                Discontinued = 1,
            },
            Publisher = "2",
            PublisherName = "TestPublisher",
            GenreNames = new List<string>
            {
                "TestGenre",
            },
            Genres = new List<string>
            {
                "1",
            },
            Platforms = new List<string>
            {
                "10",
            },
        };

        // Act
        var result = await _gameRepository.Add(gameDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameDto.Game.Name, result.Name);
        Assert.Equal(gameDto.Game.Key, result.Key);
        Assert.Equal(gameDto.Game.Price, result.Price);
        Assert.Equal(gameDto.Publisher, result.PublisherId);
        Assert.Equal(gameDto.Genres.First(), result.Genres.First().Id);
        Assert.Equal(gameDto.Platforms.First(), result.Platforms.First().Id);
    }

    [Fact]
    public async Task AddGame_WithoutGenres()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Name = "Test 3",
                Description = "Description 3",
                Key = "Key 3",
                Price = 300,
                UnitInStock = 50,
                Discontinued = 1,
            },
            Publisher = "2",
            Genres = new List<string>(),
            Platforms = new List<string> { "10" },
        };

        // Act
        var result = await _gameRepository.Add(gameDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Genres);
        Assert.Equal(gameDto.Platforms.First(), result.Platforms.First().Id);
    }

    [Fact]
    public async Task AddGame_WithoutPlatforms()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Name = "Test 3",
                Description = "Description 3",
                Key = "Key 3",
                Price = 300,
                UnitInStock = 50,
                Discontinued = 1,
            },
            Publisher = "2",
            Genres = new List<string>() { "1" },
            Platforms = new List<string>(),
        };

        // Act
        var result = await _gameRepository.Add(gameDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Platforms);
        Assert.Equal(gameDto.Game.Name.First(), result.Name.First());
    }

    [Fact]
    public async Task AddGame_WithoutPublisher()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Name = "Test 2",
                Description = "Description 2",
                Key = "Key 2",
                Price = 200,
                UnitInStock = 50,
                Discontinued = 1,
            },
            Publisher = string.Empty,
            PublisherName = string.Empty,
            GenreNames = new List<string>
            {
                "TestGenre",
            },
            Genres = new List<string>
            {
                "1",
            },
            Platforms = new List<string>
            {
                "10",
            },
        };

        // Act
        var result = await _gameRepository.Add(gameDto);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.PublisherId);
        Assert.Null(result.Publisher);
        Assert.Equal(gameDto.Genres.First(), result.Genres.First().Id);
        Assert.Equal(gameDto.Platforms.First(), result.Platforms.First().Id);
    }

    [Fact]
    public async Task GetAllGames_HandleFilteringByName()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        };

        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestSecond",
            Description = "Description",
            Key = "Key-2",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            Sort = null,
            Name = "Test",
            DatePublishing = null,
            Genres = null,
            Platforms = null,
            Publishers = null,
            MaxPrice = null,
            MinPrice = null,
            Page = 1,
            PageCount = null,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Equal(2, result.Games.Length);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetAllGames_WithNonEnglishLanguage_ReturnsTranslatedGames()
    {
        // Arrange
        var filters = new FilterOptions() { Trigger = "ApplyFilters" };
        string languageCode = "ua";

        var game = new Game()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        };

        var language = new Language
        {
            Id = "40",
            LanguageCode = "ua",
            LanguageName = "Ukranian",
            GameTranslations = new List<GameTranslation>(),
        };

        var gameTranslation = new GameTranslation
        {
            Id = "1",
            CopyType = "Повна гра",
            Game = game,
            Description = "Опис гри",
            GameId = game.Id,
            Language = language,
            LanguageId = language.Id,
            ReleasedDate = "10 Жовтня",
        };

        language.GameTranslations.Add(gameTranslation);
        _dbContext.GameTranslations.Add(gameTranslation);
        _dbContext.Languages.Add(language);
        _dbContext.Add(game);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(game.Key, result.Games.First().Key);
        Assert.Equal(gameTranslation.CopyType, result.Games.First().CopyType);
    }

    [Fact]
    public async Task GetAllGames_HandleFilteringByGenresAndPlatforms()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            Genres = new List<Genre>
            {
                new()
                {
                    Id = "15",
                    Name = "TestGenre1",
                },
            },
            Platforms = new List<Platform>
            {
                new()
                {
                    Id = "11",
                    Type = "Mobile",
                },
            },
        };

        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestSecond",
            Description = "Description",
            Key = "Key-2",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            Genres = new List<Genre>
            {
                new()
                {
                    Id = "22",
                    Name = "TestGenre2",
                },
            },
            Platforms = game1.Platforms,
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            Sort = null,
            Name = "Test",
            DatePublishing = null,
            Genres = new string[] { "15" },
            Platforms = new string[] { "11" },
            Publishers = null,
            MaxPrice = null,
            MinPrice = null,
            Page = 1,
            PageCount = null,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Single(result.Games);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetAllGames_HandleFilteringByPublisher()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            Publisher = new Publisher { Id = "111", CompanyName = "TestPublisher" },
        };

        _dbContext.Add(game1);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            Publishers = new string[] { "111" },
            Page = 1,
            PageCount = 20,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Single(result.Games);
        Assert.Equal(game1.Name, result.Games.FirstOrDefault().Name);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetAllGames_HandleFilteringByPriceRange()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        };

        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test2",
            Description = "Description2",
            Key = "Key2",
            Price = 200,
            UnitInStock = 50,
            Discontinued = 1,
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            MinPrice = 150,
            MaxPrice = 250,
            Page = 1,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Single(result.Games);
        Assert.Equal(game2.Name, result.Games.FirstOrDefault().Name);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetAllGames_HandleFilteringByDate()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddMonths(-2),
        };
        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test2",
            Description = "Description2",
            Key = "Key2",
            Price = 200,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddDays(-2),
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            DatePublishing = "last week",
            Page = 1,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Single(result.Games);
        Assert.Equal(game2.Name, result.Games.FirstOrDefault().Name);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetAllGames_HandleFilteringByDateLastMonth()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddMonths(-2),
        };
        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test2",
            Description = "Description2",
            Key = "Key2",
            Price = 200,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddDays(-20),
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            DatePublishing = "last month",
            Page = 1,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Single(result.Games);
        Assert.Equal(game2.Name, result.Games.FirstOrDefault().Name);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetAllGames_HandleFilteringByDateLastYear()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddMonths(-11),
        };
        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test2",
            Description = "Description2",
            Key = "Key2",
            Price = 200,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddYears(-5),
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            DatePublishing = "last year",
            Page = 1,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Single(result.Games);
        Assert.Equal(game1.Name, result.Games.FirstOrDefault().Name);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetAllGames_HandleFilteringByDateLastTwoYears()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddYears(-1),
        };
        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test2",
            Description = "Description2",
            Key = "Key2",
            Price = 200,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddYears(-5),
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            DatePublishing = "2 years",
            Page = 1,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Single(result.Games);
        Assert.Equal(game1.Name, result.Games.FirstOrDefault().Name);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetAllGames_HandleFilteringByDateLastThreeYears()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddYears(-2),
        };
        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test2",
            Description = "Description2",
            Key = "Key2",
            Price = 200,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddYears(-5),
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            DatePublishing = "3 years",
            Page = 1,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Single(result.Games);
        Assert.Equal(game1.Name, result.Games.FirstOrDefault().Name);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetAllGames_HandleFilteringByDateThrowsExceptionForInvalidFilter()
    {
        // Arrange
        var languageCode = "en";
        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            DatePublishing = "invalid",
            Page = 1,
        };

        // Act and Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _gameRepository.GetAllGames(filters, languageCode));
        Assert.Equal("Invalid DateAddedOption filter", exception.Message);
    }

    [Fact]
    public async Task GetAllGames_HandleSortingByName()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ZTest",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        };

        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ATest",
            Description = "Description",
            Key = "Key2",
            Price = 200,
            UnitInStock = 50,
            Discontinued = 1,
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "SortingChange",
            Sort = "name",
            Page = 1,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Equal(game2.Name, result.Games[0].Name);
    }

    [Fact]
    public async Task GetAllGames_HandleSortingByPriceAsc()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 200,
            UnitInStock = 50,
            Discontinued = 1,
        };

        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test2",
            Description = "Description2",
            Key = "Key2",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "SortingChange",
            Sort = "price asc",
            Page = 1,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Equal(game2.Price, result.Games[0].Price);
    }

    [Fact]
    public async Task GetAllGames_HandleSortingByPriceDesc()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 200,
            UnitInStock = 50,
            Discontinued = 1,
        };

        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test2",
            Description = "Description2",
            Key = "Key2",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "SortingChange",
            Sort = "Price Desc",
            Page = 1,
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Equal(game1.Price, result.Games[0].Price);
    }

    [Fact]
    public async Task GetAllGames_HandleSortingByMostViewed()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ZTest",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            ViewCount = 10,
        };

        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ATest",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            ViewCount = 50,
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "SortingChange",
            Sort = "most popular (most viewed)",
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Equal(game2.Name, result.Games[0].Name);
    }

    [Fact]
    public async Task GetAllGames_HandleSortingByMostCommented()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ZTest",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            Comments = new List<Comment>
            {
                new()
                {
                    Id = "1",
                    Name = "Alex",
                    Body = "Test comment",
                },
            },
        };

        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ATest",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            Comments = new List<Comment>
            {
                new()
                {
                    Id = "2",
                    Name = "Alex1",
                    Body = "Test comment",
                },
                new()
                {
                    Id = "3",
                    Name = "Alex2",
                    Body = "Test comment",
                },
            },
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "SortingChange",
            Sort = "most commented",
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Equal(game2.Name, result.Games[0].Name);
    }

    [Fact]
    public async Task GetAllGames_HandleSortingByNew()
    {
        // Arrange
        var languageCode = "en";
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ZTest",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddDays(-7),
        };

        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ATest",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            DateAdded = DateTime.Now.AddDays(-1),
        };

        _dbContext.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        var filters = new FilterOptions
        {
            Trigger = "SortingChange",
            Sort = "new (by date)",
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GamePageResult>(result);
        Assert.Equal(game2.Name, result.Games[0].Name);
    }

    [Fact]
    public async Task GetAllGames_InvalidSortingOption_ThrowsException()
    {
        // Arrange
        var languageCode = "en";
        var filters = new FilterOptions
        {
            Trigger = "SortingChange",
            Sort = "invalid_sort_option",
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _gameRepository.GetAllGames(filters, languageCode));
        Assert.Equal("Invalid sorting option", exception.Message);
    }

    [Fact]
    public async Task GetAllGames_InvalidTriggerOption_ThrowsException()
    {
        // Arrange
        var languageCode = "en";
        var filters = new FilterOptions
        {
            Trigger = "InvalidTrigger",
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _gameRepository.GetAllGames(filters, languageCode));
        Assert.Equal("Invalid filter/sorting option", exception.Message);
    }

    [Fact]
    public async Task GetAllGames_WhenTriggerIsPageChange_IgnoresOtherFilters()
    {
        // Arrange
        var languageCode = "en";
        var filters = new FilterOptions
        {
            Trigger = "PageChange",
        };

        // Act
        var result = await _gameRepository.GetAllGames(filters, languageCode);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AliasExists_AliasDoesExist_ReturnsTrue()
    {
        // Arrange
        var alias = "Key 1";

        _dbContext.Games.Add(new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test 1",
            Description = "Description 1",
            Key = alias,
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        });

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.AliasExists(alias);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AliasExists_AliasDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var alias = "Key 2";

        // Act
        var result = await _gameRepository.AliasExists(alias);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Delete_GameExists_GameIsDeleted()
    {
        // Arrange
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        };

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.Delete(game);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(game, result);

        var gameInDb = await _dbContext.Games.FindAsync(game.Id);
        Assert.Null(gameInDb);
    }

    [Fact]
    public async Task ReturnAllGames_Success()
    {
        // Arrange
        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = "Key",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        };
        _dbContext.Add(game1);
        await _dbContext.SaveChangesAsync();

        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test-2",
            Description = "Description",
            Key = "Key-2",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
        };
        _dbContext.Add(game2);
        await _dbContext.SaveChangesAsync();

        // Act
        var games = await _gameRepository.GetAll();

        // Assert
        Assert.Equal(2, games.Count());
    }

    [Fact]
    public async Task GetByAlias_AliasDoesExist_ReturnsGame()
    {
        // Arrange
        var languageCode = "en";
        var alias = "Key";

        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Description = "Description",
            Key = alias,
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            Genres = new List<Genre>(),
            Platforms = new List<Platform>(),
        };

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.GetByAlias(alias, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(alias, result.Key);
    }

    [Fact]
    public async Task GetByAlias_AliasDoesExist_ReturnsTranslatedGame()
    {
        // Arrange
        var languageCode = "ua";
        var alias = "game-test";

        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Game Test",
            Description = "Description",
            Key = alias,
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            Genres = new List<Genre>(),
            Platforms = new List<Platform>(),
        };

        var language = new Language
        {
            Id = "40",
            LanguageCode = "ua",
            LanguageName = "Ukranian",
        };

        var gameTranslation = new GameTranslation
        {
            Id = "1",
            CopyType = "Повна гра",
            Game = game,
            Description = "Опис гри",
            GameId = game.Id,
            Language = language,
            LanguageId = language.Id,
            ReleasedDate = "10 Жовтня",
        };

        _dbContext.GameTranslations.Add(gameTranslation);
        _dbContext.Languages.Add(language);
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.GetByAlias(alias, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(alias, result.Key);
        Assert.Equal("Опис гри", result.Description);
    }

    [Fact]
    public async Task GetByAlias_AliasDoesNotExist_ReturnsNull()
    {
        // Arrange
        var languageCode = "en";
        var alias = "MissingKey";

        // Act
        var result = await _gameRepository.GetByAlias(alias, languageCode);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetGamesByGenre_GamesExistWithGenre_ReturnsGames()
    {
        // Arrange
        var genreId = "5";
        var genre = new Genre { Id = genreId, Name = "TestGenre" };
        _dbContext.Genres.Add(genre);

        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test 1",
            Key = "Key 1",
            Genres = new List<Genre> { genre },
        };
        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test 2",
            Key = "Key 2",
            Genres = new List<Genre> { genre },
        };

        _dbContext.Games.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.GetGamesByGenre(genreId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetGamesByGenre_NoGamesExistWithGenre_ReturnsEmpty()
    {
        // Arrange
        var genreId = "5";
        var genre = new Genre { Id = genreId, Name = "Action" };
        _dbContext.Genres.Add(genre);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.GetGamesByGenre(genreId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGamesByGenre_ReturnsEmptyList_WhenGenreDoesNotExist()
    {
        // Arrange
        var nonExistentGenreId = "5005";

        // Act
        var result = await _gameRepository.GetGamesByGenre(nonExistentGenreId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGamesByPlatform_GamesExistWithPlatform_ReturnsGames()
    {
        // Arrange
        var platformId = "1";
        var platform = new Platform { Id = platformId, Type = "TestPlatform" };
        _dbContext.Platforms.Add(platform);

        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test 1",
            Key = "Key 1",
            Platforms = new List<Platform> { platform },
        };
        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test 2",
            Key = "Key 2",
            Platforms = new List<Platform> { platform },
        };

        _dbContext.Games.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.GetGamesByPlatform(platformId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetGamesByPlatform_NoGamesExistWithPlatform_ReturnsEmpty()
    {
        // Arrange
        var platformId = "1";
        var platform = new Platform { Id = platformId, Type = "Mobile" };
        _dbContext.Platforms.Add(platform);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.GetGamesByPlatform(platformId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGamesByPlatform_ThrowException_WhenPlatformDoesNotExist()
    {
        // Arrange
        var nonExistentPlatformId = "5000000";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _gameRepository.GetGamesByPlatform(nonExistentPlatformId));
    }

    [Fact]
    public async Task GetGamesByPublisher_GamesExistWithPublisher_ReturnsGames()
    {
        // Arrange
        var publisherCompanyName = "BestCompany";
        var publisher = new Publisher
        {
            Id = "33",
            CompanyName = publisherCompanyName,
            Description = "description",
            HomePage = "homepage",
        };

        _dbContext.Publishers.Add(publisher);

        var game1 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test 1",
            Key = "Key 1",
            PublisherId = publisher.Id,
            Publisher = publisher,
        };
        var game2 = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test 2",
            Key = "Key 2",
            PublisherId = publisher.Id,
            Publisher = publisher,
        };

        _dbContext.Games.AddRange(game1, game2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.GetGamesByPublisher(publisherCompanyName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetGamesByPublisher_NoGamesExistWithPublisher_ReturnsEmpty()
    {
        // Arrange
        var publisherCompanyName = "BestCompany";
        var publisher = new Publisher
        {
            Id = "33",
            CompanyName = publisherCompanyName,
            Description = "description",
            HomePage = "homepage",
        };
        _dbContext.Publishers.Add(publisher);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _gameRepository.GetGamesByPublisher(publisherCompanyName);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGamesByPublisher_ReturnEmptyList_WhenPublisherNotContainGames()
    {
        // Arrange
        var nonExistentPublisher = "NotAGameCompany";

        // Act & Assert
        var result = await _gameRepository.GetGamesByPublisher(nonExistentPublisher);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Update_GameExists_UpdatesGame()
    {
        // Arrange
        var genre = new Genre { Id = "gen1", Name = "Action" };
        var platform = new Platform { Id = "plat1", Type = "PC" };
        var publisher = new Publisher
        {
            Id = "50",
            CompanyName = "PublisherCompany",
            Description = "description",
            HomePage = "homepage",
        };

        _dbContext.Genres.Add(genre);
        _dbContext.Platforms.Add(platform);
        _dbContext.Publishers.Add(publisher);

        var gameId = Guid.NewGuid().ToString();
        var game = new Game
        {
            Id = gameId,
            Name = "Test Game",
            Key = "test-game",
            Genres = new List<Genre> { genre },
            Platforms = new List<Platform> { platform },
            Price = 10.99m,
            Publisher = publisher,
        };

        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        var updatedGameDto = new GameDto
        {
            Id = gameId,
            Name = "Updated Game",
            Key = "test-game",
            Price = 99.99m,
            UnitInStock = 5,
            Discontinued = 1,
        };

        var updateDto = new CreateUpdateGameDto
        {
            Game = updatedGameDto,
            Genres = new List<string> { genre.Id },
            GenreNames = new List<string>
            {
                "Action",
            },
            PublisherName = "PublisherCompany",
            Platforms = new List<string> { platform.Id },
            Publisher = "50",
        };

        // Act
        var updatedGame = await _gameRepository.Update(updateDto);

        // Assert
        Assert.NotNull(updatedGame);
        Assert.Equal(updatedGameDto.Name, updatedGame.Name);
        Assert.Equal(updatedGameDto.Key, updatedGame.Key);
        Assert.Equal(updatedGameDto.Price, updatedGame.Price);
    }

    [Fact]
    public async Task Update_PublisherDoesNotExist_UpdatesGameWithNullPublisher()
    {
        // Arrange
        var genre = new Genre { Id = "gen1", Name = "Action" };
        var platform = new Platform { Id = "plat1", Type = "PC" };
        var publisher = new Publisher
        {
            Id = "55",
            CompanyName = "PublisherCompany",
            Description = "description",
            HomePage = "homepage",
        };

        _dbContext.Genres.Add(genre);
        _dbContext.Platforms.Add(platform);
        _dbContext.Publishers.Add(publisher);

        var gameId = Guid.NewGuid().ToString();
        var game = new Game
        {
            Id = gameId,
            Name = "Test Game",
            Key = "test-game",
            Genres = new List<Genre> { genre },
            PublisherId = publisher.Id,
            Price = 10.99m,
        };

        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        var updatedGameDto = new GameDto
        {
            Id = gameId,
            Name = "Updated Game",
            Key = "test-game",
            Price = 99.99m,
            UnitInStock = 5,
            Discontinued = 1,
        };

        var updateDto = new CreateUpdateGameDto
        {
            Game = updatedGameDto,
            Genres = new List<string> { genre.Id },
            Platforms = new List<string> { platform.Id },
            Publisher = string.Empty,
        };

        // Act
        var updatedGame = await _gameRepository.Update(updateDto);

        // Assert
        Assert.NotNull(updatedGame);
        Assert.Null(updatedGame.PublisherId);
        Assert.Equal(updatedGameDto.Name, updatedGame.Name);
        Assert.Equal(updatedGameDto.Key, updatedGame.Key);
    }

    [Fact]
    public async Task Update_GameDoesNotExist_ThrowsException()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var gameDto = new GameDto { Id = gameId, Name = "Test", Price = 99.99m, Key = "key_for_test", UnitInStock = 5, Discontinued = 1 };

        var updateDto = new CreateUpdateGameDto
        {
            Game = gameDto,
            Genres = new List<string>(),
            Platforms = new List<string>(),
        };

        // Act and Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _gameRepository.Update(updateDto));
        Assert.Equal($"This game doesn't exist!", exception.Message);
    }

    [Fact]
    public async Task BuyGame_AddsGameToNewOrder()
    {
        // Arrange
        var gameAlias = "game-alias-1";
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Key = gameAlias,
            Name = "Game 1",
            Price = 10,
            Discount = 0,
        };

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        // Act
        var order = await _gameRepository.BuyGame(gameAlias);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(OrderStatus.Open, order.Status);
        Assert.Single(order.OrderDetails);

        var orderDetails = order.OrderDetails.First();
        Assert.Equal(1, orderDetails.Quantity);
        Assert.Equal(game.Id, orderDetails.ProductId);
        Assert.Equal(game.Name, orderDetails.ProductName);
        Assert.Equal(game.Price, orderDetails.Price);
    }

    [Fact]
    public async Task BuyGame_SelectedGameNotExists_ThrowsException()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var game = new Game { Id = gameId, Name = "Test", Price = 99.99m, Key = "key_for_test", UnitInStock = 5, Discontinued = 1 };

        // Act and Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _gameRepository.BuyGame(game.Key));
        Assert.Equal($"This game doesn't exist.", exception.Message);
    }

    [Fact]
    public async Task BuyGame_IncrementsQuantityForExistingGameInOpenOrder()
    {
        // Arrange
        var gameKey = "game-1";
        var game = new Game
        {
            Id = "11",
            Key = gameKey,
            Name = "Game 1",
            Price = 10,
            Discount = 0,
        };

        var orderDetails = new OrderDetails
        {
            Id = "123",
            CreationDate = DateTime.Now,
            Discount = 0,
            Price = 11,
            Quantity = 1,
            ProductName = "Game 1",
            ProductId = "11",
            Game = game,
        };

        var newOrder = new Order
        {
            Id = Guid.NewGuid().ToString(),
            Status = OrderStatus.Open,
            PaidDate = null,
            CustomerId = Guid.NewGuid().ToString(),
            OrderDate = DateTime.Now,
            OrderDetails = new List<OrderDetails> { orderDetails },
        };

        _dbContext.Orders.Add(newOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        var order = await _gameRepository.BuyGame(gameKey);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(OrderStatus.Open, order.Status);
        Assert.Single(order.OrderDetails);

        var ordersDetails = order.OrderDetails.First();
        Assert.Equal(2, ordersDetails.Quantity);
    }

    [Fact]
    public async Task GetById_ReturnsGameWithInfo()
    {
        // Arrange
        var genre = new Genre
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Genre1",
        };
        var platform = new Platform
        {
            Id = Guid.NewGuid().ToString(),
            Type = "Platform1",
        };
        var publisher = new Publisher
        {
            Id = Guid.NewGuid().ToString(),
            CompanyName = "Publisher1",
        };

        var gameId = Guid.NewGuid().ToString();

        var game = new Game
        {
            Id = gameId,
            Key = "game-alias-1",
            Name = "Game 1",
            Price = 10,
            Discount = 0,
            Genres = new List<Genre> { genre },
            Platforms = new List<Platform> { platform },
            Publisher = publisher,
        };

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        // Act
        var gameResult = await _gameRepository.GetById(gameId);

        // Assert
        Assert.NotNull(gameResult);
        Assert.Equal(gameId, gameResult.Id);
        Assert.Equal(game.Name, gameResult.Name);
        Assert.Single(gameResult.Genres);
        Assert.Equal(genre.Id, gameResult.Genres.First().Id);
        Assert.Single(gameResult.Platforms);
        Assert.Equal(platform.Id, gameResult.Platforms.First().Id);
        Assert.Equal(publisher.Id, gameResult.Publisher.Id);
    }

    [Fact]
    public async Task GetComments_ReturnsGameComments()
    {
        // Arrange
        var gameAlias = "test-game";
        var game = new Game
        {
            Id = "11",
            Key = gameAlias,
            Name = "Game 1",
            Price = 10,
            Discount = 0,
        };

        var comment = new Comment
        {
            Id = "idMain",
            Name = "TestName",
            Body = "BodyTest",
            ChildComments = new List<Comment>
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "TestName2",
                    Body = "BodyTest2",
                    ChildComments = new List<Comment>(),
                    CommentId = "idMain",
                    Game = game,
                },
            },
            CommentId = null,
            Game = game,
        };

        _dbContext.Games.Add(game);
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();

        // Act
        var commentResult = await _gameRepository.GetComments(gameAlias);

        // Assert
        Assert.NotNull(commentResult);
        Assert.Equal(comment.Id, commentResult.First().Id);
        Assert.Equal(comment.Name, commentResult.First().Name);
        Assert.Equal(comment.Body, commentResult.First().Body);
    }

    [Fact]
    public async Task AddCommentToGame_ReturnsCreated_WhenNewCommentAdded()
    {
        // Arrange
        var game = new Game
        {
            Id = "11",
            Key = "test-game",
            Name = "Game 1",
            Price = 10,
            Discount = 0,
        };
        var user = new User
        {
            Id = "50",
            Name = "Jake",
            Password = "123",
            NotificationId = "76",
            Roles = new List<Role>(),
        };
        var userBanned = new User
        {
            Id = "55",
            Name = "Paul",
            Password = "123",
            NotificationId = "76",
            Roles = new List<Role>(),
        };
        var banHistory = new BanHistory
        {
            Id = "1",
            User = userBanned,
            BanStarted = DateTime.UtcNow,
            BanEnds = DateTime.Now.AddHours(1),
            UserId = "55",
        };

        _dbContext.Games.Add(game);
        _dbContext.Users.AddRange(user, userBanned);
        _dbContext.BanHistories.Add(banHistory);
        await _dbContext.SaveChangesAsync();

        var commentDto = new AddCommentDto
        {
            Comment = new Comment
            {
                Name = "Jake",
                Body = "Test comment body 4",
            },
            ParentId = null,
            Action = string.Empty,
        };

        var claims = new List<Claim> { new(ClaimTypes.Name, "Jake") };
        var mockPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthentication"));
        _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(mockPrincipal);

        // Act
        var comment = await _gameRepository.AddCommentToGame(game.Key, commentDto);

        // Assert
        Assert.NotNull(comment);
        Assert.Equal(commentDto.Comment.Name, comment.Name);
        Assert.Equal(commentDto.Comment.Body, comment.Body);
    }

    [Fact]
    public async Task AddCommentToGame_ThrowsException_WhenBannedUserAttemptsToAddComment()
    {
        // Arrange
        var game = new Game
        {
            Id = "11",
            Key = "test-game",
            Name = "Game 1",
            Price = 10,
            Discount = 0,
        };
        var user = new User
        {
            Id = "50",
            Name = "Jake",
            Password = "123",
            NotificationId = "76",
            Roles = new List<Role>(),
        };
        var userBanned = new User
        {
            Id = "55",
            Name = "Paul",
            Password = "123",
            NotificationId = "76",
            Roles = new List<Role>(),
        };
        var banHistory = new BanHistory
        {
            Id = "1",
            User = userBanned,
            BanStarted = DateTime.UtcNow,
            BanEnds = DateTime.Now.AddHours(1),
            UserId = "55",
        };

        _dbContext.Games.Add(game);
        _dbContext.Users.AddRange(user, userBanned);
        _dbContext.BanHistories.Add(banHistory);
        await _dbContext.SaveChangesAsync();

        var commentDto = new AddCommentDto
        {
            Comment = new Comment
            {
                Name = "Paul",
                Body = "Test comment body 4",
            },
            ParentId = null,
            Action = string.Empty,
        };

        var claims = new List<Claim> { new(ClaimTypes.Name, "Paul") };
        var mockPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthentication"));
        _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(mockPrincipal);

        // Act and Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _gameRepository.AddCommentToGame(game.Key, commentDto));

        // Assert
        Assert.Equal("User is currently banned and cannot write a comment", ex.Message);
    }

    [Fact]
    public async Task AddReplyToGame_ReturnsCreated_WhenReplyAdded()
    {
        // Arrange
        var game = new Game
        {
            Id = "11",
            Key = "test-game",
            Name = "Game 1",
            Price = 10,
            Discount = 0,
        };

        var parentComment = new Comment
        {
            Id = "idMain",
            Name = "TestName",
            Body = "BodyTest",
            ChildComments = new List<Comment>(),
            CommentId = null,
            Game = game,
        };
        var user = new User
        {
            Id = "50",
            Name = "Jake",
            Password = "123",
            NotificationId = "76",
            Roles = new List<Role>(),
        };
        var userBanned = new User
        {
            Id = "55",
            Name = "Paul",
            Password = "123",
            NotificationId = "76",
            Roles = new List<Role>(),
        };
        var banHistory = new BanHistory
        {
            Id = "1",
            User = userBanned,
            BanStarted = DateTime.UtcNow,
            BanEnds = DateTime.Now.AddHours(1),
            UserId = "55",
        };

        _dbContext.Users.AddRange(user, userBanned);
        _dbContext.BanHistories.Add(banHistory);
        _dbContext.Comments.Add(parentComment);
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        var replyComment = new AddCommentDto
        {
            Comment = new Comment
            {
                Name = "Test Name 2",
                Body = "Test reply body 2",
            },
            ParentId = parentComment.Id,
            Action = "Reply",
        };

        var claims = new List<Claim> { new(ClaimTypes.Name, "Jake") };
        var claimsIdentity = new ClaimsIdentity(claims);
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(p => p.Identity).Returns(claimsIdentity);
        _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(mockPrincipal.Object);

        // Act
        var comment = await _gameRepository.AddCommentToGame(game.Key, replyComment);

        // Assert
        var expectedComment = $"<a href=\"game/{game.Key}#comment{parentComment.Id}\">{parentComment.Name},</a> {replyComment.Comment.Body}";
        Assert.NotNull(comment);
        Assert.Equal(replyComment.Comment.Name, comment.Name);
        Assert.Equal(expectedComment, comment.Body);
    }

    [Fact]
    public async Task AddQuoteToGame_ReturnsCreated_WhenQuoteAdded()
    {
        // Arrange
        var game = new Game
        {
            Id = "11",
            Key = "test-game",
            Name = "Game 1",
            Price = 10,
            Discount = 0,
        };

        var parentComment = new Comment
        {
            Id = "idMain",
            Name = "TestName",
            Body = "BodyTest",
            ChildComments = new List<Comment>(),
            CommentId = null,
            Game = game,
        };

        var user = new User
        {
            Id = "50",
            Name = "Jake",
            Password = "123",
            NotificationId = "76",
            Roles = new List<Role>(),
        };
        var userBanned = new User
        {
            Id = "55",
            Name = "Paul",
            Password = "123",
            NotificationId = "76",
            Roles = new List<Role>(),
        };
        var banHistory = new BanHistory
        {
            Id = "1",
            User = userBanned,
            BanStarted = DateTime.UtcNow,
            BanEnds = DateTime.Now.AddHours(1),
            UserId = "55",
        };

        _dbContext.Users.AddRange(user, userBanned);
        _dbContext.BanHistories.Add(banHistory);
        _dbContext.Comments.Add(parentComment);
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        var quoteComment = new AddCommentDto
        {
            Comment = new Comment
            {
                Name = "Jake",
                Body = "Test reply body 2",
            },
            ParentId = parentComment.Id,
            Action = "Quote",
        };

        var claims = new List<Claim> { new(ClaimTypes.Name, "Jake") };
        var claimsIdentity = new ClaimsIdentity(claims);
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(p => p.Identity).Returns(claimsIdentity);
        _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(mockPrincipal.Object);

        // Act
        var comment = await _gameRepository.AddCommentToGame(game.Key, quoteComment);

        // Assert
        Assert.NotNull(comment);
        Assert.Equal(quoteComment.Comment.Name, comment.Name);
        Assert.Equal($"<i>{parentComment.Body}</i><br>{quoteComment.Comment.Body}", comment.Body);
    }

    [Fact]
    public async Task DeleteComment_ChangesCommentAndQuotesBody_WhenCommentDeleted()
    {
        // Arrange
        var commentId = "123";
        var originalCommentBody = "Original comment body";
        var quotedCommentBody = $"<i>{originalCommentBody}</i><br>Comment comment.";

        var parentComment = new Comment
        {
            Id = commentId,
            Name = "Comment main",
            Body = originalCommentBody,
            ChildComments = new List<Comment>
            {
                new()
                {
                    Id = "456",
                    Name = "Comment children",
                    Body = quotedCommentBody,
                    CommentId = commentId,
                },
            },
        };

        _dbContext.Comments.Add(parentComment);
        await _dbContext.SaveChangesAsync();

        // Act
        var deletedComment = await _gameRepository.DeleteComment(commentId);

        // Assert
        Assert.NotNull(deletedComment);
        Assert.Equal("A comment/quote was deleted", deletedComment.Body);
        Assert.Single(deletedComment.ChildComments);
        Assert.Equal("<i>A comment/quote was deleted</i><br>Comment comment.", deletedComment.ChildComments.First().Body);
    }

    [Fact]
    public async Task DeleteComment_ThrowsException_WhenCommentNotFound()
    {
        // Arrange
        var nonExistentCommentId = "NonExistentId";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _gameRepository.DeleteComment(nonExistentCommentId));
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