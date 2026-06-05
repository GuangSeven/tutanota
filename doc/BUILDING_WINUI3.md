## WinUI3 (Windows App SDK) build via GitHub Actions

Local WinUI3 builds are intentionally not part of the supported flow. Use the GitHub Actions Windows runner instead.

### Run the workflow

1. Open the GitHub Actions tab.
2. Select "WinUI3 Build".
3. Click "Run workflow" and choose the branch.

### What it builds

- Project: windows/TutaWinUI/src/Tuta.WinUI/Tuta.WinUI.csproj
- Configuration: Release
- Platform: x64
- Packaging: disabled in CI by setting WindowsPackageType=None

If you need an MSIX package, update the workflow to set WindowsPackageType=MSIX and configure signing.
