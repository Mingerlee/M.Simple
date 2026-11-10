# M.Simple.Extensions.Cryptography

AES、RSA、哈希、签名与验签。

目标框架：.NET 8。由 M.Simple.Expand 1.0.1.8 按功能拆分，沿用原实现。

安装后使用同名命名空间：

```csharp
using M.Simple.Extensions.Cryptography;
```

迁移时需要更新旧命名空间及显式静态类调用；不提供旧程序集的二进制兼容层。
