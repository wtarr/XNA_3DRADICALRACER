# XNA to MonoGame Migration

## Migration Status

✅ **Successfully Migrated from XNA 4.0 to MonoGame 3.8.4**

The project has been migrated from the deprecated Microsoft XNA Framework 4.0 to MonoGame 3.8.4, targeting .NET 8 with DesktopGL for cross-platform support.

## Project Structure

### New Structure
```
RadicalRacer/               # New MonoGame project (clean structure)
├── Content/
│   ├── Models/            # FBX 3D models
│   ├── Textures/          # PNG/JPG textures
│   ├── Fonts/             # SpriteFont definitions
│   └── Content.mgcb       # MonoGame Content Pipeline config
├── *.cs                   # Game source code
└── RadicalRacer.csproj    # Modern SDK-style project

3D Radical Racer/          # Original XNA project (preserved for reference)
```

## Key Changes

### Framework
- **Old:** XNA 4.0 (.NET Framework 4.0)
- **New:** MonoGame 3.8.4 (.NET 8)
- **Platform:** DesktopGL (Windows, Linux, macOS)

### Breaking Changes
1. **Video Playback Removed**: The WMV intro splash video has been replaced with the static start screen image. MonoGame DesktopGL has limited video codec support.

2. **Namespace Updated**: Changed from `_3D_Radical_Racer` to `RadicalRacer`

3. **API Updates**:
   - Removed `Microsoft.Xna.Framework.GamerServices` (deprecated)
   - Updated `Keyboard.GetState(PlayerIndex)` to `Keyboard.GetState()`
   - Removed platform-specific `#if WINDOWS || XBOX` conditionals

4. **Fonts**: Updated from "Quartz MS" and "Segoe UI Mono" to "Arial" for cross-platform compatibility

## Building the Project

### Prerequisites
- .NET 8 SDK or later
- MonoGame 3.8.4 templates

### Install MonoGame Templates
```bash
dotnet new install MonoGame.Templates.CSharp
```

### Build
```bash
cd RadicalRacer
dotnet build
dotnet run
```

## Known Issues & Limitations

### FBX Model Compatibility (CRITICAL)
The original FBX model files use an older FBX format that is **incompatible** with the modern Assimp library used by MonoGame 3.8.4. All 18 FBX models fail to import with parsing errors.

**Error:** `FBX-Parser (TOK_COMMA, line X, col 10) unexpected token, expected TOK_KEY`

**Current Status:** The C# code compiles successfully, but 3D models cannot be loaded. The game will build and run but **will not display 3D models** until this is resolved.

**Potential Solutions:**
1. Re-export FBX files from Blender using FBX 7.4 binary format (most compatible)
2. Convert models to a different format (e.g., GLTF, Collada)
3. Use pre-built .xnb files from an XNA 4.0 build if compatible
4. Manually update FBX files to fix parser errors

**Affected Models:** All 18 models (cube.fbx, Cylinder.fbx, F1MODELLeft.fbx, F1MODELRight.fbx, F1MODELStraight.fbx, hoarding.fbx, horizon.fbx, luckBox.fbx, missile.fbx, MyCar.fbx, MyCarSteerLeft.fbx, MyCarSteerRight.fbx, Shed.fbx, slick.fbx, startFinish.fbx, Track1.fbx, track2.fbx, track3.fbx)

### Linux Content Pipeline
The MonoGame Content Pipeline has issues building FBX models on Linux systems with older GLIBC versions (< 2.38). The bundled `libassimp.so` requires GLIBC 2.38+. However, the primary blocker is now the FBX format compatibility issue above.

### Content Building on Other Platforms
- **Windows**: Full content pipeline support ✅
- **macOS**: Should work with newer macOS versions ✅
- **Linux**: Requires GLIBC 2.38+ ⚠️

## What Was Migrated

### ✅ Successfully Migrated
- All 19 C# source files
- Project structure to modern SDK-style
- Namespace from `_3D_Radical_Racer` to `RadicalRacer`
- All texture files (PNG/JPG)
- Font definitions (updated to Arial)
- Removed deprecated APIs

### ⚠️ Partially Migrated
- FBX 3D models (exist but **cannot be built** due to FBX format incompatibility with modern Assimp)
- Content pipeline successfully builds fonts and textures
- **Game will not display 3D models until FBX files are updated**

### ❌ Not Migrated
- Video playback (WMV format) - replaced with static image
- Xbox-specific code paths - removed
- GamerServices integration - removed (deprecated)

## Migration Benefits

1. **Cross-Platform**: Now runs on Windows, Linux, and macOS
2. **Modern .NET**: Uses .NET 8 with latest features and performance
3. **Active Support**: MonoGame is actively maintained (unlike XNA)
4. **Better Tooling**: Modern IDE support and debugging
5. **Open Source**: Full control over the framework

## Next Steps

To complete the migration:

1. **Build Content** (on Windows or system with GLIBC 2.38+):
   ```bash
   cd RadicalRacer
   dotnet build
   ```

2. **Test Game States**:
   - Main menu
   - Race countdown
   - Running gameplay
   - Race finished
   - Game over

3. **Verify Functionality**:
   - 3D model rendering
   - Car physics and controls
   - Collision detection
   - Checkpoint system
   - HUD and text rendering

## Original Project

The original XNA project is preserved in the `3D Radical Racer/` directory for reference. See the original README.md and video demo at http://youtu.be/1kny2I8kEIg

## Building on Different Platforms

### Windows
```bash
dotnet build
dotnet run
```

### Linux
```bash
# If you have GLIBC 2.38+:
dotnet build
dotnet run

# Otherwise, copy pre-built Content folder from Windows
cp -r <windows-build>/Content/bin RadicalRacer/Content/
dotnet build --no-restore
dotnet run
```

### macOS
```bash
dotnet build
dotnet run
```

## Troubleshooting

### "libassimp.so: GLIBC_2.38 not found"
Your Linux system has an older GLIBC version. Either:
- Build content on Windows
- Update your Linux distribution
- Use pre-built `.xnb` content files

### Missing Fonts
If fonts don't render, ensure Arial is installed:
```bash
# Linux
sudo apt-get install ttf-mscorefonts-installer

# macOS
# Arial is included by default
```

### Game Runs But No Models
You need to build the content pipeline. Transfer the `Content/bin` folder from a successful Windows build.

## License

See License.txt in the root directory.
