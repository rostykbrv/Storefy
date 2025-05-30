using Microsoft.EntityFrameworkCore;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Repositories.Gamestore;
using Storefy.Services.Data;

namespace Storefy.Services.Repositories.Gamestore;

/// <summary>
/// Repository to manage the operations of Cart entities using a database context.
/// </summary>
/// <inheritdoc cref="ICartRepository"/>
public class CartRepository : ICartRepository
{
    private readonly StorefyDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The data base context to be used by the repository.</param>
    public CartRepository(StorefyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDetails>> GetCartDetails()
    {
        var ordersCart = await _dbContext.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Game)
            .Where(o => o.PaidDate == null && o.Status == OrderStatus.Open)
            .ToListAsync();

        var allOrderDetails = new List<OrderDetails>();

        foreach (var order in ordersCart)
        {
            allOrderDetails.AddRange(order.OrderDetails);
        }

        return allOrderDetails;
    }
}
