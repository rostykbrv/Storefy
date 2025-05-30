using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Services;
using Storefy.Services.Data;

namespace Storefy.Tests.Services.Services;
public class UnitOfWorkTests : IDisposable
{
    private readonly StorefyDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private bool _disposed;

    public UnitOfWorkTests()
    {
        var uniqueDatabaseName = Guid.NewGuid().ToString();
        var optionsBuilder = new DbContextOptionsBuilder<StorefyDbContext>()
            .UseInMemoryDatabase(uniqueDatabaseName)
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockChangesCollection = new Mock<IMongoCollection<ChangesLog>>();
        _dbContext = new StorefyDbContext(optionsBuilder.Options);
        _unitOfWork = new UnitOfWork(_dbContext, mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task SaveChanges_CommitsPlatformChangesToDatabase()
    {
        // Arrange
        var platformDto = new CreateUpdatePlatformDto
        {
            Platform = new PlatformDto
            {
                Id = "1",
                Type = "Mobile",
            },
        };
        var addedPlatform = await _unitOfWork.PlatformRepository.Add(platformDto);

        // Act
        await _unitOfWork.SaveChanges();

        // Assert
        var retrievedPlatform = await _unitOfWork.PlatformRepository.GetById(addedPlatform.Id);
        Assert.NotNull(retrievedPlatform);
        Assert.Equal(platformDto.Platform.Type, retrievedPlatform.Type);
    }

    [Fact]
    public async Task SaveChanges_CommitsGenresChangesToDatabase()
    {
        // Arrange
        var genreDto = new CreateUpdateGenreDto
        {
            Genre = new GenreDto
            {
                Id = "1",
                Name = "Action",
            },
        };
        var addedGenre = await _unitOfWork.GenreRepository.Add(genreDto);

        // Act
        await _unitOfWork.SaveChanges();

        // Assert
        var retrievedGenre = await _unitOfWork.GenreRepository.GetById(addedGenre.Id);
        Assert.NotNull(retrievedGenre);
        Assert.Equal(genreDto.Genre.Name, retrievedGenre.Name);
    }

    [Fact]
    public async Task SaveChanges_CommitsPuiblisherChangesToDatabase()
    {
        // Arrange
        var publisherDto = new CreateUpdatePublisherDto
        {
            Publisher = new PublisherDto
            {
                Id = "1",
                CompanyName = "TestCo",
            },
        };
        var addedPublisher = await _unitOfWork.PublisherRepository.AddPublisher(publisherDto);

        // Act
        await _unitOfWork.SaveChanges();

        // Assert
        var retrievedPublisher = await _unitOfWork.PublisherRepository.GetPublisherById(addedPublisher.Id);
        Assert.NotNull(retrievedPublisher);
        Assert.Equal(publisherDto.Publisher.CompanyName, retrievedPublisher.CompanyName);
    }

    [Fact]
    public async Task SaveChanges_CommitsOrderChangesToDatabase()
    {
        // Arrange
        var gameKey = "test";

        var game = new Game
        {
            Id = "123",
            Name = "Test",
            Key = "test",
            OrderDetails = new List<OrderDetails>(),
        };

        var orderDetails = new OrderDetails
        {
            Id = "555",
            ProductId = "123",
            ProductName = "Test",
            Game = game,
        };

        var order = new Order
        {
            Id = "777",
            CustomerId = Guid.NewGuid().ToString(),
            OrderDate = DateTime.Now,
            PaidDate = null,
            Status = OrderStatus.Open,
            Sum = 22,
            OrderDetails = new List<OrderDetails> { orderDetails },
        };

        _dbContext.Orders.Add(order);
        await _unitOfWork.SaveChanges();

        // Act
        var deletedOrder = await _unitOfWork.OrderRepository.Delete(gameKey);

        // Assert
        var deleteOrder = await _unitOfWork.OrderRepository.GetOrder(order.Id);
        Assert.NotNull(deleteOrder);
        Assert.Equal(order.Sum, deletedOrder.Sum);
    }

    [Fact]
    public async Task SaveChanges_CommitsCartChangesToDatabase()
    {
        // Arrange
        _ = new List<OrderDetails>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                CreationDate = DateTime.Now,
                Quantity = 1,
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                CreationDate = DateTime.Now,
                Quantity = 1,
            },
        };
        var cartInfo = await _unitOfWork.CartRepository.GetCartDetails();

        // Act
        await _unitOfWork.SaveChanges();

        // Assert
        var retrievedCart = await _unitOfWork.CartRepository.GetCartDetails();
        Assert.NotNull(retrievedCart);
        Assert.Equal(cartInfo.Count(), retrievedCart.Count());
    }

