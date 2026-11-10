# M.Simple.Extensions.EntityFrameworkCore

EF Core 8 与 Dapper 的查询桥接，不依赖 BAMS、Core 或业务模型。

```csharp
using M.Simple.Extensions.EntityFrameworkCore;

var rows = await context.Database.QueryAsync<Product>(
    "SELECT * FROM products WHERE uid = @Uid", new { Uid = uid },
    cancellationToken: cancellationToken);
```

提供 QueryAsync 的强类型和动态版本、QueryFirstOrDefaultAsync 的强类型和 ExpandoObject 版本。参数对象传给 Dapper；自动透传 EF Core 当前事务与命令超时，支持单次覆盖超时及取消令牌。

宿主管理 DbContext、连接和事务。方法不提交事务、不释放宿主资源；Dapper 对原本关闭的连接按自身规则临时开关。无记录时首条查询返回默认值。动态首条结果保留驱动的列值类型，不经 JSON 中转。

参考 BAMS 的 DatabaseExtends 提取；已在 SQLite 内存数据库验证参数绑定、事务回滚、连接状态和取消。其他数据库驱动需在接入时验证，不提供 BAMS 权限、SQL 拼接或业务分页功能。

## 使用文档与示例

[完整使用说明](../../../docs/usage/database.md) · [可运行示例](../../../samples/M.Simple.Samples/DatabaseExamples.cs)

在仓库根目录运行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- database
```
