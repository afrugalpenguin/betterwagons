@echo off
setlocal

echo Building BetterWagons...
dotnet build "%~dp0..\src\BetterWagons\BetterWagons.csproj" -c Release

if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    exit /b %ERRORLEVEL%
)

echo Build succeeded.
