@echo off
setlocal

set "MODS_DIR=C:\Program Files (x86)\Steam\steamapps\common\Farthest Frontier\Farthest Frontier (Mono)\Mods"
set "BUILD_DIR=%~dp0..\src\BetterWagons\bin\Release"

if not exist "%MODS_DIR%" mkdir "%MODS_DIR%"

echo Installing BetterWagons.dll to Mods folder...
copy /Y "%BUILD_DIR%\BetterWagons.dll" "%MODS_DIR%\"

if %ERRORLEVEL% NEQ 0 (
    echo Install failed!
    exit /b %ERRORLEVEL%
)

echo Install succeeded. DLL copied to %MODS_DIR%
