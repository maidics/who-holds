(Get-Item artifacts\bin\WhoHolds.Cli\release_win-x64\wh.dll).VersionInfo.ProductVersion
(Get-Item artifacts\bin\WhoHolds.Cli\release_win-x64\wh.dll).VersionInfo.FileVersion

# AssemblyVersion
[System.Reflection.AssemblyName]::GetAssemblyName("$PWD\artifacts\bin\WhoHolds.Cli\release_win-x64\wh.dll").Version.ToString()