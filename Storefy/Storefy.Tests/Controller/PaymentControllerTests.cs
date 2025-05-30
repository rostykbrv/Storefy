using Microsoft.AspNetCore.Mvc;
using Storefy.API.Controllers;
using Storefy.BusinessObjects.Dto;
using Storefy.Interfaces.Services;

namespace Storefy.Tests.Controller;
public class PaymentControllerTests
{
    private readonly PaymentController _controller;
    private readonly Mock<IPaymentService> _paymentService;

    public PaymentControllerTests()
    {
        _paymentService = new Mock<IPaymentService>();
        _controller = new PaymentController(_paymentService.Object);
    }

    [Fact]
    public async Task PayForOrder_InvalidPaymentMethod_ReturnsBadRequest()
    {
        // Arrange
        var paymentRequest = new PaymentRequestDto
        {
            Method = "asdasdas",
        };

        // Act
        var result = await _controller.PayForOrder(paymentRequest);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PayForOrder_PayWithVisa_ReturnsVisaResult()
    {
        // Arrange
        var visaTransaction = new VisaTransactionDto
        {
            CardNumber = "123456789",
            CVV2 = 123,
            Holder = "Juan",
            MonthExpire = 12,
            TransactionAmount = 500,
            YearExpire = 2029,
        };

        var paymentRequest = new PaymentRequestDto
        {
            Method = "Visa",
            Model = visaTransaction,
        };

        var returned = true;
        _paymentService.Setup(s => s.ProcessVisaPayment(It.IsAny<VisaTransactionDto>())).ReturnsAsync(returned);

        // Act
        var result = await _controller.PayForOrder(paymentRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<bool>(okResult.Value);
        Assert.True(returnedResult);
    }

    [Fact]
    public async Task PayForOrder_PayWithIbox_ReturnsIboxResult()
    {
        // Arrange
        var iboxResponce = new IboxResponceDto
        {
            UserId = Guid.NewGuid().ToString(),
            OrderId = Guid.NewGuid().ToString(),
            Sum = 222,
        };

        var paymentRequest = new PaymentRequestDto
        {
            Method = "IBox terminal",
        };

        _paymentService.Setup(s => s.ProcessIboxPayment()).ReturnsAsync(iboxResponce);

        // Act
        var result = await _controller.PayForOrder(paymentRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<IboxResponceDto>(okResult.Value);

        Assert.Equal(iboxResponce.Sum, returnedResult.Sum);
        Assert.Equal(iboxResponce.UserId, returnedResult.UserId);
        Assert.Equal(iboxResponce.OrderId, returnedResult.OrderId);
    }

    [Fact]
    public async Task PayForOrder_PayWithBank_ReturnsBankResult()
    {
        // Arrange
        var bankResult = new BusinessObjects.Models.GameStoreSql.FileResult(
            fileName: "Bank_Invoice.pdf",
            fileBytes: new byte[] { 1, 2, 3, 4 },
            contentType: "application/pdf");

        var paymentRequest = new PaymentRequestDto
        {
            Method = "Bank",
        };

        _paymentService.Setup(s => s.ProcessBankPayment()).ReturnsAsync(bankResult);

        // Act
        var result = await _controller.PayForOrder(paymentRequest);

        // Assert
        var returnedFile = Assert.IsType<FileContentResult>(result);
        Assert.Equal(bankResult.FileName, returnedFile.FileDownloadName);
        Assert.Equal(bankResult.ContentType, returnedFile.ContentType);
        Assert.Equal(bankResult.FileBytes, returnedFile.FileContents);
    }
}
