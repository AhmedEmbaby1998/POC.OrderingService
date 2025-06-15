using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using POC.OrderingService.Query.Abstraction.Dtos.Orders;
using POC.OrderingService.Query.Abstraction.Repositories;
using POC.OrderingService.Query.Contracts.ReadModels.Orders;
using POC.OrderingService.Query.Data;

namespace POC.OrderingService.Query.Repositories
{
    internal class OrderReadModelRepository : BaseReadModelRepository<OrderReadModel>,IOrderReadModelRepository
    {
        public OrderReadModelRepository(ReadModelDBContext context, DbConnection dbConnection) :base(dbConnection,context)
        {
        }

        public async Task<IEnumerable<OrderReadModelDto>> GetCustomerOrders(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Customer name cannot be empty", nameof(name));
            }

             string sql = @$"
       SELECT 
    o.Id AS Id,
    o.CustomerName AS CustomerName,
    o.Street AS Street,
    o.City AS City,
    o.State AS State,
    o.ZipCode AS ZipCode,
    o.DeliveryDate AS DeliveryDate,
    o.Amount AS OrderAmount,
    o.Currency AS OrderCurrency,
    oi.Id AS ItemId,
    oi.ProductName AS ProductName,
    oi.Count AS Count,
    oi.Unit AS Unit,
    oi.Amount AS ItemAmount,
    oi.Currency AS ItemCurrency
        FROM {TableName<OrderReadModel>()} o
        LEFT JOIN {TableName<OrderItemReadModel>()} oi ON o.Id = oi.OrderId
        WHERE o.customerName = @CustomerName
        ORDER BY o.Id, oi.Id";

            var orderDictionary = new Dictionary<Guid, OrderReadModelDto>();
            await _dbConnection.OpenAsync();
            // Execute the query with multi-mapping
            await _dbConnection.QueryAsync<OrderReadModelDto, OrderItemReadModelDto, OrderReadModelDto>(
                sql,
                (order, item) =>
                {
                    // Check if we've seen this order before
                    if (!orderDictionary.TryGetValue(order.Id, out var orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.Items = new List<OrderItemReadModelDto>();
                        orderDictionary.Add(order.Id, orderEntry);
                    }

                    // Add the item if it exists (LEFT JOIN may return null)
                    if (item != null)
                    {
                        ((List<OrderItemReadModelDto>)orderEntry.Items).Add(item);
                    }

                    return orderEntry;
                },
                new { CustomerName = name },
                splitOn: "ItemId"  
            );
            await _dbConnection.CloseAsync();
            return orderDictionary.Values;
        }
    }
}
