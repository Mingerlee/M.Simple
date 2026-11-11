[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding
$projectRoot = Split-Path $PSScriptRoot -Parent
$websiteRoot = Join-Path $projectRoot 'website'

# 示例输出随手册生成，避免展示过期的运行结果。
$sampleOutput = & dotnet run --project (Join-Path $projectRoot 'samples/M.Simple.Samples') --configuration Release -- all
if ($LASTEXITCODE -ne 0) { throw ('Sample execution failed: ' + ($sampleOutput -join "`n")) }
$sampleMarkdown = "# 示例运行结果`n`n以下内容由文档构建脚本实际运行示例生成。`n`n" + '```text' + "`n" + ($sampleOutput -join "`n") + "`n" + '```' + "`n"
[System.IO.File]::WriteAllText((Join-Path $projectRoot 'docs/sample-output.md'), $sampleMarkdown, [System.Text.UTF8Encoding]::new($false))

& node (Join-Path $PSScriptRoot 'build-docs-site.cjs')
if ($LASTEXITCODE -ne 0) { throw 'Website generation failed.' }

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zipPath = Join-Path $websiteRoot 'downloads/M.Simple.Source.zip'
[System.IO.Directory]::CreateDirectory((Split-Path $zipPath -Parent)) | Out-Null
# Overwrite only this generated archive; do not change source files.
$zipStream = [System.IO.File]::Open($zipPath, [System.IO.FileMode]::Create)
$archive = [System.IO.Compression.ZipArchive]::new($zipStream, [System.IO.Compression.ZipArchiveMode]::Create)
try {
    $sourceFiles = @()
    foreach ($directory in @('src', 'samples', 'tests', 'docs', 'tools')) {
        $sourceFiles += Get-ChildItem -LiteralPath (Join-Path $projectRoot $directory) -Recurse -File |
            Where-Object { $_.FullName -notmatch '[\\/](bin|obj|TestResults)[\\/]' }
    }
    foreach ($file in @('README.md', '.gitignore', 'M.Simple.sln', 'Directory.Build.props', 'Directory.Build.targets', 'website/README.md', 'website/assets/site.css', 'website/assets/site.js')) {
        $sourceFiles += Get-Item -LiteralPath (Join-Path $projectRoot $file)
    }
    foreach ($file in $sourceFiles) {
        $relative = $file.FullName.Substring($projectRoot.Length + 1).Replace('\', '/')
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $file.FullName, $relative, [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
} finally { $archive.Dispose(); $zipStream.Dispose() }
Write-Output ('Website ready: ' + $websiteRoot)
Write-Output ('Source archive: ' + $zipPath)
