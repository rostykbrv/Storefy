using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;

namespace Storefy.Services.Services;

/// <summary>
/// Implementation of the services needed to manipulate and
/// retrieve information for Order entities in the repository.
/// </summary>
/// <inheritdoc cref="IOrderService"/>
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to be used by the order repository.</param>
    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<OrderDetails> AddGameToOrderDetails(string gameId, string orderId)
    {
        return await _unitOfWork.OrderRepository
            .AddGameToOrderDetails(gameId, orderId);
    }

    /// <inheritdoc />
    public async Task<CreateOrderDto> CreateOrder()
    {
        return await _unitOfWork.OrderRepository
            .CreateOrder();
    }

    /// <inheritdoc />
    public async Task<Order> Delete(string gameKey)
    {
        return await _unitOfWork.OrderRepository
            .Delete(gameKey);
    }

    /// <inheritdoc />
    public async Task<OrderDetails> DeleteDetailsFromOrder(string detailsId, string orderId)
    {
        return await _unitOfWork.OrderRepository
            .DeleteDetailsFromOrder(detailsId, orderId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetAll()
    {
        return await _unitOfWork.OrderRepository
            .GetAllOrders();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderHistoryDto>> GetAllOrders(string? start, string? end)
    {
        var sqlOrders = await _unitOfWork.OrderRepository
            .GetOrdersHistory(start, end);
        var returnedOrderHistory = sqlOrders;

        return returnedOrderHistory;
    }

    /// <inheritdoc />
    public async Task<Order> GetOrder(string id)
    {
        var returnedOrder = await _unitOfWork.OrderRepository
            .GetOrder(id);

        return returnedOrder;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDetails>> GetOrderDetails(string id)
    {
        var orderDetailsSql = await _unitOfWork.OrderRepository.GetOrderDetails(id);
        var returnedOrderDetails = new List<OrderDetails>();
        returnedOrderDetails.AddRange(orderDetailsSql);

        return returnedOrderDetails;
    }

    /// <inheritdoc />
    public async Task<Order> ShipOrder(string id)
    {
        return await _unitOfWork.OrderRepository
            .ShipOrder(id);
    }

    /// <inheritdoc />
    public async Task<OrderDetails> UpdateOrderDetailsQuantity(UpdateQuantityDto count, string id)
    {
        var orderDetails = await _unitOfWork.OrderRepository
            .UpdateOrderDetailsQuantity(count, id);

        return orderDetails;
    }
}
