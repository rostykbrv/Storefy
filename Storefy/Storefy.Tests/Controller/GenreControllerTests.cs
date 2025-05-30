using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storefy.API.Controllers;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.Tests.Controller;
public class GenreControllerTests
{
    private readonly GenreController _controller;
    private readonly Mock<IGenreService> _genreService;
    private readonly Mock<ILogger<GenreController>> _loggerMock;

    public GenreControllerTests()
    {
        _loggerMock = new Mock<ILogger<GenreController>>();
        _genreService = new Mock<IGenreService>();
        _controller = new GenreController(_genreService.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateGenre_ValidData_ReturnsSuccess()
    {
        // Arrange
        var testGenre = new CreateUpdateGenreDto
        {
            Genre = new GenreDto
            {
                Id = "123",
                Name = "Action",
                ParentGenreId = "444",
            },
        };

        var expectedGenre = new Genre
        {
            Id = testGenre.Genre.Id,
            Name = testGenre.Genre.Name,
            ParentGenreId = testGenre.Genre.ParentGenreId,
            ParentGenre = new Genre { Id = testGenre.Genre.ParentGenreId, Name = "Parent Genre" },
        };

        _genreService.Setup(s => s.AddGenre(testGenre)).ReturnsAsync(expectedGenre);

        // Act
        var result = await _controller.CreateGenre(testGenre);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedGenre = Assert.IsType<Genre>(createdAtResult.Value);

        Assert.Equal(expectedGenre.Id, returnedGenre.Id);
        Assert.Equal(expectedGenre.Name, returnedGenre.Name);
        Assert.Equal(expectedGenre.ParentGenreId, returnedGenre.ParentGenreId);
        Assert.Equal(expectedGenre.ParentGenre.Id, returnedGenre.ParentGenre.Id);
    }

    [Fact]
    public async Task CreateGenre_InValidData_ReturnsBadRequest()
    {
        // Arrange
        var testGenre = new CreateUpdateGenreDto
        {
            Genre = new GenreDto
            {
                Id = "123",
                Name = null,
                ParentGenreId = "44",
            },
        };

        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.CreateGenre(testGenre);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetGenre_ValidGenreId_ReturnsSuccess()
    {
        // Arrange
        var genreId = "123";
        var expectedGenre = new Genre
        {
            Id = genreId,
            Name = "Action",
            ParentGenreId = "22",
        };

        _genreService.Setup(s => s.GetGenreById(genreId)).ReturnsAsync(expectedGenre);

        // Act
        var result = await _controller.GetGenre(genreId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGenre = Assert.IsType<Genre>(okResult.Value);
        Assert.Equal(expectedGenre.Id, returnedGenre.Id);
        Assert.Equal(expectedGenre.Name, returnedGenre.Name);
        Assert.Equal(expectedGenre.ParentGenreId, returnedGenre.ParentGenreId);
    }

    [Fact]
    public async Task GetGenre_NotFound_ReturnsNotFound()
    {
        // Arrange
        var genreId = "123";
        _genreService.Setup(s => s.GetGenreById(genreId)).Returns(Task.FromResult<Genre>(null));

        // Act
        var result = await _controller.GetGenre(genreId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetGenresByParentGenre_ValidParentGenreId_ReturnsSuccess()
    {
        // Arrange
        var parentGenreId = "123";
        var expectedSubGenres = new List<Genre>
        {
            new()
            {
                Id = "1",
                Name = "SubGenre1",
                ParentGenreId = "22",
            },
            new()
            {
                Id = "2",
                Name = "SubGenre2",
                ParentGenreId = "22",
            },
        };

        _genreService.Setup(s => s.GetGenresByParentGenre(parentGenreId)).ReturnsAsync(expectedSubGenres);

        // Act
        var result = await _controller.GetGenresByParentGenre(parentGenreId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSubGenres = Assert.IsType<List<Genre>>(okResult.Value);
        Assert.Equal(expectedSubGenres.Count, returnedSubGenres.Count);
    }

    [Fact]
    public async Task GetGenresByParentGenre_InValidParentGenreId_ReturnsNotFound()
    {
        // Arrange
        var parentGenreId = "123";
        _genreService.Setup(s => s.GetGenresByParentGenre(parentGenreId)).ReturnsAsync((List<Genre>)null);

        // Act
        var result = await _controller.GetGenresByParentGenre(parentGenreId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateGenre_ValidData_ReturnsSuccess()
    {
        // Arrange
        var testGenre = new CreateUpdateGenreDto
        {
            Genre = new()
            {
                Id = "123",
                Name = "Action",
                ParentGenreId = "22",
            },
        };

        var expectedGenre = new Genre
        {
            Id = testGenre.Genre.Id,
            Name = testGenre.Genre.Name,
            ParentGenreId = testGenre.Genre.ParentGenreId,
        };

        _genreService.Setup(s => s.UpdateGenre(testGenre)).ReturnsAsync(expectedGenre);

        // Act
        var result = await _controller.UpdateGenre(testGenre);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedGenre = Assert.IsType<Genre>(createdAtResult.Value);
        Assert.Equal(expectedGenre.Name, returnedGenre.Name);
    }

    [Fact]
    public async Task UpdateGenre_BadModel_ReturnsBadRequest()
    {
        // Arrange
        var testGenre = new CreateUpdateGenreDto
        {
            Genre = new GenreDto
            {
                ParentGenreId = "22",
            },
        };

        // Make the model state invalid
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.UpdateGenre(testGenre);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteGenre_ValidGenreId_DeletesGenreAndReturnsIt()
    {
        // Arrange
        var genreId = "123";
        var expectedGenre = new Genre
        {
            Id = genreId,
            Name = "Action",
            ParentGenreId = "22",
        };

        _genreService.Setup(s => s.DeleteGenre(genreId)).ReturnsAsync(expectedGenre);

        // Act
        var result = await _controller.DeleteGenre(genreId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGenre = Assert.IsType<Genre>(okResult.Value);
        Assert.Equal(expectedGenre.Id, returnedGenre.Id);
        Assert.Equal(expectedGenre.Name, returnedGenre.Name);
        Assert.Equal(expectedGenre.ParentGenreId, returnedGenre.ParentGenreId);
    }

    [Fact]
    public async Task DeleteGenre_GenreNotFound_ReturnsNotFound()
    {
        // Arrange
        var genreId = "123";
        _genreService.Setup(s => s.DeleteGenre(genreId)).Returns(Task.FromResult<Genre>(null));

        // Act
        var result = await _controller.DeleteGenre(genreId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAllGenres_ReturnsAllGenres()
    {
        // Arrange
        var genres = new List<Genre>
        {
            new()
            {
                Id = "1",
                Name = "Action",
                ParentGenreId = "22",
            },
            new()
            {
                Id = "2",
                Name = "Adventure",
                ParentGenreId = "22",
            },
        };

        _genreService.Setup(s => s.GetAllGenres()).ReturnsAsync(genres);

        // Act
        var result = await _controller.GetAllGenres();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGenres = Assert.IsType<List<Genre>>(okResult.Value);
        Assert.Equal(2, returnedGenres.Count);
    }

    [Fact]
    public async Task GetGenresByGame_ValidGameAlias_ReturnsGenres()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "game1";
        var expectedGenres = new List<Genre>
    {
        new()
        {
            Id = "1",
            Name = "Action",
            ParentGenreId = "22",
            Games = new List<Game>
            {
                new() { Id = "1", Key = "game1", Name = "Game 1" },
            },
        },
        new()
        {
            Id = "2",
            Name = "Adventure",
            ParentGenreId = "22",
            Games = new List<Game>
            {
                new() { Id = "1", Key = "game1", Name = "Game 1" },
            },
        },
    };

        _genreService.Setup(s => s.GetGenresByGameAlias(gameAlias, languageCode)).ReturnsAsync(expectedGenres);

        // Act
        var result = await _controller.GetGenresByGame(gameAlias, languageCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGenres = Assert.IsType<List<Genre>>(okResult.Value);
        Assert.Equal(expectedGenres.Count, returnedGenres.Count);
    }
}
