namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents a data transfer object for an IBox terminal transaction.
/// </summary>
public class IboxTransactionDto
{
    /// <summary>
    /// Gets or sets the transaction amount.
    /// </summary>
    public decimal TransactionAmount { get; set; }

    /// <summary>
    /// Gets or sets the account number for the transaction.
    /// </summary>
    public string AccountNumber { get; set; }

    /// <summary>
    /// Gets or sets the invoice number for the transaction.
    /// </summary>
    public string InvoiceNumber { get; set; }
}
