using System.Text;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;

namespace Storefy.Services.Services;

/// <summary>
/// Implementation of the services needed to manipulate and retrieve information for Game entities in the repository.
/// </summary>
/// <inheritdoc cref="IGameService"/>
public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to be used by the game repository.</param>
    public GameService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Game> CreateGame(CreateUpdateGameDto gameDto)
    {
        await EnsureGenreExistsInDatabase(gameDto);
        await EnsurePublisherExistsInDatabase(gameDto);

        if (string.IsNullOrEmpty(gameDto.Game.Key))
        {
            gameDto.Game.Key = GenerateGameAlias(gameDto.Game.Name);
        }

        gameDto.Game.Key = await EnsureUniqueGameAlias(gameDto.Game.Key);

        return await _unitOfWork.GameRepository.Add(gameDto);
    }

    /// <inheritdoc />
    public async Task<Game> GetGameByAlias(string gameAlias, string languageCode)
    {
        var returnedGame = await _unitOfWork.GameRepository.GetByAlias(gameAlias, languageCode);

        return returnedGame;
    }

    /// <inheritdoc />
    public async Task<Game> UpdateGame(CreateUpdateGameDto gameDto)
    {
        await EnsureGenreExistsInDatabase(gameDto);
        await EnsurePublisherExistsInDatabase(gameDto);

        try
        {
            var game = await _unitOfWork.GameRepository.Update(gameDto);

            return game;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        throw new InvalidOperationException("Game could not be updated.");
    }

    /// <inheritdoc />
    public async Task<Game> DeleteGame(string gameAlias, string languageCode)
    {
        var game = await _unitOfWork.GameRepository.GetByAlias(gameAlias, languageCode);

        return await _unitOfWork.GameRepository.Delete(game);
    }

    /// <inheritdoc />
    public async Task<GamePageResult> GetAllGames(FilterOptions filters, string languageCode)
    {
        var returnedGames = new List<Game>();
        var gamesSql = await _unitOfWork.GameRepository
            .GetAllGames(filters, languageCode);
        var returnedGameList = gamesSql.Games.ToList();
        returnedGames.AddRange(returnedGameList);

        var returnedPage = new GamePageResult
        {
            TotalPages = gamesSql.TotalPages,
            Games = returnedGames.ToArray(),
            CurrentPage = filters.Page,
        };

        return returnedPage;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetAll()
    {
        var returnedGames = new List<Game>();
        var gamesSql = await _unitOfWork.GameRepository
            .GetAll();

        returnedGames.AddRange(gamesSql);

        return returnedGames;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetGamesByGenre(string genreId)
    {
        var returnedGames = new List<Game>();
        var gamesByGenre = await _unitOfWork.GameRepository
            .GetGamesByGenre(genreId);
        returnedGames.AddRange(gamesByGenre);

        return returnedGames;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetGamesByPlatform(string platformId)
    {
        var returnedGames = await _unitOfWork.GameRepository
            .GetGamesByPlatform(platformId);

        return returnedGames;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetGamesByPublisher(string companyName)
    {
        var returnedPublisherGames = new List<Game>();
        var gamesByPublisher = await _unitOfWork.GameRepository
            .GetGamesByPublisher(companyName);
        returnedPublisherGames.AddRange(gamesByPublisher);

        return returnedPublisherGames;
    }

    /// <inheritdoc />
    public async Task<FileResult> DownloadGame(string gameAlias, string languageCode)
    {
        var gameSql = await _unitOfWork.GameRepository.GetByAlias(gameAlias, languageCode);

        var gameGenres = gameSql.Genres.Select(g => g.Name);
        var platforms = gameSql.Platforms.Select(p => p.Type);

        var downlloadedMongoInfo = new DownloadedFileGameDto
        {
            Name = gameSql.Name,
            Alias = gameSql.Key,
            Description = gameSql.Description,
            Price = gameSql.Price,
            Genres = gameGenres,
            Platforms = platforms,
            Publisher = gameSql.Publisher?.CompanyName,
        };

        return CreateFileResult(downlloadedMongoInfo);
    }

    /// <summary>
    /// Generates a game alias from the game name.
    /// </summary>
    /// <param name="gameName">The name of the game for which to generate an alias.</param>
    /// <returns>The generated game alias.</returns>
    public static string GenerateGameAlias(string gameName)
    {
        if (string.IsNullOrEmpty(gameName))
        {
            return string.Empty;
        }

        var gameAlias = gameName.ToLower().Replace(" ", "-");

        return gameAlias;
    }

    /// <summary>
    /// Ensures that the generated game alias is unique by appending numbers at the end.
    /// </summary>
    /// <param name="gameAlias">The game alias to check for uniqueness.</param>
    /// <returns>A unique game alias.</returns>
    public async Task<string> EnsureUniqueGameAlias(string gameAlias)
    {
        var uniqueGameAlias = gameAlias;
        var counter = 1;

        while (await _unitOfWork.GameRepository.AliasExists(uniqueGameAlias))
        {
            uniqueGameAlias = gameAlias + "-" + counter;
            counter++;
        }

        return uniqueGameAlias;
    }

    /// <inheritdoc />
    public async Task<Order> BuyGame(string gameAlias)
    {
        var selectedGame = await _unitOfWork.GameRepository.BuyGame(gameAlias);

        return selectedGame;
    }

    /// <inheritdoc />
    public async Task<Game> GetGameById(string gameId)
    {
        var returnedGame = await _unitOfWork.GameRepository.GetById(gameId);

        return returnedGame;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Comment>> GetComments(string gameAlias)
    {
        return await _unitOfWork.GameRepository.GetComments(gameAlias);
    }

    /// <inheritdoc />
    public async Task<Comment> AddCommentToGame(string gameAlias, AddCommentDto commentDto)
    {
        return await _unitOfWork.GameRepository.AddCommentToGame(gameAlias, commentDto);
    }

    /// <inheritdoc />
    public async Task<Comment> DeleteComment(string commentId)
    {
        return await _unitOfWork.GameRepository.DeleteComment(commentId);
    }

    /// <inheritdoc />
    public string[] GetSortingOptions()
    {
        var sortingOptions = new string[]
        {
            "Most popular (most viewed)",
            "Most commented",
            "Price ASC",
            "Price DESC",
            "New (by date)",
        };

        return sortingOptions;
    }

    /// <inheritdoc />
    public string[] GetPublishedDateOptions()
    {
        var dateOptions = new string[]
        {
            "Last Week",
            "Last Month ",
            "Last Year",
            "2 Years",
            "3 Years",
        };

        return dateOptions;
    }

    /// <inheritdoc />
    public int[] GetPagingOptions()
    {
        var pagingOptions = new int[] { 10, 20, 30, 50, 100 };

        return pagingOptions;
    }

    private async Task EnsurePublisherExistsInDatabase(CreateUpdateGameDto gameDto)
    {
        if (gameDto.Genres != null)
        {
            foreach (var genre in gameDto.Genres)
            {
                var genreSqlCheck = await _unitOfWork.GenreRepository.GetById(genre);

                if (genreSqlCheck != null)
                {
                    gameDto.GenreNames.Add(genreSqlCheck.Name);
                }
            }
        }
    }

    private async Task EnsureGenreExistsInDatabase(CreateUpdateGameDto gameDto)
    {
        if (gameDto.Publisher != null)
        {
            var publisherSql = await _unitOfWork.PublisherRepository.GetPublisherById(gameDto.Publisher);
            gameDto.PublisherName = publisherSql.CompanyName;
        }
    }

    private static FileResult CreateFileResult(DownloadedFileGameDto gameInformation)
    {
        var content = $"Name: {gameInformation.Name}" +
            $"\nGame Alias:{gameInformation.Alias}" +
            $"\nDescription: {gameInformation.Description}" +
            $"\nPrice: {gameInformation.Price}" +
            $"\nGenres: {string.Join(", ", gameInformation.Genres)}" +
            (gameInformation.Platforms != null ? $"" +
            $"\nPlatforms: {string.Join(", ", gameInformation.Platforms)}" : string.Empty) +
            $"\nPublisher: {gameInformation.Publisher}";
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var filename = $"{gameInformation.Name}_{DateTime.Now:yyyyMMddHHmmss}.txt";

        return new FileResult(contentBytes, "text/plain", filename);
    }
}
