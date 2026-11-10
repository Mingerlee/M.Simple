# M.Simple.Services.Ftp：FTP 服务

依赖 DI Abstractions。可运行入口只注册并解析服务；UploadAsync 方法供接入真实服务器后主动调用。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/FtpExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Services.Ftp --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- ftp
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| 配置 | FtpClientSettings | FtpServerIP、FtpRemotePath、FtpUserID、FtpPassword |
| 注册 | AddFtpClientSetup | 配置与服务均为 singleton |
| 上传 | FtpUpload、FtpUploadAsync | 参数为本地文件完整路径 |
| 下载 | FtpDownload、FtpDownloadAsync | 分别传本地目录和远端文件名 |
| 文件操作 | FtpDelete、GetFtpFileList 等 | 以接口签名与服务器行为为准 |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
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
```

## 使用边界

FtpServerIP 传主机名或主机加端口，不带 ftp://；FtpRemotePath 传相对路径，不附加首尾斜杠。FtpUrl 由配置自动组合。账号从宿主配置读取，示例不提供凭据。

运行 ftp 或 all 不连接服务器。真实操作可通过 UploadAsync 包装方法主动调用。旧实现基于 FtpWebRequest，不能当作 SFTP 客户端；下载路径拼接使用 Windows 分隔符，跨平台下载需要额外验证。取消令牌用于部分异步流操作，不能承诺取消所有连接建立或目录操作。

配置可被 ResetFtpClientSettings 修改；singleton 服务并发使用时不在请求间随意切换账号或服务器。
