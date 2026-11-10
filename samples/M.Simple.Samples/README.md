# M.Simple 可运行示例

目标框架 .NET 8。项目通过 ProjectReference 引用当前仓库的 11 个功能包，此外使用 Microsoft.Extensions.DependencyInjection 9.0.2 和 Microsoft.EntityFrameworkCore.Sqlite 8.0.18。

从仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- all
dotnet run --project samples/M.Simple.Samples --configuration Release -- core
dotnet run --project samples/M.Simple.Samples --configuration Release -- database
```

参数：all、core、chinese、datatable、cryptography、configuration、expressions、database、json、di、email、ftp。未知参数返回退出码 2。

[完整使用文档](../../docs/usage.md)介绍安装、命名空间、API 和边界。Models.cs 是示例模型，Verify 是示例自检助手，都不是公共包 API。appsettings.json 自动复制到输出目录。

默认 all 不连接外部服务；数据库仅在内存中创建，退出即释放。EmailExamples.SendAsync、FtpExamples.UploadAsync 只是可编译的接入模板，默认入口不调用。

[完整运行输出](../../docs/sample-output.md)可用于核对各场景结果。验证时全部 11 个场景运行成功，现有 47 项测试通过。
