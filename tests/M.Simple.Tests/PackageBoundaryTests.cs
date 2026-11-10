using M.Simple.Extensions.Configuration;
using M.Simple.Services.DependencyInjection;
using M.Simple.Services.DependencyInjection.Attributes;
using M.Simple.Services.Email;
using M.Simple.Services.Ftp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace M.Simple.Tests;

/// <summary>验证拆包后的依赖边界、配置加载和服务注册。</summary>
[TestClass]
public class PackageBoundaryTests
{
    [TestMethod]
    public void CoreDoesNotReferenceIntegrationPackages()
    {
        var references = typeof(M.Simple.Extensions.Core.StringExtensions).Assembly.GetReferencedAssemblies();
        Assert.IsFalse(references.Any(reference => reference.Name.StartsWith("M.Simple.Services.")));
        Assert.IsFalse(references.Any(reference => reference.Name.StartsWith("Microsoft.Extensions.")));
        Assert.IsFalse(references.Any(reference => reference.Name.Contains("PinYin")));
    }

    [TestMethod]
    public void EmailAndFtpRegistrationsResolveTheirOwnServices()
    {
        // 只构造服务，避免测试发送邮件或连接 FTP。
        var mailSettings = new MailSettings();
        var ftpSettings = new FtpClientSettings();
        var services = new ServiceCollection();
        services.AddEMailSetup(_ => mailSettings);
        services.AddFtpClientSetup(_ => ftpSettings);
        using var provider = services.BuildServiceProvider();

        Assert.AreSame(mailSettings, ((EMailService)provider.GetRequiredService<IEMailService>()).Settings);
        Assert.AreSame(ftpSettings, ((FtpClientService)provider.GetRequiredService<IFtpClientService>()).Settings);
    }

    [TestMethod]
    public void AppServicesAndAutowiredRemainInDependencyInjectionPackage()
    {
        var services = new ServiceCollection();
        services.AddAppServices();
        using var provider = services.BuildServiceProvider();
        var target = new InjectionTarget();
        provider.GetRequiredService<AutowiredService>().Autowired(target);

        Assert.AreSame(provider.GetRequiredService<InjectionDependency>(), target.Dependency);
    }

    [TestMethod]
    public void ConfigurationLoadsJsonFromTheConfiguredBasePath()
    {
        var directory = Path.Combine(Path.GetTempPath(), "m-simple-config-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var file = Path.Combine(directory, "settings.json");
        try
        {
            File.WriteAllText(file, "{\"PackageName\":\"M.Simple\"}");
            var configuration = new ConfigurationBuilder()
                .SetConfigurationBasePath(directory)
                .AddJsonFileWithBasePath("settings.json")
                .Build();
            using var configurationLifetime = (IDisposable)configuration;
            Assert.AreEqual("M.Simple", configuration["PackageName"]);
        }
        finally
        {
            File.Delete(file);
            Directory.Delete(directory);
        }
    }
}

[AppService]
public class InjectionDependency { }

public class InjectionTarget
{
    [Autowired]
    public InjectionDependency Dependency { get; set; }
}
