using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Repositories.Gamestore;
using Storefy.Services.Data;

namespace Storefy.Services.Repositories.Gamestore;

/// <summary>
/// Repository to manage the operations of Game entities using a database context.
/// </summary>
/// <inheritdoc cref="IGameRepository{T}"/>
public class GameRepository : IGameRepository<Game>
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly StorefyDbContext _dbContext;
    private readonly DbSet<Game> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The data base context to be used
    /// by the repository.</param>
    /// <param name="contextAccessor">Provides access to
    /// the HTTP context for the current request.</param>
    public GameRepository(
        StorefyDbContext dbContext,
        IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        _dbContext = dbContext;
        _dbSet = dbContext.Set<Game>();
    }

    /// <inheritdoc />
    public async Task<Game> Add(CreateUpdateGameDto gameDto)
    {
        var newGame = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = gameDto.Game.Name,
            Description = gameDto.Game.Description,
            Key = gameDto.Game.Key,
            Price = gameDto.Game.Price,
            UnitInStock = gameDto.Game.UnitInStock,
            Discontinued = gameDto.Game.Discontinued,
            DateAdded = DateTime.Now,
            ImageUrl = gameDto.Game.ImageUrl,
            GameSize = gameDto.Game.GameSize,
            CopyType = gameDto.Game.CopyType,
            ReleasedDate = gameDto.Game.ReleasedDate,
            Genres = new List<Genre>(),
            Platforms = new List<Platform>(),
        };

        if (gameDto.Publisher != null)
        {
            var publisherName = gameDto.PublisherName;
            var publisher = await _dbContext.Publishers
                .FirstOrDefaultAsync(p => p.CompanyName == publisherName);

            if (publisher != null)
            {
                newGame.PublisherId = gameDto.Publisher;
                newGame.Publisher = publisher;
            }
        }

        foreach (var genreName in gameDto.GenreNames)
        {
            var genre = await _dbContext.Genres
                .FirstOrDefaultAsync(g => g.Name == genreName);

            if (genre != null)
            {
                newGame.Genres.Add(genre);
            }
        }

        foreach (var platformId in gameDto.Platforms)
        {
            var platform = await _dbContext.Platforms.FindAsync(platformId);

            if (platform != null)
            {
                newGame.Platforms.Add(platform);
            }
        }

        await _dbContext.AddAsync(newGame);
        await _dbContext.SaveChangesAsync();

        return newGame;
    }

    /// <inheritdoc />
    public async Task<bool> AliasExists(string alias)
    {
        var gameKeyExists = await _dbContext.Games
            .AnyAsync(x => x.Key.Equals(alias));

        return gameKeyExists;
    }

    /// <inheritdoc />
    public async Task<Game> Delete(Game entity)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetAll()
    {
        return await _dbContext.Games.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<GamePageResult> GetAllGames(FilterOptions filters, string languageCode)
    {
        IQueryable<Game> queryableGames = _dbContext.Games;

        if (languageCode != "en")
        {
            queryableGames = queryableGames.Select(game => new Game
            {
                Id = game.Id,
                Name = game.Name,
                CopyType = _dbContext.GameTranslations.Where(t => t.GameId == game.Id && t.Language.LanguageCode == languageCode)
                .Select(t => t.CopyType)
                .FirstOrDefault(),
                Key = game.Key,
                ImageUrl = game.ImageUrl,
                Price = game.Price,
            });
        }

        if (filters.Trigger is not "PageChange" and not "PageCountChange")
        {
            queryableGames = filters.Trigger switch
            {
                "ApplyFilters" => ApplyFilters(queryableGames, filters),
                "SortingChange" => ApplySorting(queryableGames, filters),
                _ => throw new ArgumentException("Invalid filter/sorting option"),
            };
        }

        var gamePageResult = await GetPaginatedResult(queryableGames, filters.Page, filters.PageCount);

        return gamePageResult;
    }

    /// <inheritdoc />
    public async Task<Game> GetByAlias(string alias, string languageCode)
    {
        var game = await _dbContext.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(x => x.Key.Equals(alias));

        if (game != null)
        {
            game.ViewCount++;
        }

        await _dbContext.SaveChangesAsync();

        if (languageCode != "en" && game != null)
        {
            var gameTranslations = await _dbContext.GameTranslations
                .Include(gt => gt.Game)
                .Include(gt => gt.Language)
                .Where(gt => gt.Game.Key.Equals(alias) && gt.Language.LanguageCode == languageCode)
                .FirstOrDefaultAsync();

            if (gameTranslations != null)
            {
                game.Description = gameTranslations.Description;
                game.CopyType = gameTranslations.CopyType;
                game.ReleasedDate = gameTranslations.ReleasedDate;
            }
        }

        return game;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetGamesByGenre(string genreId)
    {
        var selectedGenre = await _dbContext.Genres.Where(g => g.Id == genreId)
            .FirstOrDefaultAsync();

        if (selectedGenre != null)
        {
            var returnedGames = await _dbContext.Games
                .Include(g => g.Genres)
                .Where(g => g.Genres
                .Any(genre => genre.Name == selectedGenre.Name))
                .ToListAsync();

            return returnedGames;
        }

        return Enumerable.Empty<Game>();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetGamesByPlatform(string platformId)
    {
        var platformsExist = _dbContext.Platforms.Select(p => p.Id);

        return !platformsExist.Contains(platformId)
            ? throw new InvalidOperationException($"Invalid platform identificator!")
            : (IEnumerable<Game>)await _dbContext.Games
            .Include(g => g.Platforms)
            .Where(g => g.Platforms.Any(platform => platform.Id == platformId)).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Game>> GetGamesByPublisher(string companyName)
    {
        var existedPublisher = _dbContext.Publishers.Select(p => p.CompanyName);

        return !existedPublisher.Contains(companyName)
            ? Enumerable.Empty<Game>()
            : await _dbContext.Games.Include(p => p.Publisher)
            .Where(p => p.Publisher.CompanyName.Equals(companyName)).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Game> Update(CreateUpdateGameDto updatedGame)
    {
        var game = await _dbContext.Games
        .Include(g => g.Genres)
        .Include(g => g.Platforms)
        .FirstOrDefaultAsync(g => g.Key == updatedGame.Game.Key)
        ?? null;

        if (game != null)
        {
            var outdatedGame = new Game
            {
                Key = game.Key,
                Name = game.Name,
                UnitInStock = game.UnitInStock,
                DateAdded = game.DateAdded,
                Discontinued = game.Discontinued,
                Comments = game.Comments,
                Description = game.Description,
                Discount = game.Discount,
                GameId = game.GameId,
                Genres = game.Genres,
                OrderDetails = game.OrderDetails,
                Price = game.Price,
                PublisherId = game.PublisherId,
                Platforms = game.Platforms,
                Publisher = game.Publisher,
                UnitsOnOrder = game.UnitsOnOrder,
                ReorderLevel = game.ReorderLevel,
                QuantityPerUnit = game.QuantityPerUnit,
                ViewCount = game.ViewCount,
            };
            var publisherUpdate = await _dbContext.Publishers
                .FirstOrDefaultAsync(p => p.CompanyName == updatedGame.PublisherName);

            game.Name = updatedGame.Game.Name;
            game.Key = updatedGame.Game.Key;
            game.Description = updatedGame.Game.Description;
            game.Price = updatedGame.Game.Price;
            game.UnitInStock = updatedGame.Game.UnitInStock;
            game.Discontinued = updatedGame.Game.Discontinued;
            game.ImageUrl = updatedGame.Game.ImageUrl;
            game.GameSize = updatedGame.Game.GameSize;
            game.CopyType = updatedGame.Game.CopyType;
            game.ReleasedDate = updatedGame.Game.ReleasedDate;

            game.Genres.Clear();
            game.Platforms.Clear();
            game.PublisherId = publisherUpdate?.Id;

            foreach (var genreName in updatedGame.GenreNames)
            {
                var genre = await _dbContext.Genres
                    .FirstOrDefaultAsync(g => g.Name == genreName);

                if (genre != null)
                {
                    game.Genres.Add(genre);
                }
            }

            foreach (var platformId in updatedGame.Platforms)
            {
                var platform = await _dbContext.Platforms
                    .FindAsync(platformId);

                if (platform != null)
                {
                    game.Platforms.Add(platform);
                }
            }

            await _dbContext.SaveChangesAsync();

            return game;
        }

        throw new InvalidOperationException("This game doesn't exist!");
    }

    /// <inheritdoc />
    public async Task<Order> BuyGame(string gameAlias)
    {
        var game = await _dbContext.Games
            .FirstOrDefaultAsync(g => g.Key.Equals(gameAlias));

        if (game != null)
        {
            var openOrder = await _dbContext.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.Status == OrderStatus.Open)
                .FirstOrDefaultAsync();

            if (openOrder == null)
            {
                openOrder = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Open,
                    OrderDetails = new List<OrderDetails>(),
                };

                _dbContext.Orders.Add(openOrder);
            }

            var existingOrderDetail = openOrder.OrderDetails
                .FirstOrDefault(od => od.ProductId.ToString() == game.Id);

            if (existingOrderDetail != null)
            {
                existingOrderDetail.Quantity += 1;
            }
            else
            {
                var orderDetails = new OrderDetails
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = game.Id,
                    ProductName = game.Name,
                    Game = game,
                    Quantity = 1,
                    CreationDate = DateTime.Now,
                    Price = game.Price,
                    Discount = game.Discount,
                };

                openOrder.OrderDetails.Add(orderDetails);
            }

            openOrder.Sum = openOrder.OrderDetails
                .Sum(od => od.Price * od.Quantity);

            await _dbContext.SaveChangesAsync();

            return openOrder;
        }

        throw new InvalidOperationException("This game doesn't exist.");
    }

    /// <inheritdoc />
    public async Task<Game> GetById(string id)
    {
        return await _dbContext.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(g => g.Id.Equals(id));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Comment>> GetComments(string gameAlias)
    {
        var commentsRoot = await _dbContext.Games
        .Where(g => g.Key == gameAlias)
        .SelectMany(g => g.Comments)
        .Where(c => c.CommentId == null)
        .ToListAsync();

        foreach (var comment in commentsRoot)
        {
            LoadChildComments(comment);
        }

        return commentsRoot;
    }

    /// <inheritdoc />
    public async Task<Comment> AddCommentToGame(string gameAlias, AddCommentDto commentDto)
    {
        var game = await _dbContext.Games
            .FirstAsync(g => g.Key.Equals(gameAlias));

        var userName = await GetUserNameAndBanStatusAsync(commentDto);

        var comment = new Comment
        {
            Id = Guid.NewGuid().ToString(),
            Name = userName,
            Body = commentDto.Comment.Body,
            ChildComments = new List<Comment>(),
            Game = game,
        };

        var parentComment = await _dbContext.Comments.FindAsync(commentDto.ParentId);

        switch (commentDto.Action)
        {
            case "Reply":
                comment.Body = $"<a href=\"game/{gameAlias}#comment{parentComment.Id}\">" +
                    $"{parentComment.Name},</a> {commentDto.Comment.Body}";
                parentComment.ChildComments.Add(comment);
                break;
            case "Quote":
                comment.Body = $"<i>{parentComment.Body}</i><br>{commentDto.Comment.Body}";
                parentComment.ChildComments.Add(comment);
                break;
            default:
                await _dbContext.AddAsync(comment);
                break;
        }

        await _dbContext.SaveChangesAsync();

        return comment;
    }

    /// <inheritdoc />
    public async Task<Comment> DeleteComment(string commentId)
    {
        var comment = await _dbContext.Comments
            .Include(c => c.ChildComments)
            .Where(c => c.Id == commentId)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Comment not found");
        var quotedBody = comment.Body;
        comment.Body = "A comment/quote was deleted";

        foreach (var childComment in comment.ChildComments)
        {
            childComment.Body = childComment.Body
                .Replace($"<i>{quotedBody}</i><br>", "<i>A comment/quote was deleted</i><br>");
        }

        await _dbContext.SaveChangesAsync();

        return comment;
    }

    private void LoadChildComments(Comment comment)
    {
        var childComments = _dbContext.Comments
                    .Where(c => c.CommentId == comment.Id)
                    .ToList();

        foreach (var childComment in childComments)
        {
            LoadChildComments(childComment);
        }

        comment.ChildComments = childComments;
    }

    private static IQueryable<Game> ApplyFilters(IQueryable<Game> queryableGames, FilterOptions filters)
    {
        if (!string.IsNullOrEmpty(filters.DatePublishing))
        {
            var currentDate = DateTime.Now;
            switch (filters.DatePublishing.ToLower())
            {
                case "last week":
                    var lastWeekDate = currentDate.AddDays(-7);
                    queryableGames = queryableGames
                        .Where(game => game.DateAdded > lastWeekDate);
                    break;
                case "last month":
                    var lastMonthDate = currentDate.AddMonths(-1);
                    queryableGames = queryableGames
                        .Where(game => game.DateAdded > lastMonthDate);
                    break;
                case "last year":
                    var lastYearDate = currentDate.AddYears(-1);
                    queryableGames = queryableGames
                        .Where(game => game.DateAdded > lastYearDate);
                    break;
                case "2 years":
                    var twoYearsAgoDate = currentDate.AddYears(-2);
                    queryableGames = queryableGames
                        .Where(game => game.DateAdded > twoYearsAgoDate);
                    break;
                case "3 years":
                    var threeYearsAgoDate = currentDate.AddYears(-3);
                    queryableGames = queryableGames
                        .Where(game => game.DateAdded > threeYearsAgoDate);
                    break;
                default:
                    throw new ArgumentException("Invalid DateAddedOption filter");
            }
        }

        if (!string.IsNullOrWhiteSpace(filters.Name) && filters.Name.Length >= 3)
        {
            queryableGames = queryableGames
                .Where(game => game.Name.Contains(filters.Name));
        }

        if (filters.Genres != null)
        {
            queryableGames = queryableGames
                .Where(game => game.Genres
                .Any(genre => filters.Genres.Contains(genre.Id)));
        }

        if (filters.Platforms != null)
        {
            queryableGames = queryableGames
                .Where(game => game.Platforms
                .Any(platform => filters.Platforms.Contains(platform.Id)));
        }

        if (filters.Publishers != null)
        {
            queryableGames = queryableGames
                .Where(game => filters.Publishers
                .Contains(game.Publisher.Id));
        }

        if (filters.MaxPrice.HasValue)
        {
            queryableGames = queryableGames
                .Where(game => game.Price <= filters.MaxPrice.Value);
        }

        if (filters.MinPrice.HasValue)
        {
            queryableGames = queryableGames
                .Where(game => game.Price >= filters.MinPrice.Value);
        }

        return queryableGames;
    }

    private static IQueryable<Game> ApplySorting(IQueryable<Game> queryableGames, FilterOptions filters)
    {
        queryableGames = filters.Sort.ToLower() switch
        {
            "name" => queryableGames
            .OrderBy(game => game.Name),
            "price asc" => queryableGames
            .OrderBy(game => game.Price),
            "price desc" => queryableGames
            .OrderByDescending(game => game.Price),
            "most popular (most viewed)" => queryableGames
            .OrderByDescending(game => game.ViewCount),
            "most commented" => queryableGames
            .OrderByDescending(game => game.Comments.Count),
            "new (by date)" => queryableGames
            .OrderByDescending(game => game.DateAdded.Date),
            _ => throw new ArgumentException("Invalid sorting option"),
        };

        return queryableGames;
    }

    private static async Task<GamePageResult> GetPaginatedResult(IQueryable<Game> queryableGames, int page, int? pageCount)
    {
        var totalGamesCount = queryableGames.Count();
        var gamePageSize = pageCount ?? 24;
        var totalPages = (int)Math
            .Ceiling((double)totalGamesCount / gamePageSize);
        var games = await queryableGames
            .Skip((page - 1) * gamePageSize)
            .Take(gamePageSize)
            .ToArrayAsync();

        return new GamePageResult
        {
            Games = games,
            CurrentPage = page,
            TotalPages = totalPages,
        };
    }

    private async Task<string> GetUserNameAndBanStatusAsync(AddCommentDto commentDto)
    {
        var claimUserName = _contextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?
            .Value;

        if (!string.IsNullOrEmpty(claimUserName))
        {
            var user = await _dbContext.Users
                .Where(u => u.Name == claimUserName)
                .FirstOrDefaultAsync();

            var activeBan = await _dbContext
                .BanHistories
                .Where(b => b.UserId == user.Id)
                .FirstOrDefaultAsync(b => b.BanEnds > DateTime.UtcNow);

            if (activeBan != null)
            {
                throw new InvalidOperationException("User is currently banned and cannot write a comment");
            }
        }

        var userName = string.IsNullOrEmpty(claimUserName)
            ? commentDto.Comment.Name :
            claimUserName;

        return userName;
    }
}
