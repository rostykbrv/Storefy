using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;

namespace Storefy.Services.Services;

/// <summary>
/// Implementation of the services needed to manipulate and
/// retrieve information for Cart entities in the repository.
/// </summary>
/// <inheritdoc cref="ICartService"/>
public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to be used by the cart repository.</param>
    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDetails>> GetCartDetails()
    {
        return await _unitOfWork.CartRepository.GetCartDetails();
    }
}
