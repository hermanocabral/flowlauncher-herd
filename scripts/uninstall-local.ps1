#requires -Version 5
# Removes the plugin from the local Flow Launcher plugins folder.
param(
    [Parameter(Mandatory)] [string] $Dest
)

$ErrorActionPreference = 'Stop'

if (Test-Path $Dest) {
    Get-Process Flow.Launcher -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500
    Remove-Item -Recurse -Force $Dest
    Write-Host "Removed $Dest"
}
else {
    Write-Host "Not installed: $Dest"
}
