using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.API.Controllers;

/// <summary>
/// Represents the Cart API Controller class.
/// </summary>
[Route("cart/")]
[ApiController]
[Authorize(Policy = "User")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartController"/> class.
    /// </summary>
    /// <param name="cartService">Service to perform operations on Cart.</param>
    /// <param name="logger">Logger service for logging.</param>
    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// HTTP Get method to retrieve all products in cart.
    /// </summary>
    /// <returns>If successful, it returns all order details.</returns>
    [HttpGet]
    public async Task<ActionResult<OrderDetails>> GetCartDetails()
    {
        var cartOrders = await _cartService.GetCartDetails();
        _logger.LogDebug("Successfully get cart details.");

        return Ok(cartOrders);
    }
}
