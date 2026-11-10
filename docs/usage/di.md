# M.Simple.Services.DependencyInjection：服务扫描与属性注入

依赖 DI Abstractions。控制台示例另外安装 Microsoft.Extensions.DependencyInjection，以创建服务容器。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/DependencyInjectionExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Services.DependencyInjection --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- di
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| 服务标记 | AppServiceAttribute | 指定 ServiceType、Lifetime 或按首个接口注册 |
| 扫描 | AddAppServices | 扫描当前已加载程序集 |
| 属性标记 | AutowiredAttribute | 标记字段或属性 |
| 装配 | AutowiredService.Autowired | 必须显式调用，提供正确的 IServiceProvider |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
using M.Simple.Services.DependencyInjection;
using M.Simple.Services.DependencyInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace M.Simple.Samples;

public static class DependencyInjectionExamples
{
    public static void Run()
    {
        var services = new ServiceCollection();
        services.AddAppServices();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var target = new GreetingTarget();
        // 属性装配需要显式执行，且应使用与目标对象相同的作用域。
        new AutowiredService(scope.ServiceProvider).Autowired(target);
        Verify.That(target.Greeting.GetText() == "你好，M.Simple", "属性装配失败");
        Console.WriteLine(target.Greeting.GetText());
    }
}

public interface IGreeting
{
    string GetText();
}

[AppService(ServiceType = typeof(IGreeting), Lifetime = ServiceLifetime.Scoped)]
public sealed class GreetingService : IGreeting
{
    public string GetText() => "你好，M.Simple";
}

public sealed class GreetingTarget
{
    [Autowired]
    public IGreeting Greeting { get; set; } = null!;
}
```

## 使用边界

建议显式指定 ServiceType，避免一个类实现多个接口时依赖接口枚举顺序。只扫描调用时已加载的程序集，插件程序集需先加载。

AddAppServices 只是注册服务，默认 Microsoft DI 不会因为出现 Autowired 特性就自动装配。示例显式调用 AutowiredService，并在同一 scope 中装配 scoped 服务；不要从根容器解析 scoped 依赖。

服务注册顺序与其他 DI 注册遵循 Microsoft DI 规则。此包不自动接管业务对象构造或对象生命周期。
