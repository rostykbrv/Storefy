using System.Net.Http.Json;
using System.Transactions;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;
using Storefy.Interfaces.Services.Notifications;

namespace Storefy.Services.Services;

/// <summary>
/// Implementation of the services needed to manipulate and retrieve information for Payment entities.
/// </summary>
/// <inheritdoc cref="IPaymentService"/>
public class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentService"/> class.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
    /// <param name="unitOfWork">The IUnitOfWork instance used for performing operations on the repositories.</param>
    public PaymentService(
        HttpClient httpClient,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _httpClient = httpClient;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    private static string ApiUrl => "https://localhost:5001/api/payments";

    /// <inheritdoc />
    public async Task<bool> ProcessVisaPayment(VisaTransactionDto visaTransaction)
    {
        var order = await _unitOfWork.OrderRepository.GetActiveOrder();

        var microserviceVisaTransaction = new MicroserviceVisaTransactionDto
        {
            TransactionAmount = order.Sum,
            CardHolderName = visaTransaction.Holder,
            CardNumber = visaTransaction.CardNumber,
            ExpirationMonth = visaTransaction.MonthExpire,
            ExpirationYear = visaTransaction.YearExpire,
            CVV = visaTransaction.CVV2,
        };

        var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/visa", microserviceVisaTransaction);
        using var visaPaymentTransaction = _unitOfWork.BeginTransaction();

        try
        {
            if (response.IsSuccessStatusCode)
            {
                await _unitOfWork.OrderRepository.CompleteOrder();
                visaPaymentTransaction.Commit();

                await _notificationService
                    .GenerateNotificationAsync(order);

                return true;
            }
            else
            {
                await _unitOfWork.OrderRepository.CancelledOrder();
                visaPaymentTransaction.Commit();

                return false;
            }
        }
        catch
        {
            visaPaymentTransaction.Rollback();
            throw new TransactionAbortedException();
        }
    }

    /// <inheritdoc />
    public async Task<IboxResponceDto> ProcessIboxPayment()
    {
        var order = await _unitOfWork.OrderRepository.GetActiveOrder();

        var iboxTransaction = new IboxTransactionDto
        {
            TransactionAmount = order.Sum,
            AccountNumber = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            InvoiceNumber = Guid.NewGuid().ToString(),
        };

        var responce = await _httpClient.PostAsJsonAsync($"{ApiUrl}/ibox", iboxTransaction);
        using var iboxPaymentTransaction = _unitOfWork.BeginTransaction();

        try
        {
            if (responce.IsSuccessStatusCode)
            {
                await _unitOfWork.OrderRepository.CompleteOrder();
                iboxPaymentTransaction.Commit();

                await _notificationService
                    .GenerateNotificationAsync(order);
            }
            else
            {
                await _unitOfWork.OrderRepository.CancelledOrder();
                iboxPaymentTransaction.Commit();
            }
        }
        catch
        {
            iboxPaymentTransaction.Rollback();
            throw new TransactionAbortedException();
        }

        var result = new IboxResponceDto
        {
            OrderId = order.Id,
            UserId = order.CustomerId,
            Sum = order.Sum,
        };

        return result;
    }

    /// <inheritdoc />
    public async Task<FileResult> ProcessBankPayment()
    {
        var order = await _unitOfWork.OrderRepository.GetActiveOrder();
        var invoice = new Invoice
        {
            OrderId = order.Id,
            UserId = order.CustomerId,
            ValidUntil = order.OrderDate.AddDays(3),
            Sum = order.Sum,
        };

        byte[] contentBytes;
        using (var ms = new MemoryStream())
        {
            PdfWriter writer = new(ms);
            PdfDocument pdf = new(writer);

            Document document = new(pdf, iText.Kernel.Geom.PageSize.A4);

            var content = $"Bank invoice:\nOrderId = {invoice.OrderId}" +
                $"\nUserId = {invoice.UserId}" +
                $"\nValid until - {invoice.ValidUntil}" +
                $"\nSum = {invoice.Sum}";

            var paragraph = new Paragraph(content);

            document.Add(paragraph);

            document.Close();

            contentBytes = ms.ToArray();
        }

        var filename = $"BankInvoice_{DateTime.Now}.pdf";
        await _unitOfWork.OrderRepository.OrderCheckout();
        await _notificationService
            .GenerateNotificationAsync(order);

        return new FileResult(contentBytes, "application/pdf", filename);
    }
}