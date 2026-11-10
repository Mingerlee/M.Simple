# M.Simple.Extensions.Json

基于 System.Text.Json 的 long/long? 转换器，无第三方依赖。

```csharp
using System.Text.Json;
using M.Simple.Extensions.Json;

var options = new JsonSerializerOptions().AddLongAsStringConverters();
var json = JsonSerializer.Serialize(long.MaxValue, options);
```

long 写为字符串，读取 JSON 整数或整数字符串；long? 另支持 null 和空字符串。非法、溢出和非整数输入抛出 JsonException，不静默改为 0。注册扩展可重复调用，已只读的选项不允许注册；已有其他转换器仍遵循 System.Text.Json 的优先级规则。

参考 BAMS 查询层的严格转换器提取，不采用 API 层非法输入返回 0 的宽松策略。

## 使用文档与示例

[完整使用说明](../../../docs/usage/json.md) · [可运行示例](../../../samples/M.Simple.Samples/JsonExamples.cs)

在仓库根目录运行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- json
```
