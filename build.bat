@echo off
setlocal enabledelayedexpansion

echo.
echo -------------------------------------------
echo Detecting KSP structure
echo -------------------------------------------

REM KSPDIR must point to the KSP installation root.
if not defined KSPDIR (
    echo ERROR: The KSPDIR environment variable is not defined
    echo Please set KSPDIR to your KSP installation path
    echo Example: set "KSPDIR=C:\Program Files ^(x86^)\Steam\steamapps\common\Kerbal Space Program"
    exit /b 1
)

REM Detect the KSP DLLs (Windows or Linux layout).
REM Use !KSPDIR! inside ( ) blocks: the parentheses of "(x86)" otherwise break batch parsing.
if exist "!KSPDIR!\KSP_x64_Data\Managed\Assembly-CSharp.dll" (
    echo Windows layout detected ^(KSP_x64_Data^)
    set "KSP_DATA_DIR=!KSPDIR!\KSP_x64_Data"
) else if exist "!KSPDIR!\KSP_Data\Managed\Assembly-CSharp.dll" (
    echo Linux layout detected ^(KSP_Data^)
    set "KSP_DATA_DIR=!KSPDIR!\KSP_Data"
) else (
    echo ERROR: Assembly-CSharp.dll not found in !KSPDIR!\KSP_x64_Data\Managed\ or !KSPDIR!\KSP_Data\Managed\
    echo Check that KSPDIR points to the right KSP directory
    exit /b 1
)

echo Using KSPDIR: !KSPDIR!
echo Using KSP_DATA_DIR: !KSP_DATA_DIR!

echo.
echo ===============================
echo Building DrawLayerMod
echo ===============================

echo Removing Release folder
if exist Release rmdir /s /q Release

echo Creating Release folder
mkdir Release\DrawLayerMod
if errorlevel 1 (
    echo ERROR: Failed to create the Mod folder
    exit /b 1
)
mkdir Release\DrawLayerMod\Textures
if errorlevel 1 (
    echo ERROR: Failed to create the Textures folder
    exit /b 1
)
mkdir Release\DrawLayerMod\Localization
if errorlevel 1 (
    echo ERROR: Failed to create the Localization folder
    exit /b 1
)

echo Building Mod DLL
dotnet build DrawLayerMod.sln
if errorlevel 1 (
    echo ERROR: Failed to build the Mod DLL
    exit /b 1
)

echo Copying Mod dll file
copy /y "Output\bin\DrawLayerMod.dll" "Release\DrawLayerMod"
if errorlevel 1 (
    echo ERROR: Failed to copy the Mod DLL
    exit /b 1
)

echo Copying Config file
copy /y "draw_layer.cfg" "Release\DrawLayerMod"
if errorlevel 1 (
    echo ERROR: Failed to copy the Config file
    exit /b 1
)

echo Copying shared TMP sprite textures
copy /y "KSP-Shared\GameData\Textures\*" "Release\DrawLayerMod\Textures"
if errorlevel 1 (
    echo ERROR: Failed to copy the shared textures
    exit /b 1
)

echo Copying localization files
copy /y "GameData\DrawLayerMod\Localization\*" "Release\DrawLayerMod\Localization"
if errorlevel 1 (
    echo ERROR: Failed to copy the localization files
    exit /b 1
)

echo Zipping Mod
powershell -Command "Compress-Archive -Path 'Release\DrawLayerMod\*' -DestinationPath 'Release\DrawLayerMod.zip' -Force"
if errorlevel 1 (
    echo ERROR: Failed to zip the Mod
    exit /b 1
)

echo Removing Mod folder
rmdir /s /q Release\DrawLayerMod
if errorlevel 1 (
    echo ERROR: Failed to remove the Mod folder
    exit /b 1
)

echo.
echo Build Complete
echo.
echo Run at: %date% %time%
