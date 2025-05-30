using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Services.Services;

namespace Storefy.Tests.Services.Services;
public class GenreServiceTests
{
    private readonly GenreService _genreService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public GenreServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _genreService = new GenreService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddGenre_AddsGenreIfItNotExists()
    {
        // Arrange
        var genreDto = new CreateUpdateGenreDto { Genre = new GenreDto { Id = Guid.NewGuid().ToString(), Name = "Action" } };
        _unitOfWorkMock.Setup(uow => uow.GenreRepository.GenreExists(genreDto.Genre.Name))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(uow => uow.GenreRepository.Add(genreDto))
            .ReturnsAsync(new Genre { Id = genreDto.Genre.Id, Name = genreDto.Genre.Name });

        // Act
        var newGenre = await _genreService.AddGenre(genreDto);

        // Assert
        Assert.NotNull(newGenre);
        Assert.Equal(genreDto.Genre.Name, newGenre.Name);
    }

    [Fact]
    public async Task DeleteGenre_DeletesGenreWithSpecifiedId()
    {
        // Arrange
        var genreId = "1";
        var genre = new Genre { Id = genreId, Name = "Action" };
        _unitOfWorkMock.Setup(uow => uow.GenreRepository.GetById(genreId))
            .ReturnsAsync(genre);
        _unitOfWorkMock.Setup(uow => uow.GenreRepository.Delete(genre))
            .ReturnsAsync(genre);

        // Act
        var deletedGenre = await _genreService.DeleteGenre(genreId);

        // Assert
        Assert.NotNull(deletedGenre);
        Assert.Equal(genreId, deletedGenre.Id);
    }

    [Fact]
    public async Task GetGenreById_ReturnsGenreWithMatchingId()
    {
        // Arrange
        var genreId = "1";
        var genre = new Genre { Id = genreId, Name = "Action" };
        _unitOfWorkMock.Setup(uow => uow.GenreRepository.GetById(genreId))
            .ReturnsAsync(genre);

        // Act
        var result = await _genreService.GetGenreById(genreId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genreId, result.Id);
    }

    [Fact]
    public async Task GetAllGenres_ReturnsListOfAllGenres()
    {
        // Arrange
        var genres = new List<Genre>
            {
                new() { Id = "1", Name = "Action" },
                new() { Id = "2", Name = "Adventure" },
                new() { Id = "3", Name = "RPG" },
            };
        _unitOfWorkMock.Setup(uow => uow.GenreRepository.GetAll())
            .ReturnsAsync(genres);

        // Act
        var result = await _genreService.GetAllGenres();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task UpdateGenre_UpdatesTheGenreInformation()
    {
        // Arrange
        var genreDto = new CreateUpdateGenreDto
        {
            Genre = new GenreDto
            {
                Id = "1",
                Name = "Test Genre",
            },
        };
        _unitOfWorkMock.Setup(uow => uow.GenreRepository.Update(genreDto))
            .ReturnsAsync(new Genre { Id = genreDto.Genre.Id, Name = genreDto.Genre.Name });

        // Act
        var updatedGenre = await _genreService.UpdateGenre(genreDto);

        // Assert
        Assert.NotNull(updatedGenre);
        Assert.Equal(genreDto.Genre.Name, updatedGenre.Name);
        Assert.Equal(genreDto.Genre.Id, updatedGenre.Id);
    }

    [Fact]
    public async Task UpdateGenre_UpdatesCategoryAndAddsNewGenre_WhenGenreNotExistsInGenreRepository()
    {
        // Arrange
        var genreDto = new CreateUpdateGenreDto
        {
            Genre = new GenreDto
            {
                Id = "2",
                Name = "Strategy",
            },
        };

        _unitOfWorkMock.Setup(uow => uow.GenreRepository.Update(genreDto)).ThrowsAsync(new Exception());
        _unitOfWorkMock.Setup(uow => uow.GenreRepository.Add(genreDto))
            .ReturnsAsync(new Genre { Id = genreDto.Genre.Id, Name = genreDto.Genre.Name });

        // Act
        var result = await _genreService.UpdateGenre(genreDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genreDto.Genre.Name, result.Name);
    }

    [Fact]
    public async Task UpdateGenre_ThrowsInvalidOperationException_WhenGenreNotFoundInBothRepositories()
    {
        // Arrange
        var genreDto = new CreateUpdateGenreDto
        {
            Genre = new GenreDto
            {
                Id = "3",
                Name = "Action",
            },
        };

        _unitOfWorkMock
            .Setup(uow => uow.GenreRepository.Update(genreDto))
            .ThrowsAsync(new Exception());

        // Act
        var exception = await Record.ExceptionAsync(() => _genreService.UpdateGenre(genreDto));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task GetGenresByGameAlias_GivenValidGameAlias_ReturnsGenres()
    {
        // Arrange
        var languageCode = "en";
        var gameAlias = "game1";
        var genres = new List<Genre> { new() { Id = "1", Name = "genre1" } };

        _unitOfWorkMock.Setup(u => u.GenreRepository.GetGenresByGame(gameAlias, languageCode)).ReturnsAsync(genres);

        // Act
        var result = await _genreService.GetGenresByGameAlias(gameAlias, languageCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetGenresByParentGenre_GivenValidParentGenreId_ReturnsGenres()
    {
        // Arrange
        var parentGenreId = "1";
        var genres = new List<Genre> { new() { Id = "2", Name = "genre2", ParentGenreId = parentGenreId } };

        _unitOfWorkMock.Setup(u => u.GenreRepository.GetGenresByParentGenre(parentGenreId)).ReturnsAsync(genres);

        // Act
        var result = await _genreService.GetGenresByParentGenre(parentGenreId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(genres, result);
    }
}
