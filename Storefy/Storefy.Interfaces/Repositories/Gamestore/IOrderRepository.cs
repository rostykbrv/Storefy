using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Repositories.Gamestore;

/// <summary>
/// Interface for the Order Repository. Defines the repository operations for the Order objects.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Retrieves all Orders from the repository by date.
    /// </summary>
    /// <param name="start">Filter of started date.</param>
    /// <param name="end">Filter of ended date.</param>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains a collection
    /// of filtered all Orders history.</returns>
    Task<IEnumerable<OrderHistoryDto>> GetOrdersHistory(string? start, string? end);

    /// <summary>
    /// Retrieves all Orders from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The result contains a collection of all Orders.</returns>
    Task<IEnumerable<Order>> GetAllOrders();

    /// <summary>
    /// Retrieves an Order based on its unique identification.
    /// </summary>
    /// <param name="id">Unique identifier of the order to retrieve.</param>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains the retrieved Order object.</returns>
    Task<Order> GetOrder(string id);

    /// <summary>
    /// Updates the quantity of a specific OrderDetails.
    /// </summary>
    /// <param name="count">The object containing new quantity.</param>
    /// <param name="id">The order detail's identifier to be updated.</param>
    /// <returns>A task representing the asynchronous operation,
    /// result completes when updated OrderDetails is returned.</returns>
    Task<OrderDetails> UpdateOrderDetailsQuantity(UpdateQuantityDto count, string id);

    /// <summary>
    /// Adds a game to a specific order.
    /// </summary>
    /// <param name="gameId">The identifier of the game to be added.</param>
    /// <param name="orderId">The identifier of the order to which game will be added.</param>
    /// <returns>A task representing the asynchronous operation,
    /// result completes when updated OrderDetails is returned.</returns>
    Task<OrderDetails> AddGameToOrderDetails(string gameId, string orderId);

    /// <summary>
    /// Deletes a specific detail from an order.
    /// </summary>
    /// <param name="detailsId">The identifier of the details to be deleted.</param>
    /// <param name="orderId">The identifier of the order from
    /// which details will be deleted.</param>
    /// <returns>A task representing the asynchronous operation,
    /// result completes when updated OrderDetails is returned.</returns>
    Task<OrderDetails> DeleteDetailsFromOrder(string detailsId, string orderId);

    /// <summary>
    /// Marks an order as shipped.
    /// </summary>
    /// <param name="id">The identifier of the order to be dispatched.</param>
    /// <returns>A task representing the asynchronous operation,
    /// result completes when updated Order is returned.</returns>
    Task<Order> ShipOrder(string id);

    /// <summary>
    /// Retrieves the OrderDetails for the specified order.
    /// </summary>
    /// <param name="id">Unique identifier of the order.</param>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains a collection of OrderDetails associated with the given order.</returns>
    Task<IEnumerable<OrderDetails>> GetOrderDetails(string id);

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the created order as a CreateOrderDto object.</returns>
    Task<CreateOrderDto> CreateOrder();

    /// <summary>
    /// Deletes the specified order using the provided game key.
    /// </summary>
    /// <param name="gameKey">The game key associated with the order to delete.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the deleted Order object.</returns>
    Task<Order> Delete(string gameKey);

    /// <summary>
    /// Retrieves the active order.
    /// </summary>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains the active Order object.</returns>
    Task<Order> GetActiveOrder();

    /// <summary>
    /// Completes the current order.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CompleteOrder();

    /// <summary>
    /// Cancels the current order.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CancelledOrder();

    /// <summary>
    /// Changes the status of the current order to 'Checkout'.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OrderCheckout();
}
