using System.Data;
using System.Linq.Expressions;
using M.Simple.Extensions.EntityFrameworkCore;
using M.Simple.Extensions.Expressions;
using Microsoft.EntityFrameworkCore;

namespace M.Simple.Tests;

[TestClass]
public class DatabaseExtensionsTests
{
    [TestMethod]
    public async Task QueryBridgeSharesTransactionAndKeepsHostConnectionOpen()
    {
        await using var database = await CreateDatabaseAsync();
        await using (var transaction = await database.Database.BeginTransactionAsync())
        {
            await database.Database.ExecuteSqlRawAsync("INSERT INTO Records (Id, Name) VALUES (1, 'test')");
            var rows = await database.Database.QueryAsync<Record>("SELECT * FROM Records WHERE Id = @Id", new { Id = 1 });
            Assert.AreEqual("test", rows.Single().Name);
            IDictionary<string, object> row = await database.Database.QueryFirstOrDefaultAsync("SELECT * FROM Records WHERE Id = @Id", new { Id = 1 });
            Assert.AreEqual("test", row["Name"]);
            Assert.AreEqual(1L, row["Id"]);
            await transaction.RollbackAsync();
        }
        Assert.AreEqual(0L, await database.Database.QueryFirstOrDefaultAsync<long>("SELECT COUNT(*) FROM Records"));
        Assert.AreEqual(ConnectionState.Open, database.Database.GetDbConnection().State);
        Assert.IsNull(await database.Database.QueryFirstOrDefaultAsync("SELECT * FROM Records"));
        Assert.AreEqual(0, (await database.Database.QueryAsync("SELECT * FROM Records")).Count());
    }

    [TestMethod]
    public async Task CancelledQueryIsNotExecuted()
    {
        await using var database = await CreateDatabaseAsync();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        try
        {
            await database.Database.QueryAsync<Record>("SELECT * FROM Records", cancellationToken: cancellation.Token);
            Assert.Fail("已取消查询不应成功。");
        }
        catch (OperationCanceledException) { }
    }

    [TestMethod]
    public async Task CombinedPredicatesTranslateThroughEfCore()
    {
        await using var database = await CreateDatabaseAsync();
        database.Records.AddRange(new Record { Id = 1, Name = "M.Simple" }, new Record { Id = 2, Name = "Other" });
        await database.SaveChangesAsync();
        Expression<Func<Record, bool>> startsWith = row => row.Name.StartsWith("M.");
        Expression<Func<Record, int>> id = row => row.Id;
        var predicate = startsWith.And(id.In(new[] { 1, 3 }));
        Assert.AreEqual(1, (await database.Records.Where(predicate).SingleAsync()).Id);
    }

    private static async Task<ProbeDatabase> CreateDatabaseAsync()
    {
        var database = new ProbeDatabase(new DbContextOptionsBuilder<ProbeDatabase>().UseSqlite("Data Source=:memory:").Options);
        await database.Database.OpenConnectionAsync();
        await database.Database.EnsureCreatedAsync();
        return database;
    }

    public class ProbeDatabase(DbContextOptions<ProbeDatabase> options) : DbContext(options)
    {
        public DbSet<Record> Records => Set<Record>();
    }

    public class Record
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
