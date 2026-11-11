# 字符串脱敏

命名空间：M.Simple.Extensions.Strings。

## 示例

```csharp
using M.Simple.Extensions.Strings;

"13812345678".MaskPhone();       // 138****5678
"alice@example.com".MaskEmail(); // a****@example.com
"1234567890".Mask(2, 2);         // 12******90
"abcdef".Mask(2, 2, '#');        // ab##ef
"123".MaskPhone();              // ***
```

## API 与规则

| API | 参数与返回 |
|---|---|
| Mask(visiblePrefix = 0, visibleSuffix = 0, mask = '*') | 保留首尾，返回可空字符串 |
| MaskPhone() | 保留前 3 后 4 位，不验证地区或手机号格式 |
| MaskEmail() | 保留用户名首字符和完整域名，单字符用户名遮盖 |

- null 原样返回，空串仍为空串。
- 保留长度为负数抛 ArgumentOutOfRangeException，即使输入为 null 也校验参数。
- 首尾保留长度之和大于等于输入长度时全部遮盖，避免短值泄露。
- 邮箱必须恰好有一个 @ 且两侧非空，否则全部遮盖；这不是完整的邮箱合法性验证。
- 遮盖后长度保持不变；计数按 UTF-16 code unit，不保证 emoji 或组合字符的显示完整性。
- MaskEmail 会保留域名，适用于允许展示域名的场景；可用 Mask 自定义更严格规则。
