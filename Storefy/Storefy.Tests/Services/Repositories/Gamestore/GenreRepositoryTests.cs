using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Services.Data;
using Storefy.Services.Repositories.Gamestore;

namespace Storefy.Tests.Services.Repositories.Gamestore;
public class GenreRepositoryTests : IDisposable
{
    private readonly GenreRepository _genreRepository;
    private readonly StorefyDbContext _dbContext;
    private bool _disposed;

    public GenreRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StorefyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StorefyDbContext(options);
        _genreRepository = new GenreRepository(_dbContext);
    }

    [Fact]
    public async Task Add_GenreDtoIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        CreateUpdateGenreDto nullGenreDto = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _genreRepository.Add(nullGenreDto!));
    }

    [Fact]
    public async Task Add_GenreDtoIsValid_AddsNewGenre()
    {
        // Arrange
        var genreDto = new GenreDto
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Genre",
            ParentGenreId = Guid.NewGuid().ToString(),
        };
        var createUpdateGenreDto = new CreateUpdateGenreDto { Genre = genreDto };

        // Act
        var result = await _genreRepository.Add(createUpdateGenreDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genreDto.Name, result.Name);
        Assert.Equal(genreDto.ParentGenreId, result.ParentGenreId);
    }

    [Fact]
    public async Task Add_Genre_GenreAlreadyExists_ThrowsException()
    {
        // Arrange
        var existingGenre = new Genre
        {
            Id = "1",
            Name = "AlreadyExistedGenre",
        };

        await _dbContext.AddAsync(existingGenre);
        await _dbContext.SaveChangesAsync();

        var genreDto = new GenreDto
        {
            Id = existingGenre.Id,
            Name = existingGenre.Name,
        };
        var addGenreDto = new CreateUpdateGenreDto { Genre = genreDto };

        // Act and Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _genreRepository.Add(addGenreDto));
        Assert.Equal("This genre alredy exists!", exception.Message);
    }

    [Fact]
    public async Task DeleteGenre_DeleteGenre_ValidData()
    {
        // Arrange
        var genre = new Genre { Id = "12", Name = "Existing Genre" };
        _dbContext.Genres.Add(genre);
        await _dbContext.SaveChangesAsync();

        // Act
        var removedGenre = await _genreRepository.Delete(genre);

        // Assert
        var allGenres = await _dbContext.Genres.ToListAsync();
        Assert.DoesNotContain(removedGenre, allGenres);
    }

    [Fact]
    public async Task GenreExisting_ReturnTrue_GenreExists()
    {
        // Arrange
        var genre = new Genre { Id = "13", Name = "Existing Genre" };
        _dbContext.Genres.Add(genre);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _genreRepository.GenreExists(genre.Name);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task GetByIdGenre_ReturnExistingGenre_ValidData()
    {
        // Arrange
        var genre = new Genre { Id = "5", Name = "Existing Genre" };
        _dbContext.Genres.Add(genre);
        await _dbContext.SaveChangesAsync();

        // Act
        var existedGenre = await _genreRepository.GetById(genre.Id);

        // Assert
        Assert.Equal(genre.Id, existedGenre.Id);
    }

    [Fact]
    public async Task GetById_ThrowsException_When_GenreDoesNotExist()
    {
        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _genreRepository.GetById("55"));
    }

    [Fact]
    public async Task GetAllGenre_ReturnAllGenres()
    {
        // Arrange
        var genre1 = new Genre { Id = "15", Name = "Genre1" };
        var genre2 = new Genre { Id = "16", Name = "Genre2" };
        _dbContext.Genres.AddRange(genre1, genre2);
        await _dbContext.SaveChangesAsync();

        // Act
        var genres = await _genreRepository.GetAll();

        // Assert
        Assert.Equal(2, genres.Count());
    }

    [Fact]
    public async Task GenreNotExisting_ReturnFalse_GenreDoesNotExist()
    {
        // Act
        var result = await _genreRepository.GenreExists("Non-Existing Genre");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Update_GenreExists_UpdatesGenre()
    {
        // Arrange
        var genreId = Guid.NewGuid().ToString();
        var genre = new Genre { Id = genreId, Name = "Original Name" };
        _dbContext.Genres.Add(genre);
        await _dbContext.SaveChangesAsync();

        var updatedGenreDto = new GenreDto
        {
            Id = genreId,
            Name = "Updated Name",
        };
        var updateDto = new CreateUpdateGenreDto { Genre = updatedGenreDto };

        // Act
        var updatedGenre = await _genreRepository.Update(updateDto);

        // Assert
        Assert.NotNull(updatedGenre);
        Assert.Equal(updatedGenreDto.Name, updatedGenre.Name);
    }

    [Fact]
    public async Task Update_GenreDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistingGenreId = Guid.NewGuid().ToString();
        var genreDto = new GenreDto { Id = nonExistingGenreId, Name = "Test Name" };
        var updateDto = new CreateUpdateGenreDto { Genre = genreDto };

        // Act and Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _genreRepository.Update(updateDto));
        Assert.Equal("This genre doesn't exist!", exception.Message);
    }

    [Fact]
    public async Task Update_GenreExists_ParentGenreIsNotNull_SetParentGenre()
    {
        // Arrange
        var genreId = Guid.NewGuid().ToString();
        var genre = new Genre { Id = genreId, Name = "Original Name" };
        _dbContext.Genres.Add(genre);
        await _dbContext.SaveChangesAsync();

        var updatedGenreDto = new GenreDto
        {
            Id = genreId,
            Name = "Updated Name",
            ParentGenreId = "5",
        };
        var updateDto = new CreateUpdateGenreDto { Genre = updatedGenreDto };

        // Act
        var result = await _genreRepository.Update(updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Genre.ParentGenreId, result.ParentGenreId);
    }

    [Fact]
    public async Task GetGenresByGame_GameExists_ReturnsGenres()
    {
        // Arrange
        var languageCode = "en";
        var genre = new Genre { Id = Guid.NewGuid().ToString(), Name = "Genre1" };
        var game = new Game { Id = "4", Name = "Game1", Key = "game1", Genres = new List<Genre> { genre } };
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _genreRepository.GetGenresByGame(game.Key, languageCode);

        // Assert
        Assert.Single(result);
        Assert.Equal(genre.Name, result.First().Name);
    }

    [Fact]
    public async Task GetGenresByGame_GameExists_ReturnsTranslatedGenres()
    {
        // Arrange
        var languageCode = "ua";
        var genre = new Genre { Id = Guid.NewGuid().ToString(), Name = "Genre1" };
        var game = new Game { Id = "4", Name = "Game1", Key = "game1", Genres = new List<Genre> { genre } };
        _dbContext.Games.Add(game);

        var language = new Language
        {
            Id = "10",
            LanguageCode = languageCode,
            LanguageName = "Ukranian",
            GenreTranslations = new List<GenreTranslation>(),
        };

        var genreTranslation = new GenreTranslation
        {
            Id = "10",
            Genre = genre,
            GenreId = genre.Id,
            Language = language,
            LanguageId = language.Id,
            Name = "Назва жанру",
        };

        language.GenreTranslations.Add(genreTranslation);
        _dbContext.Languages.Add(language);
        _dbContext.GenreTranslations.Add(genreTranslation);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _genreRepository.GetGenresByGame(game.Key, languageCode);

        // Assert
        Assert.Single(result);
        Assert.Equal(genreTranslation.Name, result.First().Name);
    }

    [Fact]
    public async Task GetGenresByGame_GameDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        var languageCode = "en";
        var nonExistingGameKey = "non-existing-key";

        // Act
        var result = await _genreRepository.GetGenresByGame(nonExistingGameKey, languageCode);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGenresByParentGenre_ParentExists_ReturnsGenres()
    {
        // Arrange
        var parentGenreId = Guid.NewGuid().ToString();
        var genre1 = new Genre { Id = Guid.NewGuid().ToString(), Name = "Genre1", ParentGenreId = parentGenreId };
        var genre2 = new Genre { Id = Guid.NewGuid().ToString(), Name = "Genre2", ParentGenreId = parentGenreId };
        _dbContext.Genres.AddRange(new List<Genre> { genre1, genre2 });
        await _dbContext.SaveChangesAsync();

        // Act
        var genres = await _genreRepository.GetGenresByParentGenre(parentGenreId);

        // Assert
        Assert.Equal(2, genres.Count());
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
