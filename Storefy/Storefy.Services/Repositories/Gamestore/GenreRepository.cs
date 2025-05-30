using Microsoft.EntityFrameworkCore;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Repositories.Gamestore;
using Storefy.Services.Data;

namespace Storefy.Services.Repositories.Gamestore;

/// <summary>
/// Repository to manage the operations of Genre entities using a database context.
/// </summary>
/// <inheritdoc cref="IGenreRepository{T}"/>
public class GenreRepository : IGenreRepository<Genre>
{
    private readonly StorefyDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The data base context
    /// to be used by the repository.</param>
    public GenreRepository(StorefyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Genre> Add(CreateUpdateGenreDto genreDto)
    {
        if (genreDto == null)
        {
            throw new ArgumentNullException(nameof(genreDto));
        }

        if (!await GenreExists(genreDto.Genre.Name))
        {
            var createdGenre = new Genre
            {
                Id = Guid.NewGuid().ToString(),
                Name = genreDto.Genre.Name,
                ParentGenreId = genreDto.Genre.ParentGenreId,
                CategoryId = genreDto.Genre.Id,
            };

            createdGenre.ParentGenreId = string.IsNullOrEmpty(genreDto.Genre.ParentGenreId) ?
                null : genreDto.Genre.ParentGenreId;

            await _dbContext.AddAsync(createdGenre);
            await _dbContext.SaveChangesAsync();

            return createdGenre;
        }

        throw new InvalidOperationException("This genre alredy exists!");
    }

    /// <inheritdoc />
    public async Task<Genre> Delete(Genre entity)
    {
        _dbContext.Genres.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    /// <inheritdoc />
    public async Task<bool> GenreExists(string genreName)
    {
        return await _dbContext.Genres
            .AnyAsync(x => x.Name.Equals(genreName));
    }

    /// <inheritdoc />
    public async Task<Genre> GetById(string id)
    {
        var selectedGenre = await _dbContext.Genres
            .FirstOrDefaultAsync(g => g.Id == id);

        return selectedGenre ??
            throw new InvalidOperationException("Genre with this ID doesn't exist");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Genre>> GetAll()
    {
        return await _dbContext.Genres.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Genre> Update(CreateUpdateGenreDto genreDto)
    {
        var updatedGenre = await _dbContext.Genres.FindAsync(genreDto.Genre.Id);

        if (updatedGenre != null)
        {
            var outdatedGenre = new Genre
            {
                Name = updatedGenre.Name,
                CategoryId = updatedGenre.CategoryId,
                Description = updatedGenre.Description,
                Games = updatedGenre.Games,
                ParentGenre = updatedGenre.ParentGenre,
                ParentGenreId = updatedGenre.ParentGenreId,
                Picture = updatedGenre.Picture,
                Id = updatedGenre.Id,
            };
            updatedGenre.ParentGenreId = string.IsNullOrEmpty(genreDto.Genre.ParentGenreId) ?
                null : genreDto.Genre.ParentGenreId;
            updatedGenre.Name = genreDto.Genre.Name;
            await _dbContext.SaveChangesAsync();
            Console.WriteLine(outdatedGenre);
            return updatedGenre;
        }

        throw new InvalidOperationException("This genre doesn't exist!");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Genre>> GetGenresByGame(string gameAlias, string languageCode)
    {
        var genresByGame = await _dbContext.Genres
           .Include(g => g.Games)
           .Where(g => g.Games
           .Any(game => game.Key.Equals(gameAlias)))
           .ToListAsync();

        if (languageCode != "en")
        {
            var genreTranslations = await _dbContext.GenreTranslations
                .Where(gt => gt.Language.LanguageCode == languageCode)
                .ToListAsync();

            foreach (var genre in genresByGame)
            {
                var translation = genreTranslations.FirstOrDefault(gt => gt.GenreId == genre.Id);

                if (translation != null)
                {
                    genre.Name = translation.Name;
                }
            }
        }

        return genresByGame;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Genre>> GetGenresByParentGenre(string parentGenreId)
    {
        return await _dbContext.Genres
            .Where(g => g.ParentGenreId == parentGenreId)
            .ToListAsync();
    }
}
