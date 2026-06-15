#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

die() {
    echo "ERROR: $*" >&2
    exit 1
}

require_command() {
    command -v "$1" >/dev/null 2>&1 || die "command not found: $1"
}

detect_ksp_data_dir() {
    if [[ -z "${KSPDIR:-}" ]]; then
        die "the KSPDIR environment variable is not defined (KSP installation directory)"
    fi

    if [[ -f "$KSPDIR/KSP_x64_Data/Managed/Assembly-CSharp.dll" ]]; then
        echo "Windows layout detected (KSP_x64_Data)"
        KSP_DATA_DIR="$KSPDIR/KSP_x64_Data"
    elif [[ -f "$KSPDIR/KSP_Data/Managed/Assembly-CSharp.dll" ]]; then
        echo "Linux layout detected (KSP_Data)"
        KSP_DATA_DIR="$KSPDIR/KSP_Data"
    else
        die "Assembly-CSharp.dll not found in $KSPDIR/KSP_x64_Data/Managed/ or $KSPDIR/KSP_Data/Managed/"
    fi

    echo "Using KSPDIR: $KSPDIR"
    echo "Using KSP_DATA_DIR: $KSP_DATA_DIR"
}

echo "========="
echo "Building"
echo "========="

require_command dotnet
require_command zip
detect_ksp_data_dir

MSBUILD_PROPS=(-p:KSPDIR="$KSPDIR" -p:KSP_DATA_DIR="$KSP_DATA_DIR")

echo "Removing Release folder"
rm -rf Release

echo "Creating Release folder"
mkdir -p Release/DrawLayerMod/Textures Release/DrawLayerMod/Localization

echo "Restoring NuGet packages"
dotnet restore DrawLayerMod.sln "${MSBUILD_PROPS[@]}"

echo "Building the mod DLL (.NET Framework 4.7.2)"
dotnet build DrawLayerMod.sln "${MSBUILD_PROPS[@]}" --no-restore

echo "Copying the mod DLL"
cp -v Output/bin/DrawLayerMod.dll Release/DrawLayerMod/

echo "Copying the config file"
cp -v draw_layer.cfg Release/DrawLayerMod/

# TMP sprites (rendered inline via <sprite> in labels): shared icons (refresh_icon).
# Read at runtime from GameData/.../Textures.
echo "Copying shared textures (TMP sprites)"
cp -v KSP-Shared/GameData/Textures/* Release/DrawLayerMod/Textures/

echo "Copying localization files"
cp -v GameData/DrawLayerMod/Localization/* Release/DrawLayerMod/Localization/

echo "Creating the archive"
(
    cd Release/DrawLayerMod
    zip -qr ../DrawLayerMod.zip .
)

echo "Removing the intermediate folder"
rm -rf Release/DrawLayerMod

echo
echo "Build complete: Release/DrawLayerMod.zip"
echo "Run at: $(date)"
