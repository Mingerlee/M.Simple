[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$websiteRoot = Join-Path $projectRoot 'website'

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
    foreach ($file in @('README.md', '.gitignore', 'M.Simple.sln', 'Directory.Build.props', 'Directory.Build.targets', 'migration-map.json', 'website/assets/site.css', 'website/assets/site.js')) {
        $sourceFiles += Get-Item -LiteralPath (Join-Path $projectRoot $file)
    }
    foreach ($file in $sourceFiles) {
        $relative = $file.FullName.Substring($projectRoot.Length + 1).Replace('\', '/')
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $file.FullName, $relative, [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
} finally { $archive.Dispose(); $zipStream.Dispose() }
Write-Output ('Website ready: ' + $websiteRoot)
Write-Output ('Source archive: ' + $zipPath)
