using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Repositories.Gamestore;
using Storefy.Services.Data;
using Storefy.Services.Repositories.Gamestore;

namespace Storefy.Services;

/// <summary>
/// Manages the all units of work for application.
/// </summary>
/// <inheritdoc cref="IUnitOfWork"/>
public class UnitOfWork : IUnitOfWork
{
    private readonly StorefyDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private bool _disposed;
    private IUserRepository<User> _userRepository;
    private IRoleRepository<Role> _roleRepository;
    private IGameRepository<Game>? _gameRepository;
    private IGenreRepository<Genre>? _genreRepository;
    private IPlatformRepository<Platform>? _platformRepository;
    private IPublisherRepository<Publisher>? _publisherRepository;
    private IOrderRepository? _orderRepository;
    private ICartRepository? _cartRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/>
    /// class using the specified StorefyDbContext.
    /// </summary>
    /// <param name="dbContext">The StorefyDbContext to use for repository operations.</param>
    /// <param name="contextAccessor">Access to the HTTP context of a request.</param>
    public UnitOfWork(
        StorefyDbContext dbContext,
        IHttpContextAccessor contextAccessor)
    {
        _httpContextAccessor = contextAccessor;
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public IUserRepository<User> UserRepository =>
        _userRepository ??= new UserRepository(_dbContext, _httpContextAccessor);

    /// <inheritdoc />
    public IRoleRepository<Role> RoleRepository =>
        _roleRepository ??= new RoleRepository(_dbContext);

    /// <inheritdoc />
    public IGameRepository<Game> GameRepository =>
        _gameRepository ??= new GameRepository(_dbContext, _httpContextAccessor);

    /// <inheritdoc />
    public IGenreRepository<Genre> GenreRepository =>
        _genreRepository ??= new GenreRepository(_dbContext);

    /// <inheritdoc />
    public IPlatformRepository<Platform> PlatformRepository =>
        _platformRepository ??= new PlatformRepository(_dbContext);

    /// <inheritdoc />
    public IPublisherRepository<Publisher> PublisherRepository =>
        _publisherRepository ??= new PublisherRepository(_dbContext);

    /// <inheritdoc />
    public IOrderRepository OrderRepository =>
        _orderRepository ??= new OrderRepository(_dbContext);

    /// <inheritdoc />
    public ICartRepository CartRepository =>
        _cartRepository ??= new CartRepository(_dbContext);

    /// <inheritdoc />
    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public IDbContextTransaction BeginTransaction()
    {
        return _dbContext.Database.BeginTransaction();
    }

    /// <summary>
    /// Disposes the UnitOfWork object resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the UnitOfWork object resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
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
