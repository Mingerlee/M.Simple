# M.Simple.Extensions

.NET 8 通用扩展，零第三方运行时依赖。版本：1.0.0-preview.1。

## 功能

- M.Simple.Extensions.Enums：GetDescription、EnumExtensions.GetOptions。
- M.Simple.Extensions.Strings：Mask、MaskPhone、MaskEmail。
- M.Simple.Extensions.Collections：DiffBy、ToTree。
- M.Simple.Extensions.Comparison：ObjectComparison.Compare、ComparisonOptions。

## 示例

```csharp
using M.Simple.Extensions.Strings;
var phone = "13812345678".MaskPhone(); // 138****5678
```

Mask 长度不足时全部遮盖；ToTree 遇到重复键、孤儿和环路会抛异常；DiffBy 的内容相等判断可通过 IEqualityComparer 配置；对象比较是浅比较，默认 object.Equals，不递归处理引用对象。反射型 API 在裁剪、NativeAOT 环境下尚未验证。

本版重新设计 API，不保证兼容旧包。完整中文手册随仓库 website 目录提供，可在本地站点 http://127.0.0.1:8090/ 阅读。公共 NuGet 源尚未发布。
