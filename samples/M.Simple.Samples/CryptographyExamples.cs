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
