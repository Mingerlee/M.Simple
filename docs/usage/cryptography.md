# M.Simple.Extensions.Cryptography：加解密与摘要

本页为统一包 `M.Simple.Extensions` 的功能分类，命名空间保留，安装时仅需引用统一扩展包。

依赖 Core。示例只验证 AES/RSA 往返，不输出密钥。

[返回使用总览](../usage.md) · [可运行源文件](../../samples/M.Simple.Samples/CryptographyExamples.cs) · [共享模型](../../samples/M.Simple.Samples/Models.cs)

## 安装与运行

```powershell
dotnet add package M.Simple.Extensions --version 1.0.0
```

当前包尚未发布到公共包源，请先按[本地安装说明](../usage.md#安装本地包)配置本地源。仓库内示例直接使用项目引用，在仓库根目录执行：

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- cryptography
```

## 常用 API

| 场景 | API | 说明 |
|---|---|---|
| AES | AesEncrypt、AesDecrypt | 无参数加密返回 CipherText、byte[] Key、byte[] IV |
| RSA | RsaEncrypt、RsaDecrypt | 字符串密钥为 XML 格式；显式选择公钥和私钥 |
| 分段 RSA | RsaEncryptWithBlock、RsaDecryptWithBlock | 按实现分段处理字节 |
| 签名 | RsaSignData、RsaVerifyData | 原实现使用 SHA1，核对对端协议 |
| 摘要 | MD5Encrypt、MD5Verify、MD5SaltEncrypt、MD5SaltVerify | 兼容摘要接口，不是可逆加密 |

## 示例

以下代码来自可运行项目；Product、OrderStatus、Verify 等共享类型见 Models.cs。Verify 仅用于示例自检，不属于包的公共 API。

```csharp
using M.Simple.Extensions.Cryptography;
using System.Security.Cryptography;

namespace M.Simple.Samples;

public static class CryptographyExamples
{
    public static void Run()
    {
        const string text = "M.Simple 示例";
        var aes = text.AesEncrypt();
        var restored = aes.CipherText.AesDecrypt(aes.Key, aes.IV);
        Verify.That(restored == text, "AES 往返失败");

        // 显式选择公钥加密、私钥解密，避免依赖旧便捷重载的密钥选择。
        using var rsa = RSA.Create(2048);
        var encrypted = text.RsaEncrypt(rsa.ToXmlString(includePrivateParameters: false));
        Verify.That(encrypted.RsaDecrypt(rsa.ToXmlString(includePrivateParameters: true)) == text, "RSA 往返失败");
        Console.WriteLine("AES、RSA 往返成功；未输出密钥。");
    }
}
```

## 使用边界

AES 的字符串密钥重载与 byte[] 重载按各自实现处理，示例使用返回的 byte[] Key/IV，避免猜测编码。无参数 RSA 便捷重载内部选择私钥进行加密，示例改用 .NET 生成密钥后显式公钥加密、私钥解密。内部 RSAEncryptHelper 不是公共 API。

MD5 不用于密码存储；原签名使用 SHA1，接入时应核对算法要求。示例密钥仅在内存中使用，输出与日志不包含密钥。
