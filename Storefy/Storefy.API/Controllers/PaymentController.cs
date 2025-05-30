using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefy.BusinessObjects.Dto;
using Storefy.Interfaces.Services;

namespace Storefy.API.Controllers;

/// <summary>
/// Represents the Payment API Controller class.
/// </summary>
[Route("payments/")]
[ApiController]
[Authorize(Policy = "User")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentController"/> class.
    /// </summary>
    /// <param name="paymentService">The payment service to use.</param>
    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Pay for an order using the specified payment method.
    /// </summary>
    /// <param name="payment">The payment request data transfer object.</param>
    /// <returns>An IActionResult containing the payment result.</returns>
    [HttpPost("pay")]
    public async Task<IActionResult> PayForOrder(PaymentRequestDto payment)
    {
        switch (payment.Method)
        {
            case "Visa":
                var visaResult = await _paymentService.ProcessVisaPayment(payment.Model!);
                return Ok(visaResult);
            case "IBox terminal":
                var iBoxResult = await _paymentService.ProcessIboxPayment();
                return Ok(iBoxResult);
            case "Bank":
                var bankResult = await _paymentService.ProcessBankPayment();
                return File(bankResult.FileBytes, bankResult.ContentType, bankResult.FileName);
            default:
                return BadRequest();
        }
    }
}
