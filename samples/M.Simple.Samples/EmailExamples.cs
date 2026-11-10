using M.Simple.Services.Email;
using Microsoft.Extensions.DependencyInjection;

namespace M.Simple.Samples;

public static class EmailExamples
{
    public static void Run()
    {
        var services = new ServiceCollection();
        services.AddEMailSetup(_ => new MailSettings
        {
            StmpHostServer = "smtp.example.com",
            PortNumber = 587,
            EnableSSL = true,
            FromAddress = "sender@example.com",
            DeliveryOption = MailDeliveryOptions.UseNetworkSmtp
        });
        using var provider = services.BuildServiceProvider();
        _ = provider.GetRequiredService<IEMailService>();
        Console.WriteLine("邮件服务注册成功；此示例不发送邮件。");
    }

    // 仅供调用方配置真实服务后主动调用，默认示例入口不执行此方法。
    public static async Task<MailResult> SendAsync(IEMailService service, string recipient)
    {
        return await service.SendMailAsync(new MailContent
        {
            ToAddresses = recipient,
            Subject = "M.Simple 示例",
            Body = "邮件正文",
            IsHTML = false
        });
    }
}
