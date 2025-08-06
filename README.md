# DrawLayer - Visual Markers Mod for KSP

This mod allows you to display static visual markers on the KSP screen, like a transparent sheet stuck on your screen. It's particularly useful for:

- Waiting for the correct angle between two planets to plan a transfer
- Checking that your ship is properly aligned with a docking port
- Creating custom markers for your space maneuvers

## Installation

1. Download the mod
2. Place the `DrawLayerMod` folder in the KSP `GameData` directory
3. Launch KSP

## Usage

### Controls
- **Icon in the application bar**: Click on the DrawLayer icon (circle with markers) in the KSP application bar to show/hide the configuration interface

### Marker Types

#### Cross Lines
Two perpendicular lines (horizontal and vertical) that intersect at a precise point on the screen.

**Parameters:**
- **Position**: Defined as a percentage of screen width and height
- **Color**: RGB color choice

#### Circle
A circle with optional angular graduations.

**Parameters:**
- **Position**: Center of the circle as a percentage of screen width and height
- **Radius**: Circle size as a percentage of screen width
- **Graduations**: Option to display angular graduations
  - **Main graduation**: A thicker graduation at a specific angle
  - **Sub-graduations**: Minor graduations every 30 degrees
- **Color**: RGB color choice

### User Interface

The interface allows you to:

1. **Create a new marker**: Click on "New Marker"
2. **Edit an existing marker**: Click on the marker name in the list
3. **Show/hide a marker**: Use the checkbox next to the name
4. **Delete a marker**: Click on "Del" next to the name
5. **Save changes**: Markers are automatically saved

### Usage Examples

#### For a Hohmann Transfer
1. Create a circle with graduations
2. Position it at the center of the screen
3. Set the main graduation to the optimal angle for your transfer
4. Wait for the planets to align according to your markers

#### For Docking
1. Create cross lines
2. Position them on the target docking port
3. Align your ship with these markers

#### For an Orbital Maneuver
1. Create a circle without graduations
2. Position it to mark the maneuver zone
3. Use it as a visual guide for your corrections

## Configuration

The mod uses a single configuration file in native KSP format:

- `draw_layer.cfg`: General configuration and visual markers

### Configuration Format

The file uses the native KSP configuration format with sections:

```
GENERAL
{
    debug = false
}

MARKERS
{
    MARKER_0
    {
        name = My Marker
        type = CrossLines
        positionX = 50.0
        positionY = 50.0
        radius = 10.0
        showGraduations = false
        mainGraduationAngle = 0.0
        colorR = 1.0
        colorG = 1.0
        colorB = 1.0
        visible = true
    }
}
```

## Development

### Compilation
```bash
# Set the KSPDIR environment variable to your KSP installation
set KSPDIR=C:\Path\To\KSP

# Compile the project
msbuild DrawLayerMod.csproj
```

### Code Structure
- `DrawLayerMod.cs`: Main mod class
- `VisualMarker`: Class representing a visual marker
- `MarkerType`: Enumeration of marker types

## License

This mod is distributed under a free license. See the LICENSE file for details.

## Support

To report bugs or request features, please create an issue on the project's GitHub repository.
