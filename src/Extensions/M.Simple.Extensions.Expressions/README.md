# M.Simple.Extensions.Expressions

无第三方依赖的条件表达式扩展，面向 .NET 8。

```csharp
using System.Linq.Expressions;
using M.Simple.Extensions.Expressions;

Expression<Func<Product, bool>> active = row => row.Active;
Expression<Func<Product, long>> uid = row => row.Uid;
var filter = active.And(uid.In(new long[] { 1, 2 }));
```

提供 True、False、And、Or、In。And/Or 使用参数替换与短路求值，不产生 Invoke 节点。任一条件为 null 时返回另一条件，两者都为空则返回 null。In 使用强类型值集合，空集合不匹配记录。

参考 BAMS 的表达式组合工具重新实现；不依赖 BAMS 模型。已用 SQLite 验证基本 EF Core 查询翻译；其他查询提供程序需自行验证。

## 使用文档与示例

[完整使用说明](../../../docs/usage/expressions.md) · [可运行示例](../../../samples/M.Simple.Samples/ExpressionExamples.cs)

在仓库根目录运行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- expressions
```
