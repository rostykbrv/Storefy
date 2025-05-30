namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents a data transfer object for a Microservice Visa transaction.
/// </summary>
public class MicroserviceVisaTransactionDto
{
    /// <summary>
    /// Gets or sets the transaction amount.
    /// </summary>
    public decimal TransactionAmount { get; set; }

    /// <summary>
    /// Gets or sets the cardholder's name.
    /// </summary>
    public string CardHolderName { get; set; }

    /// <summary>
    /// Gets or sets the card number.
    /// </summary>
    public string CardNumber { get; set; }

    /// <summary>
    /// Gets or sets the card's expiration month.
    /// </summary>
    public int ExpirationMonth { get; set; }

    /// <summary>
    /// Gets or sets the card's CVV (Card Verification Value) code.
    /// </summary>
    public int CVV { get; set; }

    /// <summary>
    /// Gets or sets the card's expiration year.
    /// </summary>
    public int ExpirationYear { get; set; }
}
