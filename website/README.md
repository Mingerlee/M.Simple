# M.Simple 静态文档网站

此目录可直接作为 IIS 网站或应用的物理目录。包含 17 个 HTML 页面、全文搜索、代码复制、移动端导航、示例源码和 4 个 NuGet 包下载，不依赖 Node.js 服务或外部 CDN。

## IIS 挂载

在 IIS 管理器中添加网站：

- 网站名称：`M.Simple.Docs`
- 物理路径：`D:\oym\M.Simple\website`
- 应用程序池：独立池，`.NET CLR 版本`选择`无托管代码`
- 绑定：HTTP，IP `127.0.0.1`，端口 `8090`
- 应用程序池身份对物理目录具有读取权限

访问 `http://127.0.0.1:8090/`，使用绑定的 IPv4 地址可避免 localhost 被解析到 IPv6 时访问失败。也可以作为现有站点下的应用挂载；页面和资源均使用相对路径。

仓库中提供部署脚本，在**管理员 Windows PowerShell**中运行：

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File D:\oym\M.Simple\tools\Install-DocsIis.ps1
```

脚本只创建指定站点及独立应用程序池，不修改现有其他站点，不开放防火墙。结果写入 `artifacts/iis-deployment.json`。如同名站点指向其他目录或端口已占用，脚本停止并保留现有配置。

`web.config` 已配置默认文档和 `.nupkg` MIME 类型。IIS 需要启用“静态内容”和“默认文档”；如服务器锁定了相应配置节，需要由管理员允许站点级配置。

## 更新网站

先按需生成最新 NuGet 包，再在仓库根目录执行：

```powershell
dotnet pack M.Simple.sln --configuration Release --output artifacts/packages
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/Build-DocsSite.ps1
```

生成过程读取 `docs` 和 `samples`，覆盖派生 HTML、搜索索引与下载文件；不会修改库源码。CSS 和浏览器脚本位于 `website/assets/site.css`、`website/assets/site.js`。只需将整个网站目录复制到服务器，无需在服务器上安装 Node.js 或 .NET SDK。

源码压缩包包含构建所需的项目与示例，排除 `.git`、`.vs`、`bin`、`obj`、日志和包缓存。不要把整个仓库根目录作为 IIS 物理目录。
