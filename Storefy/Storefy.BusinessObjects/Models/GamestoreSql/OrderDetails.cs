using System.ComponentModel.DataAnnotations;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents an OrderDetails model in the business objects layer.
/// </summary>
public class OrderDetails
{
    /// <summary>
    /// Gets or sets the unique identification of the OrderDetails.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the Product's unique identification related to this OrderDetails.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the Product's id related to this OrderDetails.
    /// </summary>
    public string ProductId { get; set; }

    /// <summary>
    /// Gets or sets the Product's name related to this OrderDetails.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Gets or sets the creation date of this OrderDetails.
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the Game instance related to this OrderDetails.
    /// </summary>
    public virtual Game Game { get; set; }

    /// <summary>
    /// Gets or sets the price of the product in this OrderDetails.
    /// </summary>

    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product in this OrderDetails.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the discount applied to the product in this OrderDetails.
    /// </summary>
    public double Discount { get; set; }
}
