using Microsoft.EntityFrameworkCore;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Services.Data;
using Storefy.Services.Repositories.Gamestore;

namespace Storefy.Tests.Services.Repositories.Gamestore;
public class CartRepositoryTests : IDisposable
{
    private readonly CartRepository _cartRepository;
    private readonly StorefyDbContext _dbContext;
    private bool _disposed;

    public CartRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StorefyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

        _dbContext = new StorefyDbContext(options);
        _cartRepository = new CartRepository(_dbContext);
    }

    [Fact]
    public async Task GetCartDetails_ReturnedOrderDetails()
    {
        // Arrange
        var testGame = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test 1",
            Description = "Description 1",
            Key = "test-1",
            Price = 100,
            UnitInStock = 50,
            Discontinued = 1,
            CopyType = "Full Game",
            ImageUrl = "image",
            ReleasedDate = "21 Jan, 2021",
            OrderDetails = new List<OrderDetails>(),
        };

        var orderDetails = new OrderDetails
        {
            Id = "222",
            ProductName = "Test",
            Price = 12,
            CreationDate = DateTime.Now,
            Discount = 0,
            ProductId = "22",
            Quantity = 10,
            Game = testGame,
        };

        var orderDetailsOrder = new List<OrderDetails>
        {
            orderDetails,
        };

        var openOrder = new Order
        {
            Id = "22",
            Status = OrderStatus.Open,
            PaidDate = null,
            OrderDate = DateTime.Now,
            CustomerId = Guid.NewGuid().ToString(),
            Sum = 25,
            OrderDetails = orderDetailsOrder,
        };

        _dbContext.Orders.Add(openOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartRepository.GetCartDetails();

        // Assert
        Assert.Single(result);
        var returnedOrderDetails = result.First();

        Assert.Equal(orderDetailsOrder[0].Id, returnedOrderDetails.Id);
        Assert.Equal(orderDetailsOrder[0].ProductName, returnedOrderDetails.ProductName);
        Assert.Equal(orderDetailsOrder[0].Price, returnedOrderDetails.Price);
        Assert.Equal(orderDetailsOrder[0].Discount, returnedOrderDetails.Discount);
        Assert.Equal(orderDetailsOrder[0].ProductId, returnedOrderDetails.ProductId);
        Assert.Equal(orderDetailsOrder[0].Quantity, returnedOrderDetails.Quantity);
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
