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
        public OrderReadModelRepository(ReadModelDBContext context, IDbConnection dbconnection) :base(dbconnection,context)
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
            o.Id AS OrderId,
            c.Name AS CustomerName,
            a.Street AS Street,
            a.City AS City,
            a.State AS State,
            a.ZipCode AS ZipCode,
            o.DeliveryDate AS DeliveryDate,
            o.TotalAmount AS Amount,
            o.Currency AS Currency,
            oi.Id AS ItemId,
            oi.ProductName AS ProductName,
            oi.Quantity AS Count,
            oi.Unit AS Unit,
            oi.Price AS Amount,
            oi.Currency AS Currency
        FROM {TableName<OrderReadModel>()} o
        LEFT JOIN {TableName<OrderItemReadModel>()} oi ON o.Id = oi.OrderId
        WHERE c.Name = @CustomerName
        ORDER BY o.Id, oi.Id";

            var orderDictionary = new Dictionary<Guid, OrderReadModelDto>();

            // Execute the query with multi-mapping
            await _dbConnection.QueryAsync<OrderReadModelDto, OrderItemReadModelDto, OrderReadModelDto>(
                sql,
                (order, item) =>
                {
                    // Check if we've seen this order before
                    if (!orderDictionary.TryGetValue(order.OrderId, out var orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.Items = new List<OrderItemReadModelDto>();
                        orderDictionary.Add(order.OrderId, orderEntry);
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

            return orderDictionary.Values;
        }
    }
}
