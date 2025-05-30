using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents a data transfer object for creating an order.
/// </summary>
public class CreateOrderDto
{
    /// <summary>
    /// Gets or sets the order to be created.
    /// </summary>
    public Order Order { get; set; }

    /// <summary>
    /// Gets or sets the collection of payment methods.
    /// </summary>
    public ICollection<PaymentMethod> PaymentMethods { get; set; }
}
