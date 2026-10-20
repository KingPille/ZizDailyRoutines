$ErrorActionPreference = 'Stop'
$packageUrl = 'https://github.com/KingPille/ZizPlugins/releases/download/DailyRoutines-v1.0.0.0/ZizDailyRoutines-1.0.0.zip'
$expectedHash = '5fbc115a62631dcbaf18381612d697d0b2b260e1b60a7fce0e96d8cbebf005f1'
$temporaryZip = Join-Path ([System.IO.Path]::GetTempPath()) ('ZizDailyRoutines-' + [Guid]::NewGuid().ToString('N') + '.zip')
Invoke-WebRequest -Uri $packageUrl -OutFile $temporaryZip
if ((Get-FileHash -LiteralPath $temporaryZip -Algorithm SHA256).Hash.ToLowerInvariant() -ne $expectedHash) {
    throw 'Package SHA-256 verification failed.'
}
Expand-Archive -LiteralPath $temporaryZip -DestinationPath (Join-Path $PSScriptRoot 'lib') -Force
Write-Output 'Dependencies extracted to lib. Configure DALAMUD_HOME for an API 15 SDK.'
