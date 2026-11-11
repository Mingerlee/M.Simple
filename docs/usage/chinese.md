# M.Simple.Extensions.Chinese：中文工具

本页为统一包 `M.Simple.Extensions` 的功能分类，命名空间保留，安装时仅需引用统一扩展包。

依赖 PinYinConverterCore；提供拼音、首字母、中文日期和农历转换。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/ChineseExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Extensions --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- chinese
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| 拼音 | ToPinYin、ToFirstPinyin | 词典结果不提供上下文多音字消歧 |
| 日期 | ToChineseDateString、ToChineseDateTimeString | 仅格式化，不进行时区转换 |
| 农历 | ToLunarCalendarDate | 固定历法表，使用前核对支持年份 |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
using M.Simple.Extensions.Chinese;

namespace M.Simple.Samples;

public static class ChineseExamples
{
    public static void Run()
    {
        var date = new DateTime(2024, 2, 10, 9, 30, 0);
        var pinyin = "中文".ToPinYin();
        Verify.That(!string.IsNullOrWhiteSpace(pinyin), "拼音转换结果为空");
        Console.WriteLine($"拼音：{pinyin}；首字母：{"中文".ToFirstPinyin()}");
        Console.WriteLine($"中文日期：{date.ToChineseDateTimeString(removeSecond: true)}");
        Console.WriteLine($"农历：{date.ToLunarCalendarDate()}");
    }
}
```

## 使用边界

中文金额 `ToChinese(decimal)` 在 Core 包。拼音方法采用词典中的首个读音；业务名称涉及多音字时请核对。农历采用内置年份范围；不将固定日期样例外推为所有年份可用。可空农历重载未传默认值时，null 会使用当前日期。
