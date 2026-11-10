using M.Simple.Services.Ftp;
using Microsoft.Extensions.DependencyInjection;

namespace M.Simple.Samples;

public static class FtpExamples
{
    public static void Run()
    {
        var services = new ServiceCollection();
        services.AddFtpClientSetup(_ => new FtpClientSettings
        {
            FtpServerIP = "ftp.example.com",
            FtpRemotePath = "incoming"
        });
        using var provider = services.BuildServiceProvider();
        _ = provider.GetRequiredService<IFtpClientService>();
        Console.WriteLine("FTP 服务注册成功；此示例不建立 FTP 连接。");
    }

    // 真实传输需由调用方配置连接后主动调用，默认示例入口不执行。
    public static Task<bool> UploadAsync(IFtpClientService service, string localFile, CancellationToken token) =>
        service.FtpUploadAsync(Path.GetFullPath(localFile), token);
}
