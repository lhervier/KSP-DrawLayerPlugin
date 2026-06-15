using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.sprites;

namespace com.github.lhervier.ksp.ui.ugui.editor
{
    /// <summary>
    /// Grid of the predefined colors (mod-specific): one clickable swatch per PredefinedColors value,
    /// laid out in a fixed-column grid, with the selected color name shown below.
    /// </summary>
    public class ColorGridBuilder : IUGUIBuilder<ColorGridController>
    {
        public ColorGridController Build()
        {
            var rootGo = new GameObject("ColorGrid", typeof(RectTransform));
            var rootLayout = rootGo.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(0, 0, 0, 0);
            rootLayout.spacing = 6f;
            rootLayout.childAlignment = TextAnchor.UpperLeft;
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = true;
            rootLayout.childForceExpandHeight = false;

            var gridGo = new GameObject("Grid", typeof(RectTransform));
            gridGo.transform.SetParent(rootGo.transform, false);
            var grid = gridGo.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(DrawLayerPalette.ColorCellSize, DrawLayerPalette.ColorCellSize);
            grid.spacing = new Vector2(DrawLayerPalette.ColorCellSpacing, DrawLayerPalette.ColorCellSpacing);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = DrawLayerPalette.ColorGridColumns;
            grid.childAlignment = TextAnchor.UpperLeft;

            var values = (PredefinedColors[])Enum.GetValues(typeof(PredefinedColors));
            var cells = new Image[values.Length];
            var pointers = new PointerHandler[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                var cellGo = new GameObject("Color_" + values[i], typeof(RectTransform));
                cellGo.transform.SetParent(gridGo.transform, false);
                var cell = cellGo.AddComponent<Image>();
                cell.type = Image.Type.Sliced;
                cell.color = Color.white;
                cell.raycastTarget = true;
                cells[i] = cell;
                pointers[i] = cellGo.AddComponent<PointerHandler>();
            }

            var nameGo = new GameObject("ColorName", typeof(RectTransform));
            nameGo.transform.SetParent(rootGo.transform, false);
            var nameLabel = UGUILabels.AddLabel(nameGo);
            nameLabel.fontSize = DrawLayerPalette.ColorNameFontSize;
            nameLabel.color = DrawLayerPalette.ColorNameColor;
            nameLabel.alignment = TextAlignmentOptions.Left;

            ColorGridController controller = rootGo
                .AddComponent<ColorGridController>()
                .Cells(cells, nameLabel);

            for (int i = 0; i < pointers.Length; i++)
            {
                int index = i;   // capture
                pointers[i].OnClick = () => controller.Select(index);
            }

            return controller;
        }
    }
}
