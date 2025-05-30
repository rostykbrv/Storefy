using System.Text;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Services.Services;

namespace Storefy.Tests.Services.Services;
public class GameServiceTests
{
    private readonly GameService _gameService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public GameServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _gameService = new GameService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateGame_GeneratesUniqueGameAlias_WhenCreatingGame()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Game",
                Key = string.Empty,
            },
        };
        _unitOfWorkMock.Setup(uow => uow.GameRepository.Add(It.IsAny<CreateUpdateGameDto>()))
            .ReturnsAsync((CreateUpdateGameDto g) => new Game { Name = g.Game.Name, Key = g.Game.Key });
        _unitOfWorkMock.Setup(uow => uow.GameRepository.AliasExists(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var createdGame = await _gameService.CreateGame(gameDto);

        // Assert
        Assert.NotNull(createdGame.Key);
        Assert.False(string.IsNullOrEmpty(createdGame.Key));
        Assert.Equal("test-game", createdGame.Key);
    }

    [Fact]
    public async Task CreateGame_ShouldAddPublisher_IfItDoesNotAlreadyExist()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Game",
                Key = "test-game",
            },
            Publisher = "123",
        };

        var game = new Game
        {
            Name = gameDto.Game.Name,
            Key = gameDto.Game.Key,
        };

        _unitOfWorkMock.Setup(uow => uow.GameRepository
        .AliasExists(It.IsAny<string>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.PublisherRepository
        .GetPublisherById(gameDto.Publisher))
            .ReturnsAsync(() => null);
        _unitOfWorkMock.Setup(x => x.GameRepository
        .Add(gameDto))
            .ReturnsAsync(game);

        // Act
        await _gameService.CreateGame(gameDto);

        // Assert
        _unitOfWorkMock.Verify(x => x.PublisherRepository.AddPublisher(It.IsAny<CreateUpdatePublisherDto>()), Times.Once);
    }

    [Fact]
    public async Task CreateGame_ShouldNotAddPublisher_IfItAlreadyExists()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Game",
                Key = "test-game",
            },
            Publisher = "123",
        };

        var existingPublisher = new Publisher
        {
            CompanyName = "ExistingCompany",
        };

        _unitOfWorkMock
            .Setup(x => x.PublisherRepository.GetPublisherById(gameDto.Publisher))
            .ReturnsAsync(existingPublisher);

        _unitOfWorkMock.Setup(u => u.GameRepository.AliasExists(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        await _gameService.CreateGame(gameDto);

        // Assert
        _unitOfWorkMock.Verify(x => x.PublisherRepository.AddPublisher(It.IsAny<CreateUpdatePublisherDto>()), Times.Never);
        Assert.Equal(existingPublisher.CompanyName, gameDto.PublisherName);
    }

    [Fact]
    public async Task CreateGame_ShouldGetGenre_IfItAlreadyExistsInSqlDatabase()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Game",
                Key = "test-game",
            },
            Genres = new List<string> { "action" },
        };

        var genreSql = new Genre
        {
            Id = "action",
            Name = "Action",
        };

        _unitOfWorkMock.Setup(u => u.GameRepository.AliasExists(It.IsAny<string>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.GenreRepository.GetById("action"))
            .ReturnsAsync(genreSql);

        // Act
        await _gameService.CreateGame(gameDto);

        // Assert
        Assert.Contains("Action", gameDto.GenreNames);
        _unitOfWorkMock.Verify(x => x.GenreRepository.Add(It.IsAny<CreateUpdateGenreDto>()), Times.Never);
    }

    [Fact]
    public async Task CreateGame_ShouldAddGenre_IfItDoesNotAlreadyExistsInSqlDatabase()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Game",
                Key = "test-game",
            },
            Genres = new List<string> { "action" },
        };

        _unitOfWorkMock.Setup(u => u.GameRepository.AliasExists(It.IsAny<string>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.GenreRepository.GetById("action"))
            .ThrowsAsync(new Exception());

        // Act
        await _gameService.CreateGame(gameDto);

        // Assert
        _unitOfWorkMock.Verify(x => x.GenreRepository.Add(It.IsAny<CreateUpdateGenreDto>()), Times.Once);
        Assert.Contains("Action", gameDto.GenreNames);
    }

    [Fact]
    public async Task GetGameByAlias_ReturnsGameWithMatchingAlias()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "game-1";
        var game = new Game { Id = Guid.NewGuid().ToString(), Name = "Game 1", Key = gameAlias };
        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetByAlias(gameAlias, languageCode))
            .ReturnsAsync(game);

        // Act
        var result = await _gameService.GetGameByAlias(gameAlias, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameAlias, result.Key);
    }

    [Fact]
    public async Task GetAllGames_ReturnsListOfAllGames()
    {
        // Arrange
        var games = new List<Game>
        {
            new() { Id = Guid.NewGuid().ToString(), Name = "Game 1", Key = "game-1" },
            new() { Id = Guid.NewGuid().ToString(), Name = "Game 2", Key = "game-2" },
            new() { Id = Guid.NewGuid().ToString(), Name = "Game 3", Key = "game-3" },
        };
        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetAll())
            .ReturnsAsync(games);

        // Act
        var result = await _gameService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count());
    }

    [Fact]
    public async Task GetAllGames_ValidFilterOption_ReturnsExpectedGames()
    {
        // Arrange
        var languageCode = "en";
        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
        };

        var games = new[]
        {
            new Game { Id = Guid.NewGuid().ToString(), Name = "Game 1", Key = "game-1" },
            new Game { Id = Guid.NewGuid().ToString(), Name = "Game 2", Key = "game-2" },
        };
        var gamePageResult = new GamePageResult
        {
            Games = games,
            CurrentPage = 1,
            TotalPages = 1,
        };

        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetAllGames(filters, languageCode)).ReturnsAsync(gamePageResult);

        var products = new[]
        {
             new Game { Id = Guid.NewGuid().ToString(), Name = "Game 4", Key = "game-4" },
             new Game { Id = Guid.NewGuid().ToString(), Name = "Game 5", Key = "game-5" },
        };
        var gamePageResultMongo = new GamePageResult
        {
            Games = products,
            CurrentPage = 1,
            TotalPages = 1,
        };

        // Act
        var result = await _gameService.GetAllGames(filters, languageCode);

        // Assert
        var returnedTotalPages = gamePageResult.TotalPages + gamePageResultMongo.TotalPages;
        var returnedTotalGames = gamePageResult.Games.Length + gamePageResultMongo.Games.Length;
        Assert.Equal(0, result.CurrentPage);
        Assert.Equal(returnedTotalPages, result.TotalPages);
        Assert.Equal(returnedTotalGames, result.Games.Length);
    }

    [Fact]
    public void GetSortingOptions_ReturnsExpectedSortingOptions()
    {
        // Act
        var result = _gameService.GetSortingOptions();

        // Assert
        Assert.Equal(new string[] { "Most popular (most viewed)", "Most commented", "Price ASC", "Price DESC", "New (by date)" }, result);
    }

    [Fact]
    public void GetPublishedDateOptions_ReturnsExpectedDateOptions()
    {
        // Act
        var result = _gameService.GetPublishedDateOptions();

        // Assert
        Assert.Equal(new string[] { "Last Week", "Last Month ", "Last Year", "2 Years", "3 Years" }, result);
    }

    [Fact]
    public void GetPagingOptions_ReturnsExpectedPagingOptions()
    {
        // Act
        var result = _gameService.GetPagingOptions();

        // Assert
        Assert.Equal(new int[] { 10, 20, 30, 50, 100 }, result);
    }

    [Fact]
    public async Task DeleteGame_DeletesGameWithSpecifiedAlias()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "test-game";
        var game = new Game { Id = Guid.NewGuid().ToString(), Name = "Test Game", Key = gameAlias };

        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetByAlias(gameAlias, languageCode))
            .ReturnsAsync(game);

        _unitOfWorkMock.Setup(uow => uow.GameRepository.Delete(game))
            .ReturnsAsync(game);

        // Act
        var deletedGame = await _gameService.DeleteGame(gameAlias, languageCode);

        // Assert
        Assert.NotNull(deletedGame);
        Assert.Equal(gameAlias, deletedGame.Key);
    }

    [Fact]
    public async Task DeleteGame_ReturnsNull_WhenAliasNotFound()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "non-existing-game";

        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetByAlias(gameAlias, languageCode))
            .Returns(Task.FromResult<Game>(null));

        // Act
        var deletedGame = await _gameService.DeleteGame(gameAlias, languageCode);

        // Assert
        Assert.Null(deletedGame);
    }

    [Fact]
    public async Task DownloadGame_GameExists_ReturnsGameFileResult()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "game1";
        var genres = new List<Genre> { new() { Id = Guid.NewGuid().ToString(), Name = "genre1" } };
        var platforms = new List<Platform> { new() { Id = Guid.NewGuid().ToString(), Type = "platform1" } };
        var publisher = new Publisher { Id = Guid.NewGuid().ToString(), CompanyName = "Publisher1" };
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Key = gameAlias,
            Name = "Game1",
            Genres = genres,
            Platforms = platforms,
            Publisher = publisher,
        };

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByAlias(gameAlias, languageCode)).ReturnsAsync(game);

        // Act
        var result = await _gameService.DownloadGame(gameAlias, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FileResult>(result);
        Assert.Equal("text/plain", result.ContentType);
        Assert.Contains("Game1", result.FileName);
    }

    [Fact]
    public async Task DownloadGame_GameExistsAndPublisherIsNull_ReturnsGameFileResult()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "game2";
        var genres = new List<Genre> { new() { Id = Guid.NewGuid().ToString(), Name = "genre1" } };
        var platforms = new List<Platform> { new() { Id = Guid.NewGuid().ToString(), Type = "platform1" } };
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Key = gameAlias,
            Name = "Game2",
            Genres = genres,
            Platforms = platforms,
            Publisher = null,
        };

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByAlias(gameAlias, languageCode)).ReturnsAsync(game);

        // Act
        var result = await _gameService.DownloadGame(gameAlias, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("text/plain", result.ContentType);
        Assert.Contains("Game2", result.FileName);
        Assert.Contains("Publisher: ", Encoding.UTF8.GetString(result.FileBytes));
    }

    [Fact]
    public async Task UpdateGame_UpdatesTheGameInformation()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test",
                Key = "test",
            },
        };
        _unitOfWorkMock.Setup(uow => uow.GameRepository.Update(gameDto))
            .ReturnsAsync(new Game { Name = gameDto.Game.Name, Key = gameDto.Game.Key });

        // Act
        var updatedGame = await _gameService.UpdateGame(gameDto);

        // Assert
        Assert.NotNull(updatedGame);
        Assert.Equal(gameDto.Game.Name, updatedGame.Name);
        Assert.Equal(gameDto.Game.Key, updatedGame.Key);
    }

    [Fact]
    public async Task UpdateGame_GameDoesNotExistInGameRepositoryButExistInProductRepository_ReturnsUpdatedGame()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Game",
                Key = "test-game",
            },
        };
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Updated Game",
            Key = "updated-game",
        };
        _unitOfWorkMock.Setup(uow => uow.GameRepository.Update(It.IsAny<CreateUpdateGameDto>())).Throws<Exception>();
        _unitOfWorkMock.Setup(uow => uow.GameRepository.Add(It.IsAny<CreateUpdateGameDto>())).ReturnsAsync(game);

        // Act
        var result = await _gameService.UpdateGame(gameDto);

        // Assert
        Assert.Equal(game, result);
    }

    [Fact]
    public async Task UpdateGame_GameDoesNotExistInBothRepositories_ThrowsInvalidOperationException()
    {
        // Arrange
        var gameDto = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Game",
                Key = "test-game",
            },
        };
        _unitOfWorkMock.Setup(uow => uow.GameRepository.Update(It.IsAny<CreateUpdateGameDto>())).Throws<Exception>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _gameService.UpdateGame(gameDto));
    }

    [Fact]
    public async Task GetGamesByGenre_ReturnsGamesWithSpecifiedGenre()
    {
        // Arrange
        var genreId = "123";
        var games = new List<Game>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Game 1",
                Key = "game-1",
                Genres = new List<Genre>
                {
                    new() { Id = genreId, Name = "TestGenre" },
                },
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Game 2",
                Key = "game-2",
                Genres = new List<Genre>
                {
                    new() { Id = genreId, Name = "TestGenre" },
                },
            },
        };
        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetGamesByGenre(genreId))
            .ReturnsAsync(games);

        // Act
        var result = await _gameService.GetGamesByGenre(genreId);

        // Assert
        var gamesCount = games.Count;
        Assert.NotNull(result);
        Assert.Equal(gamesCount, result.Count());
        Assert.True(result.All(game => game.Genres.Any(g => g.Id == genreId)));
    }

    [Fact]
    public async Task GetGamesByPlatform_ReturnsGamesWithSpecifiedPlatform()
    {
        // Arrange
        var platformId = "5";
        var games = new List<Game>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Game 1",
                Key = "game-1",
                Platforms = new List<Platform>
                {
                    new() { Id = platformId, Type = "TestPlatform" },
                },
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Game 2",
                Key = "game-2",
                Platforms = new List<Platform>
                {
                    new() { Id = platformId, Type = "TestPlatform" },
                },
            },
        };
        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetGamesByPlatform(platformId))
            .ReturnsAsync(games);

        // Act
        var result = await _gameService.GetGamesByPlatform(platformId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(games.Count, result.Count());
        Assert.True(result.All(game => game.Platforms.Any(p => p.Id == platformId)));
    }

    [Fact]
    public async Task GetGamesByPublisher_ReturnsGamesWithSpecifiedPublisher()
    {
        // Arrange
        var publisherName = "TestCompany";
        var games = new List<Game>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Game 1",
                Key = "game-1",
                Publisher = new Publisher
                {
                    Id = Guid.NewGuid().ToString(),
                    CompanyName = publisherName,
                },
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Game 2",
                Key = "game-2",
                Publisher = new Publisher
                {
                    Id = Guid.NewGuid().ToString(),
                    CompanyName = publisherName,
                },
            },
        };
        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetGamesByPublisher(publisherName))
            .ReturnsAsync(games);

        // Act
        var result = await _gameService.GetGamesByPublisher(publisherName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(games.Count, result.Count());
        Assert.Equal(publisherName, result.First().Publisher.CompanyName);
    }

    [Theory]
    [InlineData("Super Mario World", "super-mario-world")]
    [InlineData("", "")]
    [InlineData("Game Title With SPACE", "game-title-with-space")]
    [InlineData("UPPER CASE", "upper-case")]
    public void GenerateGameAlias_GeneratesCorrectAlias(string gameName, string expectedGameAlias)
    {
        // Act
        var gameAlias = GameService.GenerateGameAlias(gameName);

        // Assert
        Assert.Equal(expectedGameAlias, gameAlias);
    }

    [Fact]
    public async Task EnsureUniqueGameAlias_ReturnsOriginalAlias_WhenAliasNotExists()
    {
        // Arrange
        var gameAlias = "unique-game";
        _unitOfWorkMock.Setup(uow => uow.GameRepository.AliasExists(gameAlias))
            .ReturnsAsync(false);

        // Act
        var uniqueGameAlias = await _gameService.EnsureUniqueGameAlias(gameAlias);

        // Assert
        Assert.Equal(gameAlias, uniqueGameAlias);
    }

    [Fact]
    public async Task EnsureUniqueGameAlias_ReturnsModifiedAlias_WhenAliasExists()
    {
        // Arrange
        var gameAlias = "non-unique-game";
        _unitOfWorkMock.Setup(uow => uow.GameRepository.AliasExists(gameAlias))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(uow => uow.GameRepository.AliasExists(gameAlias + "-1"))
            .ReturnsAsync(false);

        // Act
        var uniqueGameAlias = await _gameService.EnsureUniqueGameAlias(gameAlias);

        // Assert
        Assert.Equal(gameAlias + "-1", uniqueGameAlias);
    }

    [Fact]
    public async Task EnsureUniqueGameAlias_ReturnsCorrectlyModifiedAlias_WhenSeveralAliasesExist()
    {
        // Arrange
        var gameAlias = "repeated-game";
        _unitOfWorkMock.Setup(uow => uow.GameRepository.AliasExists(gameAlias))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(uow => uow.GameRepository.AliasExists(gameAlias + "-1"))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(uow => uow.GameRepository.AliasExists(gameAlias + "-2"))
            .ReturnsAsync(false);

        // Act
        var uniqueGameAlias = await _gameService.EnsureUniqueGameAlias(gameAlias);

        // Assert
        Assert.Equal(gameAlias + "-2", uniqueGameAlias);
    }

    [Fact]
    public async Task BuyGame_ReturnsOrder()
    {
        // Arrange
        var gameAlias = "game-alias-1";
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Game 1",
            Key = gameAlias,
        };

        var orderDetails = new OrderDetails
        {
            Id = Guid.NewGuid().ToString(),
            Game = game,
            ProductName = "Game 1",
        };

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            OrderDetails = new List<OrderDetails> { orderDetails },
        };

        _unitOfWorkMock.Setup(uow => uow.GameRepository.BuyGame(gameAlias))
            .ReturnsAsync(order);

        // Act
        var result = await _gameService.BuyGame(gameAlias);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order, result);
    }

    [Fact]
    public async Task GetGameById_ReturnsGameWithMatchingId()
    {
        // Arrange
        var id = "123";
        var game = new Game
        {
            Id = id,
            Key = "game-test",
            Name = "Game Test",
        };

        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetById(id))
            .ReturnsAsync(game);

        // Act
        var result = await _gameService.GetGameById(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(game.Key, result.Key);
    }

    [Fact]
    public async Task GetComments_ReturnsComments()
    {
        // Arrange
        var gameAlias = "game-alias-1";
        var game = new Game
        {
            Id = "idGame",
            Key = gameAlias,
            Name = "Game Test",
        };

        var comments = new List<Comment>
        {
            new()
            {
                Id = "1",
                Name = "Comment name 1",
                Body = "Comment 1",
                Game = game,
                ChildComments = new List<Comment>(),
                CommentId = null,
            },
            new()
            {
                Id = "2",
                Name = "Comment name 2",
                Body = "Comment 2",
                Game = game,
                ChildComments = new List<Comment>(),
                CommentId = null,
            },
        };

        _unitOfWorkMock.Setup(uow => uow.GameRepository.GetComments(gameAlias))
            .ReturnsAsync(comments);

        // Act
        var result = await _gameService.GetComments(gameAlias);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(comments, result);
    }

    [Fact]
    public async Task AddCommentToGame_ReturnsAddedComment()
    {
        // Arrange
        var gameAlias = "game-alias-1";
        var game = new Game
        {
            Id = "idGame",
            Key = gameAlias,
            Name = "Game Test",
        };
        var commentDto = new AddCommentDto
        {
            Comment = new Comment
            {
                Id = "1",
                Name = "Comment",
                Body = "Comment body",
                Game = game,
                ChildComments = new List<Comment>(),
                CommentId = null,
            },
            Action = string.Empty,
            ParentId = null,
        };
        var comment = new Comment
        {
            Id = "1",
            Name = "Comment",
            Body = "Comment body",
            Game = game,
            ChildComments = new List<Comment>(),
            CommentId = null,
        };

        _unitOfWorkMock.Setup(uow => uow.GameRepository.AddCommentToGame(gameAlias, commentDto))
            .ReturnsAsync(comment);

        // Act
        var result = await _gameService.AddCommentToGame(gameAlias, commentDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(comment, result);
    }

    [Fact]
    public async Task DeleteComment_ReturnsDeletedComment()
    {
        // Arrange
        var commentId = "1";
        var comment = new Comment
        {
            Id = commentId,
            Name = "Comment",
            Body = "A comment/quote was deleted",
            ChildComments = new List<Comment>(),
            CommentId = null,
        };

        _unitOfWorkMock.Setup(uow => uow.GameRepository.DeleteComment(commentId))
            .ReturnsAsync(comment);

        // Act
        var result = await _gameService.DeleteComment(commentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(comment.Body, result.Body);
    }
}
