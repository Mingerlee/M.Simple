# M.Simple.Services.Email：邮件服务

依赖 Core 和 DI Abstractions。可运行入口只注册并解析服务；SendAsync 方法供接入真实邮件配置后主动调用。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/EmailExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Services.Email --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- email
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| 配置 | MailSettings | StmpHostServer、PortNumber、EnableSSL、RequireAuth、FromAddress |
| 注册 | AddEMailSetup | 通过 IServiceProvider 工厂创建配置，服务为 singleton |
| 发送 | IEMailService.SendMail、SendMailAsync | 返回 MailResult；检查 Status 和 ErrorMsg |
| 内容 | MailContent | ToAddresses、Subject、Body、IsHTML、AttachmentPaths |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
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
```

## 使用边界

配置属性沿用旧拼写 `StmpHostServer`。真实账号与密码由调用方从配置或环境读取，示例只使用 example.com 的占位地址。附件路径应指向实际存在的文件。

SendAsync 示例没有被 Program 调用，运行 email 或 all 不会发送邮件。SendMailAsync 当前不提供 CancellationToken。错误既可能体现在 MailResult，也可能由底层操作抛出，调用方按接入场景处理。

当前旧实现的 CcAddresses、BccAddresses、ReplyToAddresses 分支读取的是 ToAddresses；使用这些功能前需修复并验证。本示例只演示 ToAddresses，不展示未经修复的抄送功能。
