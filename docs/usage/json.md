# M.Simple.Extensions.Json：JSON 长整数

本页为统一包 `M.Simple.Extensions` 的功能分类，命名空间保留，安装时仅需引用统一扩展包。

基于 System.Text.Json，无第三方依赖。适合将长整数标识序列化为字符串。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/JsonExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Extensions --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- json
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| 注册 | AddLongAsStringConverters | 幂等添加 long 与 long? 转换器 |
| 非空 | LongToStringConverter | 写字符串；读整数或整数字符串 |
| 可空 | NullableLongToStringConverter | 另接受 null 与空字符串 |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
using System.Text.Json;
using M.Simple.Extensions.Json;

namespace M.Simple.Samples;

public static class JsonExamples
{
    public static void Run()
    {
        var options = new JsonSerializerOptions().AddLongAsStringConverters();
        var json = JsonSerializer.Serialize(new { Uid = long.MaxValue, ParentUid = (long?)null }, options);
        Verify.That(json.Contains("\"9223372036854775807\""), "长整数未输出为字符串");
        Verify.That(JsonSerializer.Deserialize<long>("\"9223372036854775807\"", options) == long.MaxValue, "长整数读取失败");
        Console.WriteLine(json);
    }
}
```

## 使用边界

非法、溢出、布尔或小数输入抛出 JsonException，不静默写成 0。long? 只把空字符串视为 null，空白字符串仍按整数解析规则处理。选项在首次使用后可能变成只读，需提前注册；同类转换器不重复添加，但已有其他转换器仍遵循 System.Text.Json 的选择顺序。

ASP.NET Core MVC 可在 AddControllers().AddJsonOptions 回调中，对 options.JsonSerializerOptions 调用 AddLongAsStringConverters。Minimal API 使用其自己的 HTTP JSON 选项配置，不能假定 MVC 选项自动覆盖。
