# M.Simple

从 `M.Simple.Expand 1.0.1.8` 拆分的 .NET 8 功能包，并补充来自 BAMS 的通用扩展能力。项目名、程序集名和 NuGet 包名一致，新包起始版本为 `1.0.0`。

## 包与依赖

| 包 | 内容 | 依赖 |
|---|---|---|
| M.Simple.Extensions.Core | 字符串、集合、字典、枚举、Guid、日期、金额、实体比较、比较器、验证特性及原有雪花编码工具 | .NET |
| M.Simple.Extensions.Chinese | 拼音、首字母、中文日期格式化、农历 | PinYinConverterCore |
| M.Simple.Extensions.DataTable | DataTable 与集合互转、空值判断、筛选 | .NET |
| M.Simple.Extensions.Cryptography | AES、RSA、MD5、签名、验签及返回模型 | Core |
| M.Simple.Extensions.Configuration | 配置加载、路径处理、AppSetting | Microsoft.Extensions.Configuration 系列 |
| M.Simple.Extensions.Expressions | 强类型 In、条件组合、参数替换 | .NET |
| M.Simple.Extensions.EntityFrameworkCore | EF Core 与 Dapper 查询桥接、事务与取消透传 | EF Core Relational、Dapper |
| M.Simple.Extensions.Json | long/long? 字符串转换与序列化选项注册 | .NET |
| M.Simple.Services.DependencyInjection | AppService、Autowired 特性、服务扫描和属性装配 | DI Abstractions |
| M.Simple.Services.Email | 邮件模型、发送服务、AddEMailSetup 注册扩展 | Core、DI Abstractions |
| M.Simple.Services.Ftp | FTP 模型、传输服务、原 FtpClient 辅助类、AddFtpClientSetup 注册扩展 | DI Abstractions |

服务注册扩展随服务包提供，不放入通用依赖注入包。Core 不依赖中文、配置、DI 或其他服务包。DynamicQuery、Threading 暂未创建。

## 使用

[静态文档网站与 IIS 挂载说明](website/README.md)：生成的网站文件位于 `website`，可直接由 IIS 托管。

[完整使用文档](docs/usage.md)覆盖全部 11 个包的安装、API、示例和边界；[可运行示例项目](samples/M.Simple.Samples/README.md)可直接运行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- all
```

按需引用包，并导入其同名命名空间：

```csharp
using M.Simple.Extensions.Core;
using M.Simple.Extensions.Chinese;
using M.Simple.Services.Email;

var amount = 123.45m.ToChinese();
var pinyin = "中文".ToPinYin();
```

模型、枚举、特性分别位于对应包的 `.Models`、`.Enums`、`.Attributes` 命名空间；邮件、FTP 的模型和接口直接位于其服务包命名空间。

## 从旧包迁移

- 本仓库不包含 BAMS 业务逻辑；新增通用扩展不修改 BAMS 源码和包引用。
- 将 `using M.Simple.Expand` 改为实际使用的功能包命名空间；旧 `.Models`、`.Enums`、`.Attributes` 按所属功能调整。
- 旧 `partial Extensions` 按功能拆成 `StringExtensions`、`DateTimeExtensions`、`EntityComparisonExtensions` 等类型，避免跨程序集同名类型冲突。普通扩展调用保留方法名；显式 `Extensions.Xxx()` 调用需要修改类型名。
- 这是新的程序集与命名空间，不提供旧库的二进制兼容层。调用方需要重新编译。
- `CollectionExtensions.TakeLast` 保留原实现，与 LINQ 同名时请显式调用 `M.Simple.Extensions.Core.CollectionExtensions.TakeLast(source, count)`。
- 迁移仅调整归属、命名空间、扩展类型名和必要引用。原有方法实现保留，已知缺陷未在拆包过程中修复。

源码归属及原文件 SHA-256 见 `migration-map.json`。中文方法与服务注册方法从原混合文件中拆出。原 42 个源码文件均已纳入迁移，279 个方法实现经语法令牌对比保持一致。

## BAMS 通用方法补充

Core 新增 BeginOfMonth、EndOfMonth、BeginOfDay、EndOfDay（含可空重载）、ParseMonthToDateTime、DiffMonths、HasValue、ToFormatMatch、GetPublicPropertyNames、对象 ToDictionary、GetObjectPropertyValue。来源与行为差异见 [BAMS 方法对照](docs/bams-extensions.md)。

Expressions、EntityFrameworkCore、Json 各自独立，避免 Core 引入数据库和序列化配置依赖。已有实体比较、集合、金额、拼音继续复用，不另建同义实现。

## 构建与打包

```powershell
dotnet build M.Simple.sln --configuration Release
dotnet test M.Simple.sln --configuration Release
dotnet pack M.Simple.sln --configuration Release --output artifacts/packages
```

原有 30 项测试已迁入，另有依赖边界、配置、服务注册及 BAMS 通用扩展测试。数据库测试使用 SQLite 内存数据库；邮件和 FTP 测试只构造服务，不连接外部系统。旧源码的可空引用、过时 API 和 XML 文档警告保留可见。

初次拆包时 34 项测试通过，原 8 个包通过独立项目引用验证。补充 BAMS 通用能力后，47 项测试通过，11 个包打包成功，详见 [验证结果](docs/bams-extensions.md#验证结果)。打包产物位于 `artifacts/packages`；尚未发布到包源。

## 已知问题

沿用原实现的实体比较可能漏掉声明类型为 object 的属性；SetStringValue 不支持可空类型；GetStringValue 读取 float/double 可能异常；IsOverlap 不支持空集合；DataTable.Where 对 null 的处理有缺陷。这些问题需在单独修复中补充针对性测试。
