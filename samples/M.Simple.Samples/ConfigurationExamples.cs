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
