# M.Simple 使用手册

第一版 1.0.0-preview.1，目标框架 .NET 8。一个 NuGet 包，四类功能，不需要依赖注入、配置文件或外部服务。

## 安装本地构建包

在仓库根目录运行：

```powershell
dotnet pack src/Extensions/M.Simple.Extensions --configuration Release --output artifacts/packages
```

在你的调用项目目录运行：

```powershell
dotnet add package M.Simple.Extensions --version 1.0.0-preview.1 --source D:/oym/M.Simple/artifacts/packages
```

也可以从手册首页下载 nupkg 并放入本地包源。当前未发布到公共 NuGet 源，不能依赖默认 nuget.org 安装此版本。

## 选择功能

| 功能 | 手册 | 示例参数 |
|---|---|---|
| 枚举描述与选项 | [枚举](usage/enums.md) | enums |
| 手机号、邮箱与通用脱敏 | [字符串脱敏](usage/strings.md) | strings |
| 集合差异、平铺转树 | [集合](usage/collections.md) | collections |
| 字段前后值与自定义比较 | [对象比较](usage/comparison.md) | comparison |

## 运行示例

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- all
```

不传参数也运行全部场景。示例不访问数据库、缓存、邮件或文件服务。[运行说明](../samples/M.Simple.Samples/README.md)提供源码入口，[示例输出](sample-output.md)由实际运行生成。

## 接入边界

- 枚举值保留其枚举类型；前端需要数值时在调用层显式转换或配置序列化，不强制 int。
- 对象比较和枚举描述使用反射；裁剪、NativeAOT 尚未验证。
- 字符串保留长度按 UTF-16 计数，不按 Unicode 字素簇处理。
- 集合和对象 API 都不修改原始数据，不写数据库，不实现自动保存。
- 本版不提供旧库的 API 兼容层。
