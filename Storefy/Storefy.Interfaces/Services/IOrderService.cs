using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Services;

/// <summary>
/// Interface for the Order Service. Defines the service operations related to Orders.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Retrieves all Orders filtered by date.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous retrieval operation.
    /// Task result contains a collection
    /// of all filtered by dateOrders.</returns>
    Task<IEnumerable<OrderHistoryDto>> GetAllOrders(string? start, string? end);

    /// <summary>
    /// Retrieves all Orders.
    /// </summary>
    /// <returns>A task representing the asynchronous retrieval operation.
    /// Task result contains a collection of all Orders.</returns>
    Task<IEnumerable<Order>> GetAll();

    /// <summary>
    /// Retrieves an Order by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the Order to retrieve.</param>
    /// <returns>A task representing the asynchronous retrieval operation.
    /// Task result contains the retrieved Order object.</returns>
    Task<Order> GetOrder(string id);

    /// <summary>
    /// Retrieves the OrderDetails for the specified order.
    /// </summary>
    /// <param name="id">The unique identifier of the order to retrieve its details.</param>
    /// <returns>A task representing the asynchronous retrieval operation.
    /// Task result contains a collection of
    /// OrderDetails associated with the given order.</returns>
    Task<IEnumerable<OrderDetails>> GetOrderDetails(string id);

    /// <summary>
    /// Updates the quantity for a particular detail in an order.
    /// </summary>
    /// <param name="count">Updated count/quantity information.</param>
    /// <param name="id">The id of the specific order detail to be updated.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result contains the updated order detail.</returns>
    Task<OrderDetails> UpdateOrderDetailsQuantity(UpdateQuantityDto count, string id);

    /// <summary>
    /// Adds a game to the order details of a specific order.
    /// </summary>
    /// <param name="gameId">The id of the game to be added to the order details.</param>
    /// <param name="orderId">The id of the order to which the game is to be added.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result contains the updated order details after adding the game.</returns>
    Task<OrderDetails> AddGameToOrderDetails(string gameId, string orderId);

    /// <summary>
    /// Deletes a specific detail from an order.
    /// </summary>
    /// <param name="detailsId">The id of the order detail to be deleted.</param>
    /// <param name="orderId">The id of the order from which the detail is to be deleted.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result contains the updated order details after deleting the detail.</returns>
    Task<OrderDetails> DeleteDetailsFromOrder(string detailsId, string orderId);

    /// <summary>
    /// Marks an order as shipped.
    /// </summary>
    /// <param name="id">The id of the order to be shipped.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result contains the updated order after marking it as shipped.</returns>
    Task<Order> ShipOrder(string id);

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <returns>A task representing the asynchronous creation operation.
    /// Task result contains the created order as a CreateOrderDto object.</returns>
    Task<CreateOrderDto> CreateOrder();

    /// <summary>
    /// Deletes an Order using the provided game key.
    /// </summary>
    /// <param name="gameKey">The game key associated with the order to delete.</param>
    /// <returns>A task representing the asynchronous operation.
    /// Task result contains the deleted Order object.</returns>
    Task<Order> Delete(string gameKey);
}
