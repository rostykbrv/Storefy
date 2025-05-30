using Microsoft.EntityFrameworkCore.Storage;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Repositories.Gamestore;

namespace Storefy.Interfaces;

/// <summary>
/// UnitOfWork interface enables a simple way to group multiple repositories.
/// Makes changes in a single transaction, and abstracts the underlying persistence implementation.
/// </summary>
/// <inheritdoc cref="IDisposable"/>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets an object that represents user repository which groups
    /// together all operations that can be performed on user entities.
    /// </summary>
    IUserRepository<User> UserRepository { get; }

    /// <summary>
    /// Gets an object that represents role repository which groups
    /// together all operations that can be performed on role entities.
    /// </summary>
    IRoleRepository<Role> RoleRepository { get; }

    /// <summary>
    /// Gets an object that represents game repository which groups
    /// together all operations that can be performed on game entities.
    /// </summary>
    IGameRepository<Game> GameRepository { get; }

    /// <summary>
    /// Gets an object that represents genre repository which groups
    /// together all operations that can be performed on genre entities.
    /// </summary>
    IGenreRepository<Genre> GenreRepository { get; }

    /// <summary>
    /// Gets an object that represents platform repository which groups
    /// together all operations that can be performed on platform entities.
    /// </summary>
    IPlatformRepository<Platform> PlatformRepository { get; }

    /// <summary>
    /// Gets an object that represents publisher repository which groups
    /// together all operations that can be performed on publisher entities.
    /// </summary>
    IPublisherRepository<Publisher> PublisherRepository { get; }

    /// <summary>
    /// Gets an object that represents order repository which groups
    /// together all operations that can be performed on order entities.
    /// </summary>
    IOrderRepository OrderRepository { get; }

    /// <summary>
    /// Gets an object that represents cart repository which groups
    /// together all operations that can be performed on cart entities.
    /// </summary>
    ICartRepository CartRepository { get; }

    /// <summary>
    /// Saves any changes made within this unit of work (transaction).
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveChanges();

    /// <summary>
    /// Begins a new transaction on the database context.
    /// </summary>
    /// <returns>An object representing the new transaction,
    /// which allows the transaction to be committed or rolled back.</returns>
    IDbContextTransaction BeginTransaction();
}