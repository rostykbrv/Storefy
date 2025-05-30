namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents a data transfer object for a response from an IBox terminal payment.
/// </summary>
public class IboxResponceDto
{
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public string OrderId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the sum of the payment.
    /// </summary>
    public decimal Sum { get; set; }
}
