# PowerShell script to create Desktop and Start Menu shortcuts for MDReader
$targetExe = "D:\AI_PROJECTS\MDReader\src\MDReader.App\bin\x64\Debug\net10.0-windows10.0.26100.0\win-x64\MDReader.App.exe"
$iconPath = "D:\AI_PROJECTS\MDReader\src\MDReader.App\Assets\AppIcon.ico"
$workingDir = "D:\AI_PROJECTS\MDReader\src\MDReader.App\bin\x64\Debug\net10.0-windows10.0.26100.0\win-x64"

if (-not (Test-Path $targetExe)) {
    Write-Warning "Target exe not found at $targetExe. Building project first..."
    dotnet build "D:\AI_PROJECTS\MDReader\src\MDReader.App\MDReader.App.csproj" -c Debug
}

$wscript = New-Object -ComObject WScript.Shell

# 1. Desktop Shortcut
$desktopPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::Desktop)
$desktopShortcutPath = Join-Path $desktopPath "MDReader.lnk"
$desktopShortcut = $wscript.CreateShortcut($desktopShortcutPath)
$desktopShortcut.TargetPath = $targetExe
$desktopShortcut.WorkingDirectory = $workingDir
$desktopShortcut.IconLocation = "$iconPath, 0"
$desktopShortcut.Description = "MDReader - Google Antigravity-grade Markdown Reader and Editor"
$desktopShortcut.Save()
Write-Host "Created Desktop shortcut: $desktopShortcutPath" -ForegroundColor Green

# 2. Start Menu Shortcut
$startMenuPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::Programs)
$startShortcutPath = Join-Path $startMenuPath "MDReader.lnk"
$startShortcut = $wscript.CreateShortcut($startShortcutPath)
$startShortcut.TargetPath = $targetExe
$startShortcut.WorkingDirectory = $workingDir
$startShortcut.IconLocation = "$iconPath, 0"
$startShortcut.Description = "MDReader - Google Antigravity-grade Markdown Reader and Editor"
$startShortcut.Save()
Write-Host "Created Start Menu shortcut: $startShortcutPath" -ForegroundColor Green
