# Tools

To compile the plugin, you will need : 

- Windows
- .Net Framework 4.7
- NetCore 6

# Environment variables

"$KSPDIR": Points to the installation of KSP

# Submodule (KSP-Shared)

This mod compiles the shared UI library (KSP-Shared) directly from sources into a single DLL.
After a fresh clone, fetch the submodule first:

    git submodule update --init --recursive

To pull the latest shared library and re-pin it, run update-submodule.bat (or .sh).

# Scripts

build.bat will generate a Release Folder with the content of the release
install.bat will install the plugin into your KSP installation dir

