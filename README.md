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
- **Color**: Color choice

#### Circle
A circle with optional angular graduations.

**Parameters:**
- **Position**: Center of the circle as a percentage of screen width and height
- **Color**: Color choice
- **Radius**: Circle size as a percentage of screen width
- **Graduations**: A thicker graduation at a specific angle

### User Interface

The interface allows you to:

1. **Create a new marker**: Click on "New Marker"
2. **Edit an existing marker**: Click on the marker name in the list
3. **Show/hide a marker**: Use the checkbox next to the name
4. **Delete a marker**: Click on "Del" next to the name
5. **Save changes**: Markers are automatically saved

### Usage Examples

#### For a Hohmann Transfer to another planet
1. Create a circle with graduations
2. Position it at the center of the screen
3. Set the main graduation to the optimal angle for your transfer
4. Wait for the planets to align according to your markers

#### For Docking
1. Create cross lines
2. Position them on the target docking port
3. Align your ship with these markers

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
        graduations = 0.0
        color = White
        visible = true
    }
}
```

## Development

### Compilation
```bash
# Set the KSPDIR environment variable to your KSP installation
set KSPDIR=C:\Path\To\KSP

# Build the project: Artifact will be in the Release folder
./build.bat

# Install in your KSP directory
./install.bat
```

## License

This mod is distributed under a free license. See the LICENSE file for details.

## Support

To report bugs or request features, please create an issue on the project's GitHub repository.
