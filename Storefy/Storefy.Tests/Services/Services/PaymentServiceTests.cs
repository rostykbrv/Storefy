using System.Net;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Storage;
using Moq.Protected;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services.Notifications;
using Storefy.Services.Services;

namespace Storefy.Tests.Services.Services;
public class PaymentServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockNotificationService = new Mock<INotificationService>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _paymentService = new PaymentService(httpClient, _mockUnitOfWork.Object, _mockNotificationService.Object);
    }

    [Fact]
    public async Task ProcessVisaPayment_ShouldProcessPaymentSuccessfully()
    {
        // Arrange
        _mockUnitOfWork.Setup(x => x.OrderRepository.GetActiveOrder()).ReturnsAsync(new Order { Sum = 100 });
        _mockUnitOfWork.Setup(x => x.OrderRepository.CompleteOrder()).Returns(Task.CompletedTask);

        var mockTransaction = new Mock<IDbContextTransaction>();
        _mockUnitOfWork.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var visaTransaction = new VisaTransactionDto
        {
            Holder = "John Doe",
            CardNumber = "1234567812345678",
            MonthExpire = 1,
            YearExpire = 2025,
            CVV2 = 123,
            TransactionAmount = 111,
        };

        // Act
        var result = await _paymentService.ProcessVisaPayment(visaTransaction);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ProcessVisaPayment_ShouldFailPaymentProcessing()
    {
        // Arrange
        _mockUnitOfWork.Setup(x => x.OrderRepository.GetActiveOrder()).ReturnsAsync(new Order { Sum = 100 });
        _mockUnitOfWork.Setup(x => x.OrderRepository.CancelledOrder()).Returns(Task.CompletedTask);

        var mockTransaction = new Mock<IDbContextTransaction>();
        _mockUnitOfWork.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var visaTransaction = new VisaTransactionDto
        {
            Holder = "John Doe",
            CardNumber = "1234567812345678",
            MonthExpire = 1,
            YearExpire = 2025,
            CVV2 = 123,
            TransactionAmount = 111,
        };

        // Act
        var result = await _paymentService.ProcessVisaPayment(visaTransaction);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ProcessVisaPayment_ShouldRollbackTransactionWhenExceptionOccurs()
    {
        // Arrange
        _mockUnitOfWork.Setup(x => x.OrderRepository.GetActiveOrder()).ReturnsAsync(new Order { Sum = 100 });

        var mockTransaction = new Mock<IDbContextTransaction>();
        _mockUnitOfWork.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

        _mockUnitOfWork.Setup(x => x.OrderRepository.CompleteOrder()).Throws<Exception>();

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var visaTransaction = new VisaTransactionDto
        {
            Holder = "John Doe",
            CardNumber = "1234567812345678",
            MonthExpire = 1,
            YearExpire = 2025,
            CVV2 = 123,
            TransactionAmount = 111,
        };

        // Act & Assert
        var result = await Assert.ThrowsAsync<TransactionAbortedException>(() => _paymentService.ProcessVisaPayment(visaTransaction));
        mockTransaction.Verify(t => t.Rollback(), Times.Once);
    }

    [Fact]
    public async Task ProcessIboxPayment_ShouldRollbackTransactionWhenExceptionOccurs()
    {
        // Arrange
        var order = new Order
        {
            Id = "1",
            Sum = 100,
            CustomerId = "1",
        };

        _mockUnitOfWork.Setup(x => x.OrderRepository.GetActiveOrder()).ReturnsAsync(order);

        var mockTransaction = new Mock<IDbContextTransaction>();
        _mockUnitOfWork.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

        _mockUnitOfWork.Setup(x => x.OrderRepository.CompleteOrder()).Throws<Exception>();

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
           "SendAsync",
           ItExpr.IsAny<HttpRequestMessage>(),
           ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var result = await Assert.ThrowsAsync<TransactionAbortedException>(() => _paymentService.ProcessIboxPayment());

        // Assert
        mockTransaction.Verify(t => t.Rollback(), Times.Once);
    }

    [Fact]
    public async Task ProcessIboxPayment_ShouldProcessPaymentSuccessfully()
    {
        // Arrange
        var order = new Order
        {
            Id = "1",
            Sum = 100,
            CustomerId = "1",
        };
        _mockUnitOfWork.Setup(x => x.OrderRepository.GetActiveOrder()).ReturnsAsync(order);
        _mockUnitOfWork.Setup(x => x.OrderRepository.CompleteOrder()).Returns(Task.CompletedTask);
        var mockTransaction = new Mock<IDbContextTransaction>();
        _mockUnitOfWork.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var result = await _paymentService.ProcessIboxPayment();

        // Assert
        Assert.Equal(order.Id, result.OrderId);
        Assert.Equal(order.CustomerId, result.UserId);
        Assert.Equal(order.Sum, result.Sum);
    }

    [Fact]
    public async Task ProcessIboxPayment_ShouldFailPaymentProcessing()
    {
        // Arrange
        var order = new Order
        {
            Id = "1",
            Sum = 100,
            CustomerId = "1",
        };
        _mockUnitOfWork.Setup(x => x.OrderRepository.GetActiveOrder()).ReturnsAsync(order);
        _mockUnitOfWork.Setup(x => x.OrderRepository.CancelledOrder()).Returns(Task.CompletedTask);
        var mockTransaction = new Mock<IDbContextTransaction>();
        _mockUnitOfWork.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        // Act
        var result = await _paymentService.ProcessIboxPayment();

        // Assert
        Assert.Equal(order.Id, result.OrderId);
        Assert.Equal(order.CustomerId, result.UserId);
        Assert.Equal(order.Sum, result.Sum);
    }

    [Fact]
    public async Task ProcessBankPayment_ShouldReturnFileResultWithCorrectInfo()
    {
        // Arrange
        var order = new Order
        {
            Id = "1",
            Sum = 100,
            CustomerId = "1",
        };
        var expectedContentType = "application/pdf";
        var expectedFileName = "BankInvoice_";
        _mockUnitOfWork.Setup(x => x.OrderRepository.GetActiveOrder())
            .ReturnsAsync(order);
        _mockUnitOfWork.Setup(x => x.OrderRepository.OrderCheckout()).Returns(Task.CompletedTask);

        // Act
        FileResult fileResult = await _paymentService.ProcessBankPayment();

        // Assert
        Assert.IsType<FileResult>(fileResult);
        Assert.Equal(expectedContentType, fileResult.ContentType);
        Assert.StartsWith(expectedFileName, fileResult.FileName);
        Assert.NotNull(fileResult.FileBytes);
        Assert.NotEmpty(fileResult.FileBytes);
    }
}
