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

check_kspdir() {
    if [[ -z "${KSPDIR:-}" ]]; then
        die "the KSPDIR environment variable is not defined (KSP installation directory)"
    fi
    if [[ ! -d "$KSPDIR/GameData" ]]; then
        die "KSPDIR does not point to a valid KSP installation: $KSPDIR"
    fi
}

require_command unzip
check_kspdir

ZIP_FILE="Release/DrawLayerMod.zip"
[[ -f "$ZIP_FILE" ]] || die "archive not found: $ZIP_FILE (run ./build.sh first)"

MOD_DIR="$KSPDIR/GameData/DrawLayerMod"

echo "====================================="
echo "Removing the existing installation"
echo "====================================="
rm -rf "$MOD_DIR"

echo
echo "====================================="
echo "Extracting the mod"
echo "====================================="
mkdir -p "$MOD_DIR"
unzip -oq "$ZIP_FILE" -d "$MOD_DIR"

echo
echo "Mod installed in: $MOD_DIR"
echo "Run at: $(date)"
