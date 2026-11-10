/* Generate a standalone documentation website with Node.js 14+, without npm dependencies. */
const fs = require('fs');
const path = require('path');
const root = path.resolve(__dirname, '..');
const output = path.join(root, 'website');
const topics = [
  ['core', 'Core', '基础扩展', '字符串、日期、金额、集合与反射', 'Extensions'],
  ['chinese', 'Chinese', '中文工具', '拼音、首字母、中文日期与农历', 'Extensions'],
  ['datatable', 'DataTable', '数据表转换', '对象集合与 DataTable 互转', 'Extensions'],
  ['cryptography', 'Cryptography', '加解密', 'AES、RSA 与兼容摘要工具', 'Extensions'],
  ['configuration', 'Configuration', '配置读取', '配置文件加载与基础路径', 'Extensions'],
  ['expressions', 'Expressions', '表达式组合', 'And、Or、In 与参数替换', 'Extensions'],
  ['database', 'EntityFrameworkCore', '数据库查询', 'EF Core 与 Dapper 桥接', 'Extensions'],
  ['json', 'Json', 'JSON 序列化', '长整数转换与选项注册', 'Extensions'],
  ['di', 'DependencyInjection', '依赖注入', '服务扫描与属性装配', 'Services'],
  ['email', 'Email', '邮件服务', '配置、注册与发送模板', 'Services'],
  ['ftp', 'Ftp', 'FTP 服务', '上传、下载与服务注册', 'Services']
];
const pages = [
  { source: 'docs/usage.md', url: 'quickstart.html', title: '快速开始', label: '开始使用' },
  { source: 'samples/M.Simple.Samples/README.md', url: 'samples.html', title: '运行示例', label: '示例项目' },
  { source: 'docs/sample-output.md', url: 'sample-output.html', title: '示例运行结果', label: '验证结果' },
  { source: 'docs/bams-extensions.md', url: 'guides/bams.html', title: 'BAMS 方法对照', label: '迁移参考' },
  { source: 'README.md', url: 'migration.html', title: '项目与迁移说明', label: '迁移参考' }
].concat(topics.map(t => ({ source: `docs/usage/${t[0]}.md`, url: `guides/${t[0]}.html`, title: t[2], label: `M.Simple.${t[4]}.${t[1]}`, topic: t[0] })));
const pageBySource = new Map(pages.map(p => [p.source, p.url]));
const search = [];
const downloads = new Set();
const escape = text => String(text).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
const slug = text => text.toLowerCase().replace(/[`*]/g, '').replace(/[^\w\u3400-\u9fff\s-]/g, '').replace(/\s+/g, '-');
const write = (name, content) => { const file = path.join(output, name); fs.mkdirSync(path.dirname(file), { recursive: true }); fs.writeFileSync(file, content); };
function relative(from, target) { return path.posix.relative(path.posix.dirname(from), target) || path.posix.basename(target); }
function resolveLink(link, source, current) {
  if (/^(https?:|mailto:)/i.test(link)) return link;
  if (link.startsWith('#')) return link;
  const hashAt = link.indexOf('#');
  const pathname = hashAt < 0 ? link : link.slice(0, hashAt);
  const hash = hashAt < 0 ? '' : link.slice(hashAt);
  const resolved = path.posix.normalize(path.posix.join(path.posix.dirname(source), pathname));
  if (resolved.startsWith('../') || path.posix.isAbsolute(resolved)) throw Error('Unsafe link: ' + link);
  if (pageBySource.has(resolved)) return relative(current, pageBySource.get(resolved)) + hash;
  const file = path.join(root, resolved);
  if (!fs.existsSync(file) || !fs.statSync(file).isFile()) throw Error('Unresolved source link: ' + resolved);
  downloads.add(resolved);
  return relative(current, 'downloads/source/' + resolved + '.txt');
}
function inline(text, source, current) {
  let result = '', previous = 0;
  const tokens = /`([^`]+)`|\[([^\]]+)\]\(([^)]+)\)|\*\*([^*]+)\*\*/g;
  let match;
  while ((match = tokens.exec(text))) {
    result += escape(text.slice(previous, match.index));
    if (match[1] !== undefined) result += '<code>' + escape(match[1]) + '</code>';
    else if (match[2] !== undefined) result += '<a href="' + escape(resolveLink(match[3], source, current)) + '">' + escape(match[2]) + '</a>';
    else result += '<strong>' + escape(match[4]) + '</strong>';
    previous = tokens.lastIndex;
  }
  return result + escape(text.slice(previous));
}
function markdown(text, source, current) {
  const lines = text.replace(/^\uFEFF/, '').replace(/\r/g, '').split('\n');
  const toc = [], html = [], ids = new Map();
  let i = 0;
  const tableCells = line => line.trim().replace(/^\|/, '').replace(/\|$/, '').split('|').map(cell => cell.trim());
  const special = line => /^(#{1,6}\s|```|\s*[-*]\s|\s*\d+\.\s|\|)/.test(line);
  while (i < lines.length) {
    const line = lines[i];
    if (!line.trim()) { i++; continue; }
    if (line.startsWith('```')) {
      const language = line.slice(3).trim() || 'text', code = []; i++;
      while (i < lines.length && !lines[i].startsWith('```')) code.push(lines[i++]);
      if (i === lines.length) throw Error('Unclosed code fence: ' + source);
      i++;
      html.push(`<div class="code-block"><div class="code-toolbar"><span>${escape(language)}</span><button class="copy-button" type="button" aria-label="复制代码">复制代码</button></div><pre><code data-language="${escape(language)}">${escape(code.join('\n'))}</code></pre></div>`);
      continue;
    }
    const heading = /^(#{1,6})\s+(.+)$/.exec(line);
    if (heading) {
      const level = heading[1].length, base = slug(heading[2]), count = ids.get(base) || 0;
      ids.set(base, count + 1); const id = base + (count ? '-' + count : '');
      if (level > 1 && level < 4) toc.push({ id, text: heading[2].replace(/`/g, ''), level });
      html.push(`<h${level} id="${escape(id)}">${inline(heading[2], source, current)}</h${level}>`); i++; continue;
    }
    if (line.startsWith('|') && i + 1 < lines.length && /^\|[\s:|-]+\|?$/.test(lines[i + 1])) {
      const headers = tableCells(line); i += 2; const rows = [];
      while (i < lines.length && lines[i].startsWith('|')) rows.push(tableCells(lines[i++]));
      html.push('<div class="table-scroll"><table><thead><tr>' + headers.map(cell => '<th>' + inline(cell, source, current) + '</th>').join('') + '</tr></thead><tbody>' + rows.map(row => '<tr>' + row.map(cell => '<td>' + inline(cell, source, current) + '</td>').join('') + '</tr>').join('') + '</tbody></table></div>');
      continue;
    }
    if (/^\s*[-*]\s/.test(line) || /^\s*\d+\.\s/.test(line)) {
      const ordered = /^\s*\d+\.\s/.test(line), items = [], pattern = ordered ? /^\s*\d+\.\s+/ : /^\s*[-*]\s+/;
      while (i < lines.length && pattern.test(lines[i])) items.push(inline(lines[i++].replace(pattern, ''), source, current));
      html.push(`<${ordered ? 'ol' : 'ul'}>` + items.map(item => '<li>' + item + '</li>').join('') + `</${ordered ? 'ol' : 'ul'}>`); continue;
    }
    const paragraph = [line]; i++;
    while (i < lines.length && lines[i].trim() && !special(lines[i])) paragraph.push(lines[i++]);
    html.push('<p>' + inline(paragraph.join(' '), source, current) + '</p>');
  }
  return { html: html.join('\n'), toc };
}
function nav(url, prefix) {
  const item = (href, label, extra) => `<a class="nav-item ${url === href ? 'active' : ''}" href="${prefix + href}" ${url === href ? 'aria-current="page"' : ''}>${escape(label)}${extra ? '<small>' + escape(extra) + '</small>' : ''}</a>`;
  return `<a class="brand" href="${prefix}index.html"><span class="brand-icon">M.</span><span>M.Simple<small>开发者文档</small></span></a><div class="nav-scroll"><div class="nav-label">开始</div>${item('index.html', '文档首页')}${item('quickstart.html', '快速开始')}${item('samples.html', '运行示例')}${item('sample-output.html', '示例运行结果')}<div class="nav-label">扩展方法 <span>08</span></div>${topics.filter(t => t[4] === 'Extensions').map(t => item('guides/' + t[0] + '.html', t[1], t[2])).join('')}<div class="nav-label">服务组件 <span>03</span></div>${topics.filter(t => t[4] === 'Services').map(t => item('guides/' + t[0] + '.html', t[1], t[2])).join('')}<div class="nav-label">参考</div>${item('guides/bams.html', 'BAMS 方法对照')}${item('migration.html', '旧库迁移说明')}</div><div class="sidebar-footer"><span class="status-dot"></span>.NET 8 · v1.0.0</div>`;
}
function layout(url, title, label, content, toc) {
  const prefix = url.includes('/') ? '../' : '';
  return `<!doctype html>
<html lang="zh-CN"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><meta name="description" content="M.Simple 使用文档、API 说明与可运行示例"><title>${escape(title)} · M.Simple</title><link rel="icon" href="${prefix}assets/favicon.svg" type="image/svg+xml"><link rel="stylesheet" href="${prefix}assets/site.css"><script defer src="${prefix}assets/search-index.js"></script><script defer src="${prefix}assets/site.js"></script></head>
<body data-root="${prefix}"><a class="skip-link" href="#main-content">跳到正文</a><aside class="sidebar" id="sidebar">${nav(url, prefix)}</aside><div class="nav-backdrop" hidden></div><div class="workspace"><header class="topbar"><div class="topbar-left"><button class="menu-button" type="button" aria-label="切换导航" aria-controls="sidebar" aria-expanded="false">☰</button><span class="breadcrumb">文档<span>/</span>${escape(label)}</span></div><div class="topbar-actions"><button class="search-trigger" type="button"><span>搜索文档与 API</span><kbd>Ctrl K</kbd></button><a href="https://github.com/Mingerlee/M.Simple" target="_blank" rel="noopener" class="repository-link">GitHub ↗</a></div></header><div class="content-grid"><main id="main-content" class="${url === 'index.html' ? 'home' : 'document'}">${content}<footer class="page-footer">M.Simple <span>扩展方法与服务组件使用文档</span><a href="${prefix}index.html">返回首页 ↑</a></footer></main><aside class="page-toc"><div>本页内容</div>${toc.map(item => `<a class="toc-level-${item.level}" href="#${escape(item.id)}">${escape(item.text)}</a>`).join('')}</aside></div></div><dialog class="search-dialog"><div class="search-dialog-header"><label class="sr-only" for="search-input">搜索文档</label><input id="search-input" placeholder="搜索方法名、包名或关键字…" autocomplete="off"><button class="close-search" type="button" aria-label="关闭搜索">Esc</button></div><div class="search-results" aria-live="polite"></div><div class="search-hint">输入关键字搜索全部文档 · 按 Esc 关闭</div></dialog><div class="toast" role="status" aria-live="polite"></div></body></html>`;
}
const cards = category => topics.filter(t => t[4] === category).map(t => `<a class="package-card" href="guides/${t[0]}.html"><span class="package-icon">${escape(t[1].slice(0, 2))}</span><span class="card-arrow">↗</span><h3>${escape(t[1])}</h3><p>${escape(t[3])}</p><span class="package-name">M.Simple.${category}.${escape(t[1])}</span></a>`).join('');
const home = `<div class="eyebrow"><span class="status-dot"></span>.NET 8 开发工具库</div><section class="hero"><h1>M.Simple<br><span>使用文档与示例</span></h1><p>从基础扩展到独立服务，找到需要的功能包，<br class="desktop-break">直接查看调用方式与可运行代码。</p><div class="hero-actions"><a class="button primary" href="quickstart.html">快速开始 <span>→</span></a><a class="button secondary" href="samples.html">运行示例</a></div><div class="hero-metrics"><span><strong>11</strong>功能包</span><span><strong>11</strong>示例场景</span><span><strong>47</strong>通过的测试</span></div></section><section class="quick-command" id="run"><div><span class="section-kicker">从这里开始</span><h2>一条命令，运行全部示例</h2></div><div class="code-block"><div class="code-toolbar"><span>powershell · 仓库根目录</span><button class="copy-button" type="button">复制命令</button></div><pre><code data-language="powershell">dotnet run --project samples/M.Simple.Samples --configuration Release -- all</code></pre></div><p>使用本地源码运行。数据库采用 SQLite 内存库，邮件与 FTP 默认只演示服务注册。</p></section><section id="extensions"><div class="section-heading"><div><span class="section-kicker">EXTENSIONS</span><h2>扩展方法</h2></div><span class="count-badge">8 个包</span></div><p class="section-description">按需导入命名空间，直接调用。</p><div class="package-grid">${cards('Extensions')}</div></section><section id="services"><div class="section-heading"><div><span class="section-kicker">SERVICES</span><h2>服务组件</h2></div><span class="count-badge">3 个包</span></div><p class="section-description">随服务提供配置、接口与注册扩展。</p><div class="package-grid">${cards('Services')}</div></section><section class="reference-section" id="reference"><h2>继续阅读</h2><div class="reference-grid"><a href="guides/bams.html"><strong>BAMS 方法对照 <span>→</span></strong><p>核对来源、语义和迁移差异。</p></a><a href="sample-output.html"><strong>示例运行结果 <span>→</span></strong><p>查看全部场景的一次验证输出。</p></a><a href="migration.html"><strong>旧库迁移说明 <span>→</span></strong><p>了解命名空间、包依赖与兼容边界。</p></a></div></section><section class="download-section" id="downloads"><h2>本地下载</h2><p>包尚未发布到公共 NuGet 源。可下载源码或当前构建产物，按快速开始说明使用。</p><a class="button secondary" href="downloads/M.Simple.Source.zip" download>下载源码与示例 ↓</a><details><summary>查看 11 个 NuGet 包</summary><ul class="download-list">${topics.map(t => { const name = `M.Simple.${t[4]}.${t[1]}.1.0.0.nupkg`; return `<li><a href="downloads/packages/${name}" download>${name}</a></li>`; }).join('')}</ul></details></section>`;
write('index.html', layout('index.html', '使用文档与示例', '首页', home, [{ id: 'run', text: '运行示例', level: 2 }, { id: 'extensions', text: '扩展方法', level: 2 }, { id: 'services', text: '服务组件', level: 2 }, { id: 'reference', text: '继续阅读', level: 2 }, { id: 'downloads', text: '本地下载', level: 2 }]));
for (const page of pages) {
  const text = fs.readFileSync(path.join(root, page.source), 'utf8');
  const rendered = markdown(text, page.source, page.url);
  write(page.url, layout(page.url, page.title, page.label, `<div class="document-label">${escape(page.label)}</div>${rendered.html}`, rendered.toc));
  search.push({ title: page.title, label: page.label, url: page.url, text: text.replace(/[`#*|]/g, '').replace(/\s+/g, ' ').slice(0, 50000) });
}
for (const file of downloads) write('downloads/source/' + file + '.txt', fs.readFileSync(path.join(root, file)));
for (const t of topics) {
  const name = `M.Simple.${t[4]}.${t[1]}.1.0.0.nupkg`;
  const file = path.join(root, 'artifacts/packages', name);
  if (!fs.existsSync(file)) throw Error('Build packages before the website: ' + name);
  write('downloads/packages/' + name, fs.readFileSync(file));
}
write('assets/search-index.js', 'window.MSIMPLE_SEARCH=' + JSON.stringify(search).replace(/</g, '\\u003c') + ';\n');
write('assets/favicon.svg', '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64"><rect width="64" height="64" rx="16" fill="#2563eb"/><text x="9" y="44" fill="white" font-size="38" font-family="Arial,sans-serif" font-weight="bold">M.</text></svg>');
write('web.config', '<?xml version="1.0" encoding="utf-8"?>\n<configuration>\n  <system.webServer>\n    <defaultDocument><files><clear /><add value="index.html" /></files></defaultDocument>\n    <directoryBrowse enabled="false" />\n    <staticContent><remove fileExtension=".nupkg" /><mimeMap fileExtension=".nupkg" mimeType="application/octet-stream" /></staticContent>\n    <httpProtocol><customHeaders><remove name="X-Content-Type-Options" /><add name="X-Content-Type-Options" value="nosniff" /></customHeaders></httpProtocol>\n  </system.webServer>\n</configuration>\n');
console.log(`Generated ${pages.length + 1} HTML pages, search index, source views and 11 package downloads in ${output}`);
