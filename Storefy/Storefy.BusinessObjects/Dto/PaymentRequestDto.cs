namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents a data transfer object for processing a payment request.
/// </summary>
public class PaymentRequestDto
{
    /// <summary>
    /// Gets or sets the payment method.
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets the Visa transaction data transfer object.
    /// </summary>
    public VisaTransactionDto? Model { get; set; }
}