    [Fact]
    public void BeginTransaction_ShouldReturnDbContextTransaction()
    {
        // Act
        var transaction = _unitOfWork.BeginTransaction();

        // Assert
        Assert.IsAssignableFrom<IDbContextTransaction>(transaction);
    }

    [Fact]
    public async Task SaveChanges_CommitsGamesChangesToDatabase()
    {
        // Arrange
        var languageCode = "en";
        _dbContext.Platforms.Add(new Platform
        {
            Id = "1",
            Type = "Test Platform",
        });
        _dbContext.Genres.Add(new Genre
        {
            Id = "1",
            Name = "Test Genre",
        });
        _dbContext.Publishers.Add(new Publisher
        {
            Id = "1",
            CompanyName = "TestCompany",
            Description = "Test Description",
            HomePage = "homepage.com",
        });

        var gameDto = new GameDto
        {
            Id = "2",
            Name = "Game name",
            Description = "Game description",
            Key = "game-name",
        };

        var createdGame = new CreateUpdateGameDto
        {
            Game = gameDto,
            Platforms = new List<string> { "1" },
            Genres = new List<string> { "1" },
            Publisher = "1",
        };
        await _unitOfWork.SaveChanges();

        // Act
        var addedGame = await _unitOfWork.GameRepository.Add(createdGame);

        // Assert
        var retrievedGame = await _unitOfWork.GameRepository.GetByAlias(addedGame.Key, languageCode);
        Assert.NotNull(retrievedGame);
        Assert.Equal(gameDto.Name, retrievedGame.Name);
    }

    [Fact]
    public async Task SaveChanges_CommitsUserChangesToDatabase()
    {
        // Arrange
        _dbContext.Roles.Add(new Role
        {
            Id = "1",
            Name = "User",
        });

        var notification = new Notification
        {
            Id = Guid.NewGuid().ToString(),
            Type = "Email",
            UserNotificationType = new List<User>(),
        };

        _dbContext.Notifications.Add(notification);

        var userDto = new UserDto
        {
            Id = "1",
            Name = "TestUser",
        };

        var createdUser = new AddUserDto()
        {
            User = userDto,
            Password = "password",
            Roles = new[]
            {
                "1",
            },
            Notification = "76",
        };
        await _unitOfWork.SaveChanges();

        // Act
        var addedUser = await _unitOfWork.UserRepository.CreateUser(createdUser);

        // Assert
        Assert.NotNull(addedUser);
        Assert.Equal(userDto.Name, addedUser.Name);
    }

    [Fact]
    public async Task SaveChanges_CommitsRoleChangesToDatabase()
    {
        // Arrange
        _dbContext.RolePermissions.Add(new RolePermissions
        {
            RoleId = "1",
            Role = new Role
            {
                Id = "5",
                Name = "Admin",
                RolePermissions = new List<RolePermissions>(),
            },
            PermissionId = "1",
            Permissions = new Permissions
            {
                Id = "1",
                Name = "ViewGame",
                RolePermissions = new List<RolePermissions>(),
            },
        });
        var roleDto = new RoleDto
        {
            Id = "1",
            Name = "User",
        };

        var createdUser = new CreateUpdateRoleDto()
        {
            Role = roleDto,
            Permissions = new[]
            {
                "ViewGame",
            },
        };
        await _unitOfWork.SaveChanges();

        // Act
        var addedRole = await _unitOfWork.RoleRepository.CreateNewRole(createdUser);

        // Assert
        Assert.NotNull(addedRole);
        Assert.Equal(roleDto.Name, addedRole.Name);
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
                _unitOfWork.Dispose();
            }

            _disposed = true;
        }
    }
}
