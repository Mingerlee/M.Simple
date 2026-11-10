# M.Simple.Extensions.Configuration：配置读取

配置文件加载、基础路径处理，以及保留的 AppSetting 便捷类。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/ConfigurationExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Extensions.Configuration --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- configuration
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| 路径 | SetConfigurationBasePath | 在构建器 Properties 中保存 BasePath |
| JSON | AddJsonFileWithBasePath | 与基础路径组合后调用配置提供程序 |
| 便捷读取 | AppSetting.GetConfig、GetConfigInt32、GetConfigBoolean、GetConfigSection<T> | 依赖当前工作目录的 appsettings.json |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
using M.Simple.Extensions.Configuration;
using Microsoft.Extensions.Configuration;

namespace M.Simple.Samples;

public static class ConfigurationExamples
{
    public static void Run()
    {
        var configuration = new ConfigurationBuilder()
            .SetConfigurationBasePath(AppContext.BaseDirectory)
            .AddJsonFileWithBasePath("appsettings.json")
            .Build();
        using var lifetime = (IDisposable)configuration;
        var title = configuration["Demo:Title"];
        Verify.That(title == "M.Simple 使用示例", "配置加载失败");
        Console.WriteLine($"配置：{title}");
    }
}
```

## 使用边界

可运行示例使用 AppContext.BaseDirectory，并由项目文件将 appsettings.json 复制到输出目录，因此从仓库根目录或项目目录运行都能读取。

AppSetting 仍按当前工作目录创建静态配置实例，且要求存在 appsettings.json；多配置源或宿主依赖注入场景优先直接使用 IConfiguration。ConfigExist 的当前实现不能可靠判断键是否存在；需要判断时检查 configuration["Key"] 或节的实际内容。
