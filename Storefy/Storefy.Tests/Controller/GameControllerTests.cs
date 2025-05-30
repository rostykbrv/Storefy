using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storefy.API.Controllers;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.Tests.Controller;
public class GameControllerTests
{
    private readonly GameController _controller;
    private readonly Mock<IGameService> _gameService;
    private readonly Mock<IBlobService> _blobService;
    private readonly Mock<ILogger<GameController>> _loggerMock;

    public GameControllerTests()
    {
        _loggerMock = new Mock<ILogger<GameController>>();
        _gameService = new Mock<IGameService>();
        _blobService = new Mock<IBlobService>();
        _controller = new GameController(
            _gameService.Object,
            _loggerMock.Object,
            _blobService.Object);
    }

    [Fact]
    public async Task CreateGame_ValidData_ReturnsSuccess()
    {
        // Arrange
        var testGame = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Id = "123",
                Key = "test-game",
                Name = "Test Game",
                Price = 50,
                UnitInStock = 5,
                Discontinued = 0,
                Description = "Description",
            },
            Genres = new List<string> { "ActionId", "AdventureId" },
            Platforms = new List<string> { "WindowsId", "ConsoleId" },
            Publisher = "Sony",
        };

        var expectedGame = new Game
        {
            Id = testGame.Game.Id,
            Key = testGame.Game.Key,
            Name = testGame.Game.Name,
            Price = testGame.Game.Price,
            Description = testGame.Game.Description,
            UnitInStock = testGame.Game.UnitInStock,
            Discontinued = testGame.Game.Discontinued,
            Genres = new List<Genre> { new() { Id = "ActionId" }, new() { Id = "AdventureId" } },
            Platforms = new List<Platform> { new() { Type = "WindowsId" }, new() { Type = "ConsoleId" } },
            Publisher = new Publisher { CompanyName = "Sony" },
        };

        _gameService.Setup(s => s.CreateGame(testGame)).ReturnsAsync(expectedGame);

        // Act
        var result = await _controller.CreateGame(testGame);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedGame = Assert.IsType<Game>(createdAtResult.Value);

        Assert.Equal(expectedGame.Id, returnedGame.Id);
        Assert.Equal(expectedGame.Name, returnedGame.Name);
        Assert.Equal(expectedGame.Description, returnedGame.Description);
        Assert.Equal(expectedGame.Key, returnedGame.Key);
        Assert.Equal(expectedGame.Platforms.Count, returnedGame.Platforms.Count);
        Assert.Equal(expectedGame.Genres.Count, returnedGame.Genres.Count);
        Assert.Equal(expectedGame.PublisherId, returnedGame.PublisherId);
    }

    [Fact]
    public async Task CreateGame_ValidData_MissingGameAlias_ReturnsSuccess()
    {
        // Arrange
        var testGame = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Id = "123",
                Key = string.Empty,
                Name = "Test Game",
                Price = 50,
                UnitInStock = 5,
                Discontinued = 0,
                Description = "Description",
            },
            Genres = new List<string> { "ActionId", "AdventureId" },
            Platforms = new List<string> { "WindowsId", "ConsoleId" },
            Publisher = "Sony",
        };

        var expectedGame = new Game
        {
            Id = testGame.Game.Id,
            Key = "test-game",
            Name = testGame.Game.Name,
            Price = testGame.Game.Price,
            Description = testGame.Game.Description,
            UnitInStock = testGame.Game.UnitInStock,
            Discontinued = testGame.Game.Discontinued,
            Genres = new List<Genre> { new() { Id = "ActionId" }, new() { Id = "AdventureId" } },
            Platforms = new List<Platform> { new() { Type = "WindowsId" }, new() { Type = "ConsoleId" } },
            Publisher = new Publisher { CompanyName = "Sony" },
        };

        _gameService.Setup(s => s.CreateGame(testGame)).ReturnsAsync(expectedGame);

        // Act
        var result = await _controller.CreateGame(testGame);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedGame = Assert.IsType<Game>(createdAtResult.Value);

        Assert.Equal(expectedGame.Id, returnedGame.Id);
        Assert.Equal(expectedGame.Name, returnedGame.Name);
        Assert.Equal(expectedGame.Description, returnedGame.Description);
        Assert.Equal(expectedGame.Key, returnedGame.Key);
        Assert.Equal(expectedGame.Platforms.Count, returnedGame.Platforms.Count);
        Assert.Equal(expectedGame.Genres.Count, returnedGame.Genres.Count);
    }

    [Fact]
    public async Task GetGameImage_ExistingBlob_ReturnsBlobAsStream()
    {
        // Arrange
        string gameAlias = "test-game";
        var stream = new MemoryStream();

        _blobService.Setup(s => s.GetBlobAsync(gameAlias, "web"))
            .ReturnsAsync(stream);

        // Act
        var result = await _controller.GetGameImage(gameAlias);

        // Assert
        var fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal(stream, fileResult.FileStream);
        Assert.Equal("image/jpeg", fileResult.ContentType);
    }

    [Fact]
    public async Task GetGameImage_NonExistingBlob_ReturnsNotFound()
    {
        // Arrange
        string gameAlias = "unexistent-game";

        _blobService.Setup(s => s.GetBlobAsync(gameAlias, "web"))
            .Returns(Task.FromResult<Stream>(null));

        // Act
        var result = await _controller.GetGameImage(gameAlias);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateGame_InValidData_ReturnsBadRequest()
    {
        // Arrange
        var testGame = new CreateUpdateGameDto
        {
            Game = new GameDto
            {
                Name = null,
                Description = "Fortnite is a player-versus-environment cooperative game",
                Key = "fortnite",
            },
        };

        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.CreateGame(testGame);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllGames_ValidFilterOption_ReturnsOkResult()
    {
        // Arrange
        var filters = new FilterOptions
        {
            Trigger = "ApplyFilters",
            Page = 1,
        };

        var gamePageResult = new GamePageResult
        {
            Games = new Game[2],
            CurrentPage = 1,
            TotalPages = 1,
        };

        var languageCode = "en";

        _gameService.Setup(s => s.GetAllGames(filters, languageCode)).ReturnsAsync(gamePageResult);

        // Act
        var result = await _controller.GetAllGames(languageCode, filters);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGamePageResult = Assert.IsType<GamePageResult>(okResult.Value);

        Assert.Equal(gamePageResult.CurrentPage, returnedGamePageResult.CurrentPage);
        Assert.Equal(gamePageResult.TotalPages, returnedGamePageResult.TotalPages);
    }

    [Fact]
    public void GetSortingOptions_ReturnsOkResult()
    {
        // Arrange
        var sortingOptions = new string[] { "option1", "option2" };
        _gameService.Setup(s => s.GetSortingOptions()).Returns(sortingOptions);

        // Act
        var result = _controller.GetSortingOptions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnSortingOptions = Assert.IsType<string[]>(okResult.Value);
        Assert.Equal(sortingOptions, returnSortingOptions);
    }

    [Fact]
    public async Task GetAllGames_WithoutFilters_ReturnsAllGames()
    {
        // Arrange
        var games = new List<Game>()
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Key = "testgame1",
                Name = "TestGame1",
                Price = 10,
                Description = "Test description",
                UnitInStock = 100,
                Discontinued = 0,
                Genres = new List<Genre>(),
                Platforms = new List<Platform>(),
                Publisher = new Publisher(),
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Key = "testgame2",
                Name = "TestGame2",
                Price = 10,
                Description = "Test description",
                UnitInStock = 100,
                Discontinued = 0,
                Genres = new List<Genre>(),
                Platforms = new List<Platform>(),
                Publisher = new Publisher(),
            },
        };

        _gameService.Setup(g => g.GetAll()).ReturnsAsync(games);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGames = Assert.IsType<List<Game>>(okResult.Value);
        Assert.Equal(2, returnedGames.Count);
    }

    [Fact]
    public void GetPublishedDateOptions_ReturnsOkResult()
    {
        // Arrange
        var dateOptions = new string[] { "option1", "option2" };
        _gameService.Setup(s => s.GetPublishedDateOptions()).Returns(dateOptions);

        // Act
        var result = _controller.GetPublishedDateOptions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnDateOptions = Assert.IsType<string[]>(okResult.Value);
        Assert.Equal(dateOptions, returnDateOptions);
    }

    [Fact]
    public void GetPagingOptions_ReturnsOkResult()
    {
        // Arrange
        var pagingOptions = new int[] { 10, 20, 30, 40, 50 };
        _gameService.Setup(s => s.GetPagingOptions()).Returns(pagingOptions);

        // Act
        var result = _controller.GetPagingOptions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnPagingOptions = Assert.IsType<int[]>(okResult.Value);
        Assert.Equal(pagingOptions, returnPagingOptions);
    }

    [Fact]
    public async Task GetGameDetailedDescription_EmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var gameAlias = string.Empty;
        var languageCode = "en";

        // Act
        var result = await _controller.GetGameDetailedDescription(gameAlias, languageCode);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Game alias should not be empty.", badRequestResult.Value);
    }

    [Fact]
    public async Task GetGameDetailedDescription_GameNotFound_ReturnsNotFound()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "test-game";
        _gameService.Setup(s => s.GetGameByAlias(gameAlias, languageCode))
            .Returns(Task.FromResult<Game>(null));

        // Act
        var result = await _controller.GetGameDetailedDescription(gameAlias, languageCode);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Game with alias '{gameAlias}' not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task GetGameDetailedDescription_ValidAlias_ReturnsGame()
    {
        // Arrange
        var gameAlias = "test-game";
        var languageCode = "en";
        var game = new Game { Id = "123" };

        _gameService.Setup(s => s.GetGameByAlias(gameAlias, languageCode)).ReturnsAsync(game);

        // Act
        var result = await _controller.GetGameDetailedDescription(gameAlias, languageCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGame = Assert.IsType<Game>(okResult.Value);
        Assert.Equal(game.Id, returnedGame.Id);
    }

    [Fact]
    public async Task UpdateGame_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var updatedGame = new CreateUpdateGameDto();
        _controller.ModelState.AddModelError("Value", "Error");

        // Act
        var result = await _controller.UpdateGame(updatedGame);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateGame_ValidData_ReturnsUpdatedGame()
    {
        // Arrange
        var updatedGame = new CreateUpdateGameDto()
        {
            Game = new GameDto
            {
                Id = "123",
                Key = "key",
                Name = "Test Game",
                Price = 50,
                UnitInStock = 5,
                Discontinued = 0,
            },
        };
        var game = new Game()
        {
            Id = "123",
            Key = "key",
            Name = "Test Game",
            Price = 50,
            UnitInStock = 5,
            Discontinued = 0,
        };
        _gameService.Setup(service => service.UpdateGame(updatedGame)).ReturnsAsync(game);

        // Act
        var result = await _controller.UpdateGame(updatedGame);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGame = Assert.IsType<Game>(okResult.Value);
        Assert.Equal(game.Id, returnedGame.Id);
    }

    [Fact]
    public async Task DeleteGame_ValidAlias_ReturnsDeletedGame()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "test-game";
        var game = new Game()
        {
            Id = "123",
            Key = gameAlias,
            Name = "Test Game",
            Price = 50,
            UnitInStock = 5,
            Discontinued = 0,
        };
        _gameService.Setup(service => service.DeleteGame(gameAlias, languageCode)).ReturnsAsync(game);

        // Act
        var result = await _controller.DeleteGame(gameAlias, languageCode);

        // Assert
        var returnedGame = Assert.IsType<Game>(result.Value);
        Assert.Equal(game.Id, returnedGame.Id);
    }

    [Fact]
    public async Task DownloadGame_ValidAlias_ReturnsFileResult()
    {
        // Arrange
        var gameAlias = "test-game";
        var languageCode = "en";
        var fileResult = new BusinessObjects.Models.GameStoreSql
            .FileResult(Encoding.UTF8.GetBytes("Test Content"), "text/plain", "TestFile.txt");
        _gameService.Setup(service => service.DownloadGame(gameAlias, languageCode)).ReturnsAsync(fileResult);

        // Act
        var result = await _controller.DownloadGame(gameAlias, languageCode);

        // Assert
        var fileResultFromController = Assert.IsType<FileContentResult>(result);
        Assert.Equal(fileResult.FileBytes, fileResultFromController.FileContents);
        Assert.Equal(fileResult.ContentType, fileResultFromController.ContentType);
        Assert.Equal(fileResult.FileName, fileResultFromController.FileDownloadName);
    }

    [Fact]
    public async Task GetGamesByGenre_ValidGenre_ReturnsOk()
    {
        // Arrange
        var genreId = "1";
        var expectedGames = new List<Game>
        {
            new()
            {
                Id = "1",
                Name = "Test name",
                Description = "Test description",
                Key = "test-name",
                Platforms = new List<Platform> { new() { Id = "1" } },
                Genres = new List<Genre> { new() { Id = "1" } },
            },
            new()
            {
                Id = "2",
                Name = "Test name",
                Description = "Test description",
                Key = "test-name-2",
                Platforms = new List<Platform> { new() { Id = "1" } },
                Genres = new List<Genre> { new() { Id = "1" } },
            },
        };

        _gameService.Setup(s => s.GetGamesByGenre(genreId)).ReturnsAsync(expectedGames);

        // Act
        var result = await _controller.GetGamesByGenre(genreId);

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGames = Assert.IsType<List<Game>>(returnedResult.Value);

        Assert.Equal(expectedGames.Count, returnedGames.Count);
    }

    [Fact]
    public async Task GetGamesByGenre_InvalidGenre_ReturnsNoContent()
    {
        // Arrange
        var genreId = "33";

        _gameService.Setup(s => s.GetGamesByGenre(genreId)).ReturnsAsync(new List<Game>());

        // Act
        var result = await _controller.GetGamesByGenre(genreId);

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task GetGamesByPlatform_ValidGenre_ReturnsOk()
    {
        // Arrange
        var platformId = "1";
        var expectedGames = new List<Game>
        {
            new()
            {
                Id = "1",
                Name = "Test name",
                Description = "Test description",
                Key = "test-name",
                Platforms = new List<Platform> { new() { Id = "1" } },
                Genres = new List<Genre> { new() { Id = "1" } },
            },
            new()
            {
                Id = "2",
                Name = "Test name",
                Description = "Test description",
                Key = "test-name-2",
                Platforms = new List<Platform> { new() { Id = "1" } },
                Genres = new List<Genre> { new() { Id = "2" } },
            },
        };

        _gameService.Setup(s => s.GetGamesByPlatform(platformId)).ReturnsAsync(expectedGames);

        // Act
        var result = await _controller.GetGamesByPlatform(platformId);

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGames = Assert.IsType<List<Game>>(returnedResult.Value);

        Assert.Equal(expectedGames.Count, returnedGames.Count);
    }

    [Fact]
    public async Task GetGamesByPlatform_InvalidGenre_ReturnsNoContent()
    {
        // Arrange
        var platformId = "33";

        _gameService.Setup(s => s.GetGamesByPlatform(platformId)).ReturnsAsync(new List<Game>());

        // Act
        var result = await _controller.GetGamesByPlatform(platformId);

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task GetGamesByPublisher_ValidCompanyName_ReturnsOk()
    {
        // Arrange
        var companyName = "Sony";
        var expectedGames = new List<Game>
        {
            new()
            {
                Id = "1",
                Name = "Test name",
                Description = "Test description",
                Key = "test-name",
                Platforms = new List<Platform> { new() { Id = "1" } },
                Genres = new List<Genre> { new() { Id = "1" } },
                Publisher = new Publisher { Id = "1", CompanyName = "Sony" },
            },
            new()
            {
                Id = "2",
                Name = "Test name",
                Description = "Test description",
                Key = "test-name-2",
                Platforms = new List<Platform> { new() { Id = "1" } },
                Genres = new List<Genre> { new() { Id = "2" } },
                Publisher = new Publisher { Id = "1", CompanyName = "Sony" },
            },
        };

        _gameService.Setup(s => s.GetGamesByPublisher(companyName)).ReturnsAsync(expectedGames);

        // Act
        var result = await _controller.GetGamesByPublisher(companyName);

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGames = Assert.IsType<List<Game>>(returnedResult.Value);

        Assert.Equal(expectedGames.Count, returnedGames.Count);
    }

    [Fact]
    public async Task GetGamesByPublisher_InvalidCompanyName_ReturnsNoContent()
    {
        // Arrange
        var companyName = "superCo";

        _gameService.Setup(s => s.GetGamesByPublisher(companyName)).ReturnsAsync(new List<Game>());

        // Act
        var result = await _controller.GetGamesByPublisher(companyName);

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task AddGameToCart_ReturnsOrder()
    {
        // Arrange
        var gameAlias = "test-game";
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            OrderDate = DateTime.Now,
            PaidDate = null,
            Status = OrderStatus.Open,
            CustomerId = Guid.NewGuid().ToString(),
            Sum = 20,
            OrderDetails = new List<OrderDetails>
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Price = 12,
                    Discount = 0,
                    CreationDate = DateTime.Now,
                    ProductId = "123",
                    Quantity = 1,
                    ProductName = "Game",
                    Game = new Game
                    {
                        Id = "123",
                        Name = "Test Game",
                        Key = "test-game",
                        UnitInStock = 10,
                        Description = "description",
                        Discontinued = 0,
                        Price = 12,
                    },
                },
            },
        };

        _gameService.Setup(s => s.BuyGame(gameAlias)).ReturnsAsync(order);

        // Act
        var result = await _controller.CreateGameOrder(gameAlias);

        // Assert
        var returnedResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrder = Assert.IsType<Order>(returnedResult.Value);

        Assert.Equal(order.Id, returnedOrder.Id);
        Assert.Equal(order.OrderDetails.Count, returnedOrder.OrderDetails.Count);
        Assert.Equal(order.Status, returnedOrder.Status);
        Assert.Equal(order.Sum, returnedOrder.Sum);
        Assert.Equal(order.CustomerId, returnedOrder.CustomerId);
        Assert.Equal(order.PaidDate, returnedOrder.PaidDate);
    }

    [Fact]
    public async Task GetGameById_EmptyId_ReturnsBadRequest()
    {
        // Arrange
        var id = string.Empty;

        // Act
        var result = await _controller.GetGameById(id);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Game id should not be empty.", badRequestResult.Value);
    }

    [Fact]
    public async Task GetGameById_GameNotFound_ReturnsNotFound()
    {
        // Arrange
        var id = "123";
        _gameService.Setup(s => s.GetGameById(id)).Returns(Task.FromResult<Game>(null));

        // Act
        var result = await _controller.GetGameById(id);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Game with id '{id}' not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task GetGameById_ValidId_ReturnsGame()
    {
        // Arrange
        var id = "123";
        var game = new Game { Id = "123" };
        _gameService.Setup(s => s.GetGameById(id)).ReturnsAsync(game);

        // Act
        var result = await _controller.GetGameById(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGame = Assert.IsType<Game>(okResult.Value);
        Assert.Equal(game.Id, returnedGame.Id);
    }

    [Fact]
    public async Task GetComments_ReturnsOk_ContainsListOfComments()
    {
        // Arrange
        var gameAlias = "game-test";
        var game = new Game { Id = "123", Key = gameAlias, };
        var expectedComments = new List<Comment>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Comment UserName",
                Body = "Body comment",
                ChildComments = new List<Comment>(),
                CommentId = null,
                Game = game,
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Comment UserName2",
                Body = "Body comment2",
                ChildComments = new List<Comment>(),
                CommentId = null,
                Game = game,
            },
        };
        _gameService.Setup(s => s.GetComments(gameAlias)).ReturnsAsync(expectedComments);

        // Act
        var result = await _controller.GetComments(gameAlias);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnComments = Assert.IsType<List<Comment>>(okResult.Value);
        Assert.Equal(expectedComments.Count, returnComments.Count);
    }

    [Fact]
    public async Task AddCommentToGame_Ok_ReturnsCreatedComment()
    {
        // Arrange
        var gameAlias = "test-game";
        var game = new Game { Id = "123", Key = gameAlias, };
        var commentDto = new AddCommentDto
        {
            Action = string.Empty,
            Comment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Comment UserName",
                Body = "Body comment",
                ChildComments = new List<Comment>(),
                CommentId = null,
                Game = game,
            },
            ParentId = null,
        };

        var returnedComment = new Comment
        {
            Id = Guid.NewGuid().ToString(),
            Name = commentDto.Comment.Name,
            Body = commentDto.Comment.Body,
            ChildComments = commentDto.Comment.ChildComments,
            CommentId = commentDto.Comment.CommentId,
            Game = commentDto.Comment.Game,
        };

        _gameService.Setup(s => s.AddCommentToGame(gameAlias, commentDto)).ReturnsAsync(returnedComment);

        // Act
        var result = await _controller.AddCommentToGame(gameAlias, commentDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var resultComment = Assert.IsType<Comment>(createdAtActionResult.Value);
        Assert.Equal(returnedComment.Id, resultComment.Id);
        Assert.Equal(returnedComment.Name, resultComment.Name);
        Assert.Equal(returnedComment.Body, resultComment.Body);
    }

    [Fact]
    public async Task DeleteComment_ReturnsComment_WhenCommentDeleted()
    {
        // Arrange
        var commentId = "test-comment";
        var expectedComment = new Comment
        {
            Id = commentId,
            Name = "Test Name",
            Body = "Test comment body",
        };

        _gameService.Setup(s => s.DeleteComment(commentId)).ReturnsAsync(expectedComment);

        // Act
        var result = await _controller.DeleteComment(commentId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        var commentResult = Assert.IsType<Comment>(actionResult.Value);
        Assert.Equal(expectedComment.Id, commentResult.Id);
        Assert.Equal(expectedComment.Name, commentResult.Name);
        Assert.Equal(expectedComment.Body, commentResult.Body);
    }
}
