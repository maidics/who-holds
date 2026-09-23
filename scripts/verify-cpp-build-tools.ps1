$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"

if (-not (Test-Path $vswhere)) {
    Write-Host "vswhere not found: no Visual Studio or Build Tools installed." -ForegroundColor Red
    return
}

$vs = & $vswhere -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath

if ($vs) {
    Write-Host "C++ build tools found at: $vs" -ForegroundColor Green
} else {
    Write-Host "Visual Studio is installed, but the C++ build tools (MSVC x64) are missing." -ForegroundColor Yellow
}