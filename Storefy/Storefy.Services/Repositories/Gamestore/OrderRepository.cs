using Microsoft.EntityFrameworkCore;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Repositories.Gamestore;
using Storefy.Services.Data;

namespace Storefy.Services.Repositories.Gamestore;

/// <summary>
/// Repository to manage the operations of Order entities using a database context.
/// </summary>
/// <inheritdoc cref="IOrderRepository"/>
public class OrderRepository : IOrderRepository
{
    private readonly StorefyDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The data base context
    /// to be used by the repository.</param>
    public OrderRepository(StorefyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Order> GetOrder(string id)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        return order ??
            throw new InvalidOperationException("This order doesn't exist!");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderHistoryDto>> GetOrdersHistory(string? start, string? end)
    {
        var returnedOrders = await _dbContext.Orders.Where(o => o.Status == OrderStatus.Paid &&
            o.OrderDate >= DateTime.Now.AddDays(-30) &&
            o.OrderDate <= DateTime.Now)
            .ToListAsync();
        var returnedHistoryOrders = new List<OrderHistoryDto>();

        foreach (var order in returnedOrders)
        {
            returnedHistoryOrders.Add(Mapper.MapOrderToOrderHistoryDto(order));
        }

        return returnedHistoryOrders;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetAllOrders()
    {
        var returnedOrders = await _dbContext.Orders
            .Include(o => o.OrderDetails)
            .ToListAsync();

        return returnedOrders;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDetails>> GetOrderDetails(string id)
    {
        var orderDetails = await _dbContext.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Game)
            .Where(o => o.Id.Equals(id))
            .SelectMany(o => o.OrderDetails)
            .ToListAsync();

        return orderDetails;
    }

    /// <inheritdoc />
    public async Task<OrderDetails> UpdateOrderDetailsQuantity(UpdateQuantityDto count, string id)
    {
        var orderDetails = await _dbContext.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Game)
            .SelectMany(o => o.OrderDetails)
            .Where(od => od.Id == id)
            .FirstOrDefaultAsync();

        orderDetails.Quantity = count.Count;
        await _dbContext.SaveChangesAsync();

        return orderDetails;
    }

    /// <inheritdoc />
    public async Task<OrderDetails> AddGameToOrderDetails(string gameId, string orderId)
    {
        var game = await _dbContext.Games
            .Where(g => g.Id == gameId)
            .FirstOrDefaultAsync();

        if (game != null)
        {
            var order = await _dbContext.Orders
           .Include(o => o.OrderDetails)
           .ThenInclude(od => od.Game)
           .Where(o => o.Id == orderId)
           .FirstOrDefaultAsync();

            if (order != null)
            {
                var newOrderDetails = new OrderDetails()
                {
                    Id = Guid.NewGuid().ToString(),
                    Game = game,
                    CreationDate = DateTime.Now,
                    Discount = 0,
                    OrderId = order.OrderId,
                    Quantity = 1,
                    ProductId = game.Id,
                    ProductName = game.Name,
                    Price = game.Price,
                };

                order.OrderDetails.Add(newOrderDetails);
                await _dbContext.SaveChangesAsync();

                return newOrderDetails;
            }

            throw new InvalidDataException("Invalid order.");
        }

        throw new InvalidDataException("Invalid game.");
    }

    /// <inheritdoc />
    public async Task<OrderDetails> DeleteDetailsFromOrder(string detailsId, string orderId)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order != null)
        {
            var orderDetails = order.OrderDetails.FirstOrDefault(od => od.Id == detailsId);

            if (orderDetails != null)
            {
                order.OrderDetails.Remove(orderDetails);
                await _dbContext.SaveChangesAsync();

                return orderDetails;
            }
        }

        throw new InvalidDataException();
    }

    /// <inheritdoc />
    public async Task<Order> ShipOrder(string id)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order != null)
        {
            order.ShippedDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return order;
        }

        throw new InvalidDataException("Invalid order.");
    }

    /// <inheritdoc />
    public async Task<CreateOrderDto> CreateOrder()
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderDetails)
            .Where(o => o.PaidDate == null)
            .FirstAsync();
        var paymentMethods = await _dbContext.PaymentMethods.ToListAsync();
        var returnedOrderInfo = new CreateOrderDto()
        {
            Order = order,
            PaymentMethods = paymentMethods,
        };

        return returnedOrderInfo;
    }

    /// <inheritdoc />
    public async Task<Order> Delete(string gameKey)
    {
        var game = await _dbContext.Games
            .FirstAsync(g => g.Key.Equals(gameKey));
        var order = await _dbContext.Orders
            .Include(o => o.OrderDetails)
            .Where(o => o.Status == OrderStatus.Open)
            .FirstAsync();
        var orderDetailToRemove = order.OrderDetails
            .FirstOrDefault(od => od.Game.Id == game.Id);

        if (orderDetailToRemove != null)
        {
            order.OrderDetails.Remove(orderDetailToRemove);
            order.Sum = order.OrderDetails
                .Sum(od => od.Price * od.Quantity);

            await _dbContext.SaveChangesAsync();
        }

        return order;
    }

    /// <inheritdoc />
    public async Task<Order> GetActiveOrder()
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Game)
            .Where(o => o.Status == OrderStatus.Open)
            .FirstAsync();

        return order;
    }

    /// <inheritdoc />
    public async Task CompleteOrder()
    {
        var order = await GetActiveOrder();
        order.PaidDate = DateTime.Now;
        order.Status = OrderStatus.Paid;
        var orderedGames = order.OrderDetails
            .Select(od => od.Game);

        foreach (var game in orderedGames)
        {
            game.UnitInStock--;
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task CancelledOrder()
    {
        var order = await GetActiveOrder();
        order.Status = OrderStatus.Cancelled;
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task OrderCheckout()
    {
        var order = await GetActiveOrder();
        order.Status = OrderStatus.Checkout;
        await _dbContext.SaveChangesAsync();
    }
}
