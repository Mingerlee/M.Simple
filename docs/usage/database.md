# M.Simple.Extensions.EntityFrameworkCore：EF Core 与 Dapper

依赖 EF Core Relational 8.0.18、Dapper 2.1.35；不绑定业务实体或数据库驱动。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/DatabaseExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Extensions.EntityFrameworkCore --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- database
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| 列表 | QueryAsync<T>、QueryAsync | 分别返回强类型列表和动态列表 |
| 首条 | QueryFirstOrDefaultAsync<T>、QueryFirstOrDefaultAsync | 强类型默认值或 ExpandoObject/null |
| 参数 | sql、param、commandTimeout、cancellationToken | 可用命名参数明确超时与取消 |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
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
```

## 使用边界

- 调用入口是 context.Database，不是 DbConnection。数据库驱动由宿主安装，例如 SQLite 或 MySQL 提供程序。
- SQL 值通过 param 对象传入，示例用 @Id；SQL 标识符和排序字段不能作为普通值参数传入，应由调用方约束。
- 自动透传 EF Core 当前事务，不自动开始、提交或回滚；示例显式回滚并验证计数为 0。
- 超时优先使用方法参数，否则使用 EF 当前命令超时；取消令牌传给 Dapper CommandDefinition。
- 首条强类型查询无记录时返回 default(T)，例如 long 为 0；需要区分缺失时使用可空值类型。
- 动态首条是 ExpandoObject，保留驱动返回的列值类型，不能假定 SQLite 的整数类型与 CLR int 相同。
- Dapper 临时开关原本关闭的连接，不释放宿主 DbContext；SQLite 内存库需主动保持连接打开。
- 同一 DbContext 不用于并发查询；本包不会补数据权限或业务筛选。MySQL/Doris 尚未实际接入验证。
