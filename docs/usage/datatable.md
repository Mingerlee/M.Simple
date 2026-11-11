# M.Simple.Extensions.DataTable：DataTable 转换

本页为统一包 `M.Simple.Extensions` 的功能分类，命名空间保留，安装时仅需引用统一扩展包。

无第三方依赖；提供 IList 与 DataTable 互转和行筛选。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/DataTableExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Extensions --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- datatable
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| 集合转表 | ToDataTable(this IList) | 依靠第一条对象发现列 |
| 表转集合 | ToLists<T>() where T : class, new() | 公共可写属性与列名匹配，直接赋值 |
| 筛选 | Where(Func<DataRow,bool>) | 返回新表，不修改原表 |
| 空判断 | IsNullOrEmpty | 无列或无行均视为空 |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
using System.Collections;
using M.Simple.Extensions.DataTable;

namespace M.Simple.Samples;

public static class DataTableExamples
{
    public static void Run()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "产品 A", Price = 12m },
            new() { Id = 2, Name = "产品 B", Price = 30m }
        };
        using var table = ((IList)products).ToDataTable();
        using var filtered = table.Where(row => (decimal)row["Price"] >= 20m);
        var rows = filtered.ToLists<Product>();
        Verify.That(rows.Count == 1 && rows[0].Id == 2, "DataTable 转换结果错误");
        Console.WriteLine($"源表 {table.Rows.Count} 行；筛选后 {rows.Count} 行：{rows[0].Name}");
    }
}
```

## 使用边界

- 示例显式转换为 `IList`，以选择 DataTable 扩展。
- DataTable 列名、类型需要与目标属性匹配；当前不会将字符串自动解析为数字或枚举。DBNull 列不会赋值，目标属性保留默认值。
- ToDataTable 识别 DisplayName 特性并用显示名作列名；ToLists 按属性名匹配。因此带 DisplayName 的表不能假定可直接往返。
- 空 IList 生成无列的空表；表结构稳定的业务可自行先建列。
- Where 的接收者不能为 null，当前旧实现的空值分支存在缺陷。
