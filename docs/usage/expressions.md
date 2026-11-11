# M.Simple.Extensions.Expressions：表达式组合

本页为统一包 `M.Simple.Extensions` 的功能分类，命名空间保留，安装时仅需引用统一扩展包。

无第三方依赖；组合后的表达式可以直接传入 IQueryable.Where。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/ExpressionExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Extensions --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- expressions
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| 默认条件 | PredicateExtensions.True<T>、False<T> | 方便逐步构造条件 |
| 逻辑组合 | And、Or | 替换参数，不生成 Invoke 节点；短路求值 |
| 集合匹配 | In<T,TValue> | 字段和值集合类型一致，空集合不匹配 |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
using System.Linq.Expressions;
using M.Simple.Extensions.Expressions;

namespace M.Simple.Samples;

public static class ExpressionExamples
{
    public static void Run()
    {
        Expression<Func<Product, bool>> active = row => row.Active;
        Expression<Func<Product, int>> id = row => row.Id;
        var filter = active.And(id.In(new[] { 1, 3 }))!;
        var rows = new[]
        {
            new Product { Id = 1, Active = true },
            new Product { Id = 2, Active = true },
            new Product { Id = 3, Active = false }
        }.AsQueryable().Where(filter).ToArray();
        Verify.That(rows.Length == 1 && rows[0].Id == 1, "表达式组合结果错误");
        Console.WriteLine($"符合条件的产品：{rows[0].Id}");
    }
}
```

## 使用边界

And/Or 任一条件为 null 时返回另一条件；二者均为 null 则返回 null。在最终 Where 前检查条件或使用明确的默认条件。

示例的 `!` 来自两个非空条件；它不会替代运行时校验。LINQ to Objects 可 Compile；EF Core 场景应传原表达式，避免提前 Compile 后失去数据库翻译。SQLite 已验证基本翻译，其他提供程序需验证。
