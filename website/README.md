# M.Simple 静态使用手册

保留原有静态站点框架、搜索、代码复制、移动端导航和 IIS 发布方式。当前提供四类基础功能、可运行示例、源码压缩包与一个 NuGet 包下载，不依赖外部 CDN。

## 本地站点

- 网站名称：M.Simple.Docs。
- 物理目录：D:/oym/M.Simple/website。
- 地址：http://127.0.0.1:8090/。
- 现有 IIS 站点直接指向 website，生成后即可访问新内容。

## 更新手册

在仓库根目录运行，要求 .NET 8 SDK、Node.js 14+ 和 Windows PowerShell：

```powershell
dotnet test M.Simple.sln --configuration Release
dotnet pack src/Extensions/M.Simple.Extensions --configuration Release --output artifacts/packages
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/Build-DocsSite.ps1
```

生成脚本读取 Markdown、实际示例输出和当前版本包，更新 HTML、全文搜索、源码视图及下载文件。CSS 和浏览器脚本位于 website/assets。

## 首次安装 IIS 站点

已存在的本地站点无需重复创建。首次安装时在管理员 Windows PowerShell 中运行：

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/Install-DocsIis.ps1
```

沿用独立无托管代码应用池、127.0.0.1:8090 绑定和只读权限。脚本核对现有目录与绑定，结果写入 artifacts/iis-deployment.json。IIS 需启用静态内容与默认文档。

源码 ZIP 包含 src、tests、samples、docs、tools、解决方案和站点资源；排除 .git、.vs、bin、obj、日志与缓存。部署其他机器时复制整个 website 目录即可，不应将仓库根目录设为站点物理目录。
