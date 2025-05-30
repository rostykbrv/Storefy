namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents an Invoice model in the business objects layer.
/// </summary>
public class Invoice
{
    /// <summary>
    /// Gets or sets the unique identification of the User related to this generated Invoice.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique identification of the Order related to this generated Invoice.
    /// </summary>
    public string OrderId { get; set; }

    /// <summary>
    /// Gets or sets the date until which this generated Invoice is valid.
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Gets or sets the total sum of the generated Invoice.
    /// </summary>
    public decimal Sum { get; set; }
}
