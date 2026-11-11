# M.Simple.Extensions

统一扩展方法包，目标框架 .NET 8。包含 Core、Chinese、DataTable、Cryptography、Configuration、Expressions、EntityFrameworkCore、Json 八个功能目录。

安装：dotnet add package M.Simple.Extensions --version 1.0.0

保留原有功能命名空间，例如 using M.Simple.Extensions.Core;、using M.Simple.Extensions.Chinese;。包名与程序集名为 M.Simple.Extensions，功能命名空间不等于独立 NuGet 包。

依赖：PinYinConverterCore、Microsoft.Extensions.Configuration 系列、EF Core Relational 和 Dapper；不依赖任何 M.Simple.Services 包，数据库驱动由宿主选择。

从原八个扩展包迁移：删除旧包引用，安装此包并重新编译；现有 using 和方法调用可继续使用。不提供旧程序集的二进制兼容层。

[完整文档](../../../docs/usage.md) · [示例项目](../../../samples/M.Simple.Samples/README.md)
