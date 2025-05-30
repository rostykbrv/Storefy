using System.ComponentModel.DataAnnotations;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the status of an Order.
/// </summary>
public enum OrderStatus
{
    Open,
    Checkout,
    Paid,
    Shipped,
    Cancelled,
}

/// <summary>
/// Represents an Order model in the business objects layer.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the unique identification of the Order.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identification of the Order.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the Customer's unique identification related to this Order.
    /// </summary>
    public string CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the Employee's unique identification related to this Order.
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Gets or sets the date when the Order was created.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the Order should be delivered.
    /// </summary>
    public DateTime? RequiredDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the Order was shipped.
    /// </summary>
    public DateTime? ShippedDate { get; set; }

    /// <summary>
    /// Gets or sets the unique identification of the order's shipper.
    /// </summary>
    public int? ShipVia { get; set; }

    /// <summary>
    /// Gets or sets the freight of the Order.
    /// </summary>
    public double? Freight { get; set; }

    /// <summary>
    /// Gets or sets the shipper's name of the Order.
    /// </summary>
    public string? ShipName { get; set; }

    /// <summary>
    /// Gets or sets the shipping address of the Order.
    /// </summary>
    public string? ShipAddress { get; set; }

    /// <summary>
    /// Gets or sets the shipping city of the Order.
    /// </summary>
    public string? ShipCity { get; set; }

    /// <summary>
    /// Gets or sets the shipping region of the Order.
    /// </summary>
    public string? ShipRegion { get; set; }

    /// <summary>
    /// Gets or sets the shipping postal code of the Order.
    /// </summary>
    public string? ShipPostalCode { get; set; }

    /// <summary>
    /// Gets or sets the shipping country of the Order.
    /// </summary>
    public string? ShipCountry { get; set; }

    /// <summary>
    /// Gets or sets the date when the Order was paid.
    /// </summary>
    public DateTime? PaidDate { get; set; }

    /// <summary>
    /// Gets or sets the status of the Order. Default value is OrderStatus.Open.
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Open;

    /// <summary>
    /// Gets or sets the total sum of the Order.
    /// </summary>
    public decimal Sum { get; set; }

    /// <summary>
    /// Gets or sets the collection of the OrderDetails related to this Order.
    /// </summary>
    public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
}
