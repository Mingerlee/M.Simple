# 运行示例

要求安装 .NET 8 SDK。示例直接引用源码项目，无外部服务依赖。

```powershell
dotnet run --project samples/M.Simple.Samples --configuration Release -- all
```

| 参数 | 内容 |
|---|---|
| all | 全部示例，默认值 |
| enums | 枚举描述与选项 |
| strings | 手机号、邮箱及自定义脱敏 |
| collections | 集合差异与树结构 |
| comparison | 中文显示名与属性前后值 |

非法参数返回退出码 1。源码见 [Program.cs](Program.cs)，各功能安装及边界见 [使用手册](../../docs/usage.md)。
