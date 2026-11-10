using M.Simple.Extensions.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace M.Simple.Samples;

public static class DatabaseExamples
{
    public static async Task RunAsync()
    {
        // 内存数据库保持连接打开后才能跨命令保留数据。
        var options = new DbContextOptionsBuilder<SampleDatabase>().UseSqlite("Data Source=:memory:").Options;
        await using var context = new SampleDatabase(options);
        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            context.Products.Add(new Product { Id = 1, Name = "事务内产品", Price = 12m });
            await context.SaveChangesAsync();
            var rows = await context.Database.QueryAsync<Product>(
                "SELECT * FROM Products WHERE Id = @Id", new { Id = 1 },
                commandTimeout: 30, cancellationToken: CancellationToken.None);
            Verify.That(rows.Single().Name == "事务内产品", "未读取到当前事务的数据");
            Console.WriteLine($"事务内查询：{rows.Single().Name}");
            await transaction.RollbackAsync();
        }
        var count = await context.Database.QueryFirstOrDefaultAsync<long>("SELECT COUNT(*) FROM Products");
        Verify.That(count == 0, "事务回滚失败");
        Console.WriteLine($"回滚后记录数：{count}");
    }

    public sealed class SampleDatabase(DbContextOptions<SampleDatabase> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
    }
}
