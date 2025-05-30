using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;

namespace Storefy.Services.Services;

/// <summary>
/// Implementation of the services needed to manipulate and retrieve information for Genre entities in the repository.
/// </summary>
/// <inheritdoc cref="IGenreService"/>
public class GenreService : IGenreService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to be used by the genre repository.</param>
    public GenreService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Genre> AddGenre(CreateUpdateGenreDto genreDto)
    {
        var newGenre = await _unitOfWork.GenreRepository.Add(genreDto);

        return newGenre;
    }

    /// <inheritdoc />
    public async Task<Genre> DeleteGenre(string id)
    {
        var deletedGenre = await _unitOfWork.GenreRepository
            .GetById(id);

        return await _unitOfWork.GenreRepository.Delete(deletedGenre);
    }

    /// <inheritdoc />
    public async Task<Genre> GetGenreById(string genreId)
    {
        var selectedGenre = await _unitOfWork.GenreRepository.GetById(genreId);

        return selectedGenre;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Genre>> GetAllGenres()
    {
        var genresSql = await _unitOfWork.GenreRepository
            .GetAll();
        var returnedGenres = new List<Genre>();
        returnedGenres.AddRange(genresSql);

        return returnedGenres;
    }

    /// <inheritdoc />
    public async Task<Genre> UpdateGenre(CreateUpdateGenreDto genreDto)
    {
        try
        {
            var genre = await _unitOfWork.GenreRepository.Update(genreDto);

            return genre;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        throw new InvalidOperationException("Cannot update selected genre");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Genre>> GetGenresByGameAlias(string gameAlias, string languageCode)
    {
        var returnedGenres = new List<Genre>();
        var gameSqlGenres = await _unitOfWork.GenreRepository
            .GetGenresByGame(gameAlias, languageCode);

        returnedGenres.AddRange(gameSqlGenres);

        return returnedGenres;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Genre>> GetGenresByParentGenre(string parentGenreId)
    {
        return await _unitOfWork.GenreRepository
            .GetGenresByParentGenre(parentGenreId);
    }
}
