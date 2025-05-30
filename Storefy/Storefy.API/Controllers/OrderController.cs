using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.API.Controllers;

/// <summary>
/// Represents the Order API Controller class.
/// </summary>
[Route("orders/")]
[ApiController]
[Authorize(Policy = "Manager")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;
    private readonly IMemoryCache _memmoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderController"/> class.
    /// </summary>
    /// <param name="orderService">Service to perform operations on Order.</param>
    /// <param name="logger">Logger service for logging.</param>
    public OrderController(
        IOrderService orderService,
        ILogger<OrderController> logger,
        IMemoryCache memoryCache)
    {
        _memmoryCache = memoryCache;
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// HTTP Get method to get all orders history.
    /// </summary>
    /// <returns>All orders if successful, else an error.</returns>
    [HttpGet("history")]
    public async Task<ActionResult<OrderHistoryDto>> GetAllOrders(string? start, string? end)
    {
        var orders = await _orderService.GetAllOrders(start, end);
        _logger.LogDebug("Successfuly get all completed orders.");

        return Ok(orders);
    }

    /// <summary>
    /// HTTP Get method to get all orders history.
    /// </summary>
    /// <returns>All orders if successful, else an error.</returns>
    [HttpGet]
    public async Task<ActionResult<Order>> GetOrders()
    {
        var orders = await _orderService.GetAll();
        _logger.LogDebug("Successfuly get all orders.");

        return Ok(orders);
    }

    /// <summary>
    /// HTTP Get method to get order details by id.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>Order details with a certain id.</returns>
    [HttpGet("details/{id}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrderDetails(string id)
    {
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _memmoryCache.Set(userId, id, cacheOptions);

        var orderDetails = await _orderService.GetOrderDetails(id);

        if (orderDetails != null)
        {
            _logger.LogDebug("Successfuly get order details.");

            return Ok(orderDetails);
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// HTTP Get method to get order information by id.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>Order information with a certain id..</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(string id)
    {
        var order = await _orderService.GetOrder(id);

        if (order != null)
        {
            _logger.LogDebug("Successfuly get order.");

            return Ok(order);
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// HTTP method to create order.
    /// </summary>
    /// <returns>Created order information.</returns>
    [HttpGet("createOrder")]
    public async Task<ActionResult<CreateOrderDto>> MakeOrderInfo()
    {
        var order = await _orderService.CreateOrder();
        _logger.LogDebug("Successfuly create order.");

        return Ok(order);
    }

    /// <summary>
    /// HTTP Delete method to delete order.
    /// </summary>
    /// <param name="gamekey">The game alias of the game, including in order.</param>
    /// <returns>Deleted order.</returns>
    [HttpDelete("remove/{gamekey}")]
    public async Task<ActionResult<Order>> DeleteOrder(string gamekey)
    {
        var order = await _orderService.Delete(gamekey);
        _logger.LogDebug("Successfuly delete order details.");

        return Ok(order);
    }

    /// <summary>
    /// HTTP Put method to add order details to order.
    /// </summary>
    /// <param name="count">Quantity of game to change.</param>
    /// <param name="id">Order Detail's id.</param>
    /// <returns>Updated order details.</returns>
    [HttpPut("details/{id}/quantity")]
    public async Task<ActionResult<OrderDetails>> UpdateOrderDetailsQuantity([FromBody] UpdateQuantityDto count, string id)
    {
        var orderDetails = await _orderService.UpdateOrderDetailsQuantity(count, id);

        return Ok(orderDetails);
    }

    /// <summary>
    /// HTTP Delete method to delete order details from order.
    /// </summary>
    /// <param name="id">Order detail's id.</param>
    /// <returns>Deleted order details from order.</returns>
    [HttpDelete("details/{id}/remove")]
    public async Task<ActionResult<OrderDetails>> RemoveOrderDetails(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orderId = string.Empty;

        if (_memmoryCache.TryGetValue(userId, out string cachedId))
        {
            orderId = cachedId;
        }

        var orderDetails = await _orderService.DeleteDetailsFromOrder(id, orderId!);

        return Ok(orderDetails);
    }

    /// <summary>
    /// HTTP Post method to add Game to Order Details.
    /// </summary>
    /// <param name="id">Order Detail's id.</param>
    /// <returns>Created order details.</returns>
    [HttpPost("{id}/details/add")]
    public async Task<ActionResult<OrderDetails>> AddGameToOrderDetails(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orderId = string.Empty;

        if (_memmoryCache.TryGetValue(userId, out string cachedId))
        {
            orderId = cachedId;
        }

        var orderDetails = await _orderService.AddGameToOrderDetails(id, orderId!);

        return Ok(orderDetails);
    }

    /// <summary>
    /// HTTP Post method to change order status to shipped.
    /// </summary>
    /// <param name="id">Order's id.</param>
    /// <returns>Order with changed status.</returns>
    [HttpPost("{id}/ship")]
    public async Task<ActionResult<Order>> ShipOrder(string id)
    {
        var order = await _orderService.ShipOrder(id);

        return Ok(order);
    }
}
