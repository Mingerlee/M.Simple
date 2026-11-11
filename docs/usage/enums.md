# 枚举描述与选项

命名空间：M.Simple.Extensions.Enums。无需服务注册。

## 示例

```csharp
using System.ComponentModel;
using M.Simple.Extensions.Enums;

Console.WriteLine(AuditStatus.Approved.GetDescription()); // 已通过
var options = EnumExtensions.GetOptions<AuditStatus>();
foreach (var option in options)
    Console.WriteLine($"{(int)option.Value}: {option.Text}");

enum AuditStatus
{
    [Description("待审核")] Pending = 0,
    [Description("已通过")] Approved = 1
}
```

## API 与规则

| API | 返回 | 行为 |
|---|---|---|
| value.GetDescription() | string | 优先 DescriptionAttribute，无描述返回名称 |
| EnumExtensions.GetOptions<TEnum>() | IReadOnlyList<EnumOption<TEnum>> | Value 为 TEnum，Text 为描述 |

- 支持所有枚举底层整数类型，不强制转换为 int，避免 long 等大值溢出。
- 未定义值返回数字文本；未命名 Flags 组合使用枚举 ToString，不逐项拼接描述。
- 选项使用 Enum.GetValues 的值顺序，同值别名合并。多个别名对应哪个名称由运行时决定，不依赖其顺序。
- 返回列表只读；不添加“全部”“请选择”等业务选项。
- 本版使用 DescriptionAttribute，不读取 DisplayAttribute 的资源本地化。
