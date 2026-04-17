@echo off
setlocal

set "GAME_DIR=C:\Program Files (x86)\Steam\steamapps\common\Farthest Frontier\Farthest Frontier (Mono)"
set "LIB_DIR=%~dp0..\lib"

if not exist "%LIB_DIR%" mkdir "%LIB_DIR%"

echo Copying MelonLoader DLLs...
copy /Y "%GAME_DIR%\MelonLoader\net35\MelonLoader.dll" "%LIB_DIR%\"
copy /Y "%GAME_DIR%\MelonLoader\net35\0Harmony.dll" "%LIB_DIR%\"

echo Copying game assemblies...
copy /Y "%GAME_DIR%\Farthest Frontier_Data\Managed\Assembly-CSharp.dll" "%LIB_DIR%\"
copy /Y "%GAME_DIR%\Farthest Frontier_Data\Managed\UnityEngine.dll" "%LIB_DIR%\"
copy /Y "%GAME_DIR%\Farthest Frontier_Data\Managed\UnityEngine.CoreModule.dll" "%LIB_DIR%\"
copy /Y "%GAME_DIR%\Farthest Frontier_Data\Managed\UnityEngine.AIModule.dll" "%LIB_DIR%\"

echo Done. Libraries copied to %LIB_DIR%
