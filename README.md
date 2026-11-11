# M.Simple

面向多个 .NET 项目和 NuGet 用户的通用扩展库。第一版为 1.0.0-preview.1，目标框架为 .NET 8，零第三方运行时依赖。

## 第一版功能

| 模块 | 能力 | 命名空间 |
|---|---|---|
| 枚举 | 描述、保留原始枚举类型的选项列表 | M.Simple.Extensions.Enums |
| 脱敏 | 自定义首尾保留、手机号、邮箱 | M.Simple.Extensions.Strings |
| 集合 | 按键比较新增/修改/删除、平铺数据转树 | M.Simple.Extensions.Collections |
| 对象比较 | 属性前后值、中文显示名、忽略字段、自定义相等判断 | M.Simple.Extensions.Comparison |

只发布一个包 M.Simple.Extensions。测试框架依赖仅存在于测试项目，不随 NuGet 包传递。

## 使用手册

- [快速开始](docs/usage.md)
- [枚举](docs/usage/enums.md)
- [字符串脱敏](docs/usage/strings.md)
- [集合](docs/usage/collections.md)
- [对象比较](docs/usage/comparison.md)
- [运行示例](samples/M.Simple.Samples/README.md)
- [文档站点发布](website/README.md)

本地手册地址：http://127.0.0.1:8090/。

## 构建与验证

```powershell
dotnet build M.Simple.sln --configuration Release
dotnet test M.Simple.sln --configuration Release
dotnet run --project samples/M.Simple.Samples --configuration Release -- all
dotnet pack src/Extensions/M.Simple.Extensions --configuration Release --output artifacts/packages
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/Build-DocsSite.ps1
```

## 版本与兼容性

这是重新设计的第一版，旧源码、旧服务包及其依赖已移出当前仓库。保留解决方案、基础扩展项目、测试项目、示例项目及静态手册框架。旧命名空间和调用签名不承诺兼容，请按本版手册调整。

预览包用于接入验证，尚未发布到公共 NuGet 源。发布稳定版前确认包名归属、许可证、作者信息和版本；公开 NuGet 发布由维护者执行。
