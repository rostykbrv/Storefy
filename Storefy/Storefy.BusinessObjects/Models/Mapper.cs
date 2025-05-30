using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.BusinessObjects.Models;

/// <summary>
/// Represents class for mapping MongoDb
/// entities to Sql entities.
/// </summary>
public static class Mapper
{
    /// <summary>
    /// Maps Order model to OrderHistoryDto model.
    /// </summary>
    public static OrderHistoryDto MapOrderToOrderHistoryDto(Order order)
    {
        var returnedOrderHistory = new OrderHistoryDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
        };

        return returnedOrderHistory;
    }
}
