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
