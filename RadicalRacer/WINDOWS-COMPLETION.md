# Complete Migration on Windows

## Overview
The MonoGame migration has been completed on Linux, with all C# code compiling successfully. However, the FBX content building requires Windows (or newer Linux/macOS) to complete. This guide covers the final steps.

## Status Before Windows Build
- ✅ All C# code migrated and compiling (0 errors)
- ✅ All content files copied and configured
- ⚠️ FBX models not yet built (Linux GLIBC limitation)
- ✅ Textures and fonts build successfully

## Steps to Complete on Windows

### 1. Clone and Checkout the Migration Branch

```bash
# If you haven't cloned the repo yet:
git clone https://github.com/wtarr/XNA_3DRADICALRACER.git
cd XNA_3DRADICALRACER

# Checkout the migration branch:
git checkout monogame-migration
```

### 2. Install Prerequisites on Windows

```powershell
# Install .NET 8 SDK if not already installed
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0

# Verify .NET 8 is installed
dotnet --version
# Should show 8.0.x or higher

# Install MonoGame templates
dotnet new install MonoGame.Templates.CSharp
```

### 3. Restore the Content.mgcb File

The Linux build temporarily removed FBX models from Content.mgcb. Restore them:

```powershell
cd RadicalRacer/Content

# Restore the full content pipeline configuration
# (A backup was created: Content.mgcb.full)
if (Test-Path Content.mgcb.full) {
    Copy-Item Content.mgcb.full Content.mgcb
}
```

If Content.mgcb.full doesn't exist, add the FBX models back manually by running:

```powershell
# This will add all FBX models back to the content pipeline
$models = @(
    "cube", "Cylinder", "F1MODELLeft", "F1MODELRight", "F1MODELStraight",
    "hoarding", "horizon", "luckBox", "missile", "MyCar", "MyCarSteerLeft",
    "MyCarSteerRight", "Shed", "slick", "startFinish", "Track1", "track2", "track3"
)

foreach ($model in $models) {
    Write-Host "Adding $model.fbx to content pipeline..."
    dotnet mgcb-editor-console add Models/$model.fbx `
        --importer:FbxImporter `
        --processor:ModelProcessor `
        --build:Models/$model.fbx
}
```

### 4. Build the Content Pipeline

```powershell
cd RadicalRacer

# Clean previous build
dotnet clean

# Build the project (this will build content and code)
dotnet build
```

**Expected Output:**
```
Building content...
  - Fonts/CountDownFont.spritefont
  - Fonts/HUDfont.spritefont
  - Fonts/LapsCompletes.spritefont
  - Fonts/ResultsFont.spritefont
  - Models/cube.fbx
  - Models/Cylinder.fbx
  - ... (all FBX models)
  - Textures/... (all textures)

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 5. Test the Game

```powershell
# Run the game
dotnet run
```

**Test Checklist:**
- [ ] Game window opens
- [ ] Main menu displays with start screen image
- [ ] "Press ENTER to start" text shows
- [ ] Pressing Enter starts the race
- [ ] 3D models render (cars, track, obstacles)
- [ ] Camera follows the car
- [ ] Car controls work (arrow keys/WASD)
- [ ] HUD displays lap counter, position, etc.
- [ ] Race finish screen appears
- [ ] Multiple tracks work

### 6. Commit the Built Content (Optional)

If you want to include pre-built content in the repo for Linux users:

```powershell
# The content is built to Content/bin/DesktopGL/Content
# These .xnb files can be committed for cross-platform use

git add RadicalRacer/Content/bin/DesktopGL/
git commit -m "Add pre-built content for cross-platform support

Built on Windows with MonoGame 3.8.4 content pipeline.
Includes all FBX models, textures, and fonts.

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
git push
```

### 7. Merge to Master (When Ready)

After testing and confirming everything works:

```bash
git checkout master
git merge monogame-migration
git push
```

## Troubleshooting

### "Could not load file or assembly" Error
- Ensure .NET 8 SDK is installed
- Run `dotnet restore` before building

### FBX Import Errors
- Verify FBX files are in `Content/Models/` directory
- Check Content.mgcb has correct paths (case-sensitive)
- Ensure MonoGame templates are installed

### Missing Textures/Models in Game
- Check Content/bin/DesktopGL/Content/ directory exists
- Verify .xnb files are present
- Check content is set to "Copy if newer" or is being built

### Black Screen on Launch
- Content may not be built correctly
- Check Content/obj/ for build errors
- Verify Content.RootDirectory = "Content" in code

## Performance Notes

**First Build:**
- May take 2-5 minutes (building all FBX models)
- Assimp library processes each model
- Textures are compressed

**Subsequent Builds:**
- Much faster (only rebuilds changed content)
- MonoGame caches processed content

## What Changed from XNA

When testing, note these intentional changes:
1. **No video splash** - Replaced with static start screen image
2. **Cross-platform** - Now runs on Windows, Linux, macOS
3. **Modern .NET** - Uses .NET 8 instead of .NET Framework 4.0
4. **Deprecated APIs removed** - GamerServices, old keyboard methods

## Success Criteria

✅ **Migration is complete when:**
1. `dotnet build` completes with 0 errors
2. Game launches and displays main menu
3. All 3D models render correctly
4. Gameplay is functional (racing, physics, collisions)
5. All game states work (menu, racing, finished, game over)
6. HUD and text render properly

## Next Steps After Completion

1. **Update README.md** in root with MonoGame build instructions
2. **Tag the release**: `git tag v2.0-monogame && git push --tags`
3. **Consider enhancements**:
   - Add gamepad support (already has some)
   - Improve graphics (shaders, effects)
   - Add more tracks
   - Multiplayer support
   - Sound effects and music

## Contact / Issues

If you encounter issues during Windows build:
1. Check MIGRATION.md for detailed documentation
2. Review commit history: `git log --oneline`
3. Compare with original XNA project on master branch

---

**Branch:** monogame-migration
**Pushed to:** https://github.com/wtarr/XNA_3DRADICALRACER
**Pull Request:** https://github.com/wtarr/XNA_3DRADICALRACER/pull/new/monogame-migration
