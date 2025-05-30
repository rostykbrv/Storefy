using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.Interfaces.Services;

/// <summary>
/// Interface for the Payment Service. Defines the service operations related to Payments.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Processes a Visa payment using the provided transaction information.
    /// </summary>
    /// <param name="visaTransaction">The Visa transaction details to be processed.</param>
    /// <returns>A task representing the asynchronous operation.
    /// Task result contains a boolean value indicating the success of payment.</returns>
    Task<bool> ProcessVisaPayment(VisaTransactionDto visaTransaction);

    /// <summary>
    /// Processes an Ibox payment.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.
    /// Task result contains the Ibox payment response as an IboxResponceDto object.</returns>
    Task<IboxResponceDto> ProcessIboxPayment();

    /// <summary>
    /// Processes a bank payment.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.
    /// Task result contains the processed payment result as a FileResult object.</returns>
    Task<FileResult> ProcessBankPayment();
}
