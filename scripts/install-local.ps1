#requires -Version 5
# Installs the published plugin into the local Flow Launcher plugins folder.
# Flow holds the plugin DLL locked while running, so it must be fully stopped before the copy
# (a fixed sleep races Flow's restart) — wait until the process is actually gone, then replace.
param(
    [Parameter(Mandatory)] [string] $Source,
    [Parameter(Mandatory)] [string] $Dest
)

$ErrorActionPreference = 'Stop'

Get-Process Flow.Launcher -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

$tries = 0
while ((Get-Process Flow.Launcher -ErrorAction SilentlyContinue) -and $tries -lt 40) {
    Start-Sleep -Milliseconds 250
    $tries++
}

if (Test-Path $Dest) { Remove-Item -Recurse -Force $Dest }
Copy-Item -Recurse -Force $Source $Dest
Write-Host "Installed to $Dest"
