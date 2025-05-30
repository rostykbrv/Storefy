using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Services;

/// <summary>
/// Interface for the Cart Service. Defines the service operations related to the Cart.
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Retrieves the details of the items in the cart.
    /// </summary>
    /// <returns>A task that represents the asynchronous retrieval operation.
    /// The task result contains a collection of OrderDetails which constitutes
    /// the details of the items in the cart.</returns>
    Task<IEnumerable<OrderDetails>> GetCartDetails();
}
