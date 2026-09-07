using Dapper;
using HolyTac.Menu.Application;
using HolyTac.Menu.Domain;
using HolyTac.SharedKernel;

namespace HolyTac.Menu.Infrastructure;

public class MenuItemRepository(IDbConnectionFactory connectionFactory) : IMenuItemRepository
{
    public async Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            var rows = await connection.QueryAsync<MenuItemRow>(
                "SELECT Id, Name, Description, Price, Currency, Category, IsSpicy, IsAvailable, ImageUrl FROM MenuItems ORDER BY Category, Name");
            return (IReadOnlyList<MenuItem>)rows.Select(r => r.ToDomain()).ToList();
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(MenuCategory category, CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            var rows = await connection.QueryAsync<MenuItemRow>(
                "SELECT Id, Name, Description, Price, Currency, Category, IsSpicy, IsAvailable, ImageUrl FROM MenuItems WHERE Category = @Category ORDER BY Name",
                new { Category = (int)category });
            return (IReadOnlyList<MenuItem>)rows.Select(r => r.ToDomain()).ToList();
        }, cancellationToken);
    }

    public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            var row = await connection.QuerySingleOrDefaultAsync<MenuItemRow>(
                "SELECT Id, Name, Description, Price, Currency, Category, IsSpicy, IsAvailable, ImageUrl FROM MenuItems WHERE Id = @Id",
                new { Id = id });
            return row?.ToDomain();
        }, cancellationToken);
    }

    public async Task AddAsync(MenuItem item, CancellationToken cancellationToken = default)
    {
        await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                """
                INSERT INTO MenuItems (Id, Name, Description, Price, Currency, Category, IsSpicy, IsAvailable, ImageUrl)
                VALUES (@Id, @Name, @Description, @Price, @Currency, @Category, @IsSpicy, @IsAvailable, @ImageUrl)
                """,
                new
                {
                    item.Id,
                    item.Name,
                    item.Description,
                    Price = item.Price.Amount,
                    Currency = item.Price.Currency,
                    Category = (int)item.Category,
                    item.IsSpicy,
                    item.IsAvailable,
                    item.ImageUrl
                });
        }, cancellationToken);
    }

    public async Task UpdateAsync(MenuItem item, CancellationToken cancellationToken = default)
    {
        await ResiliencePolicies.DatabasePipeline.ExecuteAsync(async _ =>
        {
            using var connection = connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                """
                UPDATE MenuItems
                SET Name = @Name, Description = @Description, Price = @Price, Currency = @Currency,
                    Category = @Category, IsSpicy = @IsSpicy, IsAvailable = @IsAvailable, ImageUrl = @ImageUrl
                WHERE Id = @Id
                """,
                new
                {
                    item.Id,
                    item.Name,
                    item.Description,
                    Price = item.Price.Amount,
                    Currency = item.Price.Currency,
                    Category = (int)item.Category,
                    item.IsSpicy,
                    item.IsAvailable,
                    item.ImageUrl
                });
        }, cancellationToken);
    }
}
