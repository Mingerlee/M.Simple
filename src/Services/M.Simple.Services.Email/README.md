# M.Simple.Services.Email

邮件发送、附件、模型与服务注册。

目标框架：.NET 8。由 M.Simple.Expand 1.0.1.8 按功能拆分，沿用原实现。

安装后使用同名命名空间：

```csharp
using M.Simple.Services.Email;
```

迁移时需要更新旧命名空间及显式静态类调用；不提供旧程序集的二进制兼容层。

## 使用文档与示例

[完整使用说明](../../../docs/usage/email.md) · [可运行示例](../../../samples/M.Simple.Samples/EmailExamples.cs)

在仓库根目录运行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- email
```
