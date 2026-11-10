# M.Simple 使用文档

适用于当前仓库的 .NET 8 / 1.0.0 功能包。所有 11 个包均有可编译运行的示例，扩展包不需要统一服务注册，服务包通过各自的 Add 方法注册。

## 快速运行

在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- all
```

默认不传参数也运行 all。单包命令及源文件见下表。数据库用 SQLite 内存库；邮件和 FTP 仅注册与解析服务。示例不写业务数据库、不发送邮件、不建立 FTP 连接。

[查看一次完整运行的输出](sample-output.md)。

| 包 | 使用文档 | 示例参数 |
|---|---|---|
| M.Simple.Extensions.Core | [基础扩展](usage/core.md) | core |
| M.Simple.Extensions.Chinese | [中文工具](usage/chinese.md) | chinese |
| M.Simple.Extensions.DataTable | [DataTable 转换](usage/datatable.md) | datatable |
| M.Simple.Extensions.Cryptography | [加解密与摘要](usage/cryptography.md) | cryptography |
| M.Simple.Extensions.Configuration | [配置读取](usage/configuration.md) | configuration |
| M.Simple.Extensions.Expressions | [表达式组合](usage/expressions.md) | expressions |
| M.Simple.Extensions.EntityFrameworkCore | [EF Core 与 Dapper](usage/database.md) | database |
| M.Simple.Extensions.Json | [JSON 长整数](usage/json.md) | json |
| M.Simple.Services.DependencyInjection | [服务扫描与属性注入](usage/di.md) | di |
| M.Simple.Services.Email | [邮件服务](usage/email.md) | email |
| M.Simple.Services.Ftp | [FTP 服务](usage/ftp.md) | ftp |

示例由 [Program.cs](../samples/M.Simple.Samples/Program.cs) 选择场景，共享 [Models.cs](../samples/M.Simple.Samples/Models.cs) 提供 Product、枚举和自检助手。每个场景校验关键结果，失败时进程抛异常并返回非零退出码；数值、日期和拼音显示可能受区域或词典影响，不以完整控制台文本作为固定协议。

## 安装本地包

包目前未发布到公共源。先在仓库根目录生成：

```powershell
dotnet pack M.Simple.sln --configuration Release --output artifacts/packages
```

在消费项目的目录创建 NuGet.Config，替换本地源路径为你的实际目录：

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="M.Simple.Local" value="D:\oym\M.Simple\artifacts\packages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

然后按需安装，例如：

```powershell
dotnet add package M.Simple.Extensions.Core --version 1.0.0
dotnet add package M.Simple.Extensions.Json --version 1.0.0
```

示例项目使用 ProjectReference，始终运行当前工作区代码；消费项目使用 PackageReference，运行的是已打包版本。开发中反复生成相同版本时，NuGet 缓存可能继续使用旧包；需要验证新产物时使用新的包版本或隔离的还原目录，不把样例成功等同于已发布。

## 命名空间与注册

- 包名、程序集名与根命名空间一致，例如 using M.Simple.Extensions.Core。
- Core 的特性、枚举、模型位于 .Attributes、.Enums、.Models；加密结果在 Cryptography.Models。
- Email、Ftp 的服务、接口和配置位于各自根命名空间。
- 控制台程序如需 BuildServiceProvider，额外引用 Microsoft.Extensions.DependencyInjection；服务包只依赖 Abstractions。
- EF Core 包不选择数据库驱动，由宿主选择提供程序并配置 DbContext。
- [旧库迁移说明](../README.md#从旧包迁移)和 [BAMS 行为差异](bams-extensions.md)说明需要核对的兼容边界。

## 已知限制

旧库迁入的方法没有在文档任务中修改行为。CompareModel 对声明类型为 object 的模型可能漏比较；SetStringValue 不支持可空类型；GetStringValue 对 float/double 存在类型转换错误；IsOverlap 不支持空集合；DataTable.Where 不支持 null 接收者。邮件抄送等分支和 FTP 跨平台路径也有现有限制，详见对应包页。

文档示例选择已验证的调用方式，不表示这些已知问题已修复。邮件/FTP 实际网络调用、MySQL/Doris 与 BAMS API 接入未验证。
