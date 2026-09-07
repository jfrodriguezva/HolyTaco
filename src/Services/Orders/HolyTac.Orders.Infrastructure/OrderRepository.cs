using Dapper;
using HolyTac.Orders.Application;
using HolyTac.Orders.Domain;
using HolyTac.SharedKernel;

namespace HolyTac.Orders.Infrastructure;

public class OrderRepository(IDbConnectionFactory connectionFactory) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                """
                SELECT Id, TableNumber, CustomerName, Status, CreatedAtUtc FROM Orders WHERE Id = @Id;
                SELECT Id, OrderId, MenuItemId, MenuItemName, UnitPrice, Quantity, Notes FROM OrderItems WHERE OrderId = @Id;
                """,
                new { Id = id });

            var order = await multi.ReadSingleOrDefaultAsync<OrderRow>();
            if (order is null) return null;

            var items = await multi.ReadAsync<OrderItemRow>();
            return order.ToDomain(items);
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetActiveOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                """
                SELECT Id, TableNumber, CustomerName, Status, CreatedAtUtc FROM Orders
                WHERE Status NOT IN (4, 5) ORDER BY CreatedAtUtc;
                SELECT oi.Id, oi.OrderId, oi.MenuItemId, oi.MenuItemName, oi.UnitPrice, oi.Quantity, oi.Notes
                FROM OrderItems oi
                INNER JOIN Orders o ON o.Id = oi.OrderId
                WHERE o.Status NOT IN (4, 5);
                """);

            var orderRows = (await multi.ReadAsync<OrderRow>()).ToList();
            var itemRows = (await multi.ReadAsync<OrderItemRow>()).ToLookup(i => i.OrderId);

            return (IReadOnlyList<Order>)orderRows.Select(o => o.ToDomain(itemRows[o.Id])).ToList();
        }, cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(
                """
                INSERT INTO Orders (Id, TableNumber, CustomerName, Status, CreatedAtUtc)
                VALUES (@Id, @TableNumber, @CustomerName, @Status, @CreatedAtUtc)
                """,
                new { order.Id, order.TableNumber, order.CustomerName, Status = (int)order.Status, order.CreatedAtUtc },
                transaction);

            foreach (var item in order.Items)
            {
                await connection.ExecuteAsync(
                    """
                    INSERT INTO OrderItems (Id, OrderId, MenuItemId, MenuItemName, UnitPrice, Quantity, Notes)
                    VALUES (@Id, @OrderId, @MenuItemId, @MenuItemName, @UnitPrice, @Quantity, @Notes)
                    """,
                    new { item.Id, OrderId = order.Id, item.MenuItemId, item.MenuItemName, item.UnitPrice, item.Quantity, item.Notes },
                    transaction);
            }

            transaction.Commit();
        }, cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                "UPDATE Orders SET Status = @Status WHERE Id = @Id",
                new { order.Id, Status = (int)order.Status });
        }, cancellationToken);
    }
}
