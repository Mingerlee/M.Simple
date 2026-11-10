# M.Simple.Extensions.Core：基础扩展

无第三方依赖，提供字符串、日期、金额、集合、枚举、反射和实体比较。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/CoreExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Extensions.Core --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- core
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| 字符串 | HasValue、ToFormatMatch、ToCover、ToDBC、ToSBC | 空白检测、匹配文本归一化、掩码、全半角 |
| 日期 | BeginOfMonth、EndOfMonth、BeginOfDay、EndOfDay | 边界方法返回 Unspecified；结束精度为毫秒 |
| 月份 | ParseMonthToDateTime、DiffMonths、DiffMonth | 严格 yyyy-MM 解析；月份差是接收者减去参数 |
| 金额 | ToRound、ToChinese、ToFormatMoney | ToRound 使用 AwayFromZero；中文金额仍属于 Core |
| 集合 | GroupBySelectFirst、GroupByThenSortTakeFirst、SplitBatch | 分组首条、分组排序首条、分批 |
| 字典 | ToDictionarySafe、ToDictionarys、Merge、MergeIgnore | ToDictionarySafe 重复键保留最后值；ToDictionarys 保留第一值 |
| 枚举 | GetDescription、GetDisplayName、ToEnum、ToValueDescriptions | 特性读取、转换、枚举字典 |
| 反射 | GetPublicPropertyNames、ToDictionary、GetObjectPropertyValue | 属性集合、对象转字典、忽略大小写取值 |
| 实体比较 | CompareModel、IsModify、ModifyField | 有 CompareValue/ModifyValue 特性的模式可选 |
| 比较器 | PropertyEqualityComparer、FastAllPropertiesEqualityComparer | 按指定属性或全部公共属性比较 |
| 其他 | GetNextGuid、GetNoHyphenString、SnowCodeGenerator | 旧版 GUID/编码辅助 |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
using M.Simple.Extensions.Core;

namespace M.Simple.Samples;

public static class CoreExamples
{
    public static void Run()
    {
        // 匹配格式化与月份边界。
        var normalized = " （“样例”） ".ToFormatMatch();
        var month = "2024-02".ParseMonthToDateTime();
        Verify.That(normalized == "(\"样例\")" && month.EndOfMonth().Day == 29, "格式或月份结果错误");
        Console.WriteLine($"匹配文本：{normalized}；月末：{month.EndOfMonth():yyyy-MM-dd HH:mm:ss.fff}");

        // 分组默认保留第一条，安全字典默认保留最后一条。
        var products = new[]
        {
            new Product { Id = 1, Name = "原名称", Price = 12.345m },
            new Product { Id = 1, Name = "新名称", Price = 20m },
            new Product { Id = 2, Name = "另一产品", Price = 30m }
        };
        var first = products.GroupBySelectFirst(row => row.Id).ToArray();
        var last = products.ToDictionarySafe(row => row.Id);
        var batches = products.SplitBatch(2).Select(batch => batch.Count()).ToArray();
        Verify.That(first[0].Name == "原名称" && last[1].Name == "新名称", "重复键处理错误");
        Console.WriteLine($"分批：{string.Join(',', batches)}；金额：{12.345m.ToRound(2)}");

        // 接收者是新值，第二个参数是旧值；只比较标记 CompareValue 的属性。
        var changes = products[1].CompareModel(products[0], isAttr: true);
        var fields = typeof(Product).GetPublicPropertyNames();
        var dictionary = products[0].ToDictionary();
        Verify.That(changes.Count == 2 && fields.Contains("Name") && dictionary.ContainsKey("name"), "比较或反射结果错误");
        Console.WriteLine($"变化字段：{string.Join(',', changes.Select(change => change.FiledName))}");
        Console.WriteLine($"枚举：{OrderStatus.Pending.GetDisplayName()}；属性：{products[0].GetObjectPropertyValue("NAME")}");
    }
}
```

## 使用边界

### 返回值与边界

- `GroupBySelectFirst` 取输入顺序的第一条；需要“最新一条”时使用 `GroupByThenSortTakeFirst` 并提供排序字段。
- `ToDictionarySafe` 重复键保留最后一条；不要与 `ToDictionarys` 的第一条策略混用。
- `BeginOfMonth` 与 `GetFirstDayOfMonth` 等价；`EndOfMonth` 返回 23:59:59.999，`GetLastDayOfMonth` 返回零点。
- `ParseMonthToDateTime` 对非法格式抛出 FormatException；`EndOfDay(DateTime?)` 保留 null。
- `ToDictionary` 默认将键转为不变区域的小写；传 false 保留原名；跳过索引器与非公共 getter。
- `GetPublicPropertyNames` 保留属性原名；`GetObjectPropertyValue` 忽略大小写，缺失属性抛出 KeyNotFoundException。
- `CompareModel` 接收者为新值，参数为旧值；`isAttr: true` 只比较带 CompareValue 的属性。它比较标量属性的字符串表示，不是深度对象比较；默认不忽略审计字段。
- `IsOverlap` 当前会原地排序且不支持空集合；DateTimePeriod 相接不算重叠，DatePeriod 相接算重叠。
- `CollectionExtensions.TakeLast` 与 LINQ 同名，必要时使用完整类型名静态调用。
- 无特性的枚举显示方法当前可能返回空字符串；超出 Int32 的枚举值不要使用 ToInt32。
- 旧版 GetListHashCode 只是哈希值相加，不作为集合相等或持久化唯一性的依据。SnowCodeGenerator 的机器标识需在跨进程使用前核对。

实体比较、可空属性赋值等已知问题见[总览](../usage.md#已知限制)。
