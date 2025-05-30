using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Repositories.Gamestore;

/// <summary>
/// Interface for the Cart Repository. Defines the repository operations for the Cart objects.
/// </summary>
public interface ICartRepository
{
    /// <summary>
    /// Retrieves all order details in the cart.
    /// </summary>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains a collection of cart order details.</returns>
    Task<IEnumerable<OrderDetails>> GetCartDetails();
}
