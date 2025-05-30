namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents a data transfer object of orders history.
/// </summary>
public class OrderHistoryDto
{
    /// <summary>
    /// Gets or sets the unique identification number of the customer.
    /// </summary>
    public string CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the unique identification number of the order.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the ordered date.
    /// </summary>
    public string OrderDate { get; set; }
}
