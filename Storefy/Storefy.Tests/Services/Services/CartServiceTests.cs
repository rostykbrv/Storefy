using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Services.Services;

namespace Storefy.Tests.Services.Services;
public class CartServiceTests
{
    private readonly CartService _cartService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CartServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cartService = new CartService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetCartDetails_ReturnsCartDetails()
    {
        // Arrange
        var cartDetails = new List<OrderDetails>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
            },
        };
        _unitOfWorkMock.Setup(uow => uow.CartRepository.GetCartDetails())
            .ReturnsAsync(cartDetails);

        // Act
        var result = await _cartService.GetCartDetails();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cartDetails.Count, result.Count());
    }
}
