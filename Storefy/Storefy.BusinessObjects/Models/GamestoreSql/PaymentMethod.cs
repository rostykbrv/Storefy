namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents a PaymentMethod model in the business objects layer.
/// </summary>
public class PaymentMethod
{
    /// <summary>
    /// Gets or sets the unique identification of the PaymentMethod.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the ImageUrl of the PaymentMethod.
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the title of the PaymentMethod.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the PaymentMethod.
    /// </summary>
    public string Description { get; set; }
}
