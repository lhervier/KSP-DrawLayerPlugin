using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;

namespace com.github.lhervier.ksp.ui.ugui.editor
{
    /// <summary>
    /// Drives the color grid: highlights the selected swatch (white border) and shows its name. Selection
    /// is exposed as a PredefinedColors value via OnSelected.
    /// </summary>
    public class ColorGridController : MonoBehaviour
    {
        public EventData<PredefinedColors> OnSelected = new EventData<PredefinedColors>("DrawLayer.ColorGrid.OnSelected");

        private static readonly PredefinedColors[] VALUES = (PredefinedColors[])Enum.GetValues(typeof(PredefinedColors));

        private Image[] _cells;
        private TextMeshProUGUI _nameLabel;
        private int _selected = 0;

        public ColorGridController Cells(Image[] cells, TextMeshProUGUI nameLabel)
        {
            this._cells = cells;
            this._nameLabel = nameLabel;
            return this;
        }

        public void Select(int index)
        {
            _selected = index;
            UpdateVisuals();
            OnSelected.Fire(VALUES[index]);
        }

        /// <summary>Set the selection from a color without firing OnSelected (model-to-view sync).</summary>
        public void SetSelected(PredefinedColors color)
        {
            _selected = Array.IndexOf(VALUES, color);
            if (_selected < 0) _selected = 0;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                bool sel = i == _selected;
                Color fill = VALUES[i].ToColor();
                Color border = sel ? DrawLayerPalette.ColorCellSelectedBorderColor : DrawLayerPalette.ColorCellBorderColor;
                int thickness = sel ? 2 : DrawLayerPalette.ColorCellBorderThickness;
                _cells[i].sprite = SpritesGlobal.Border(fill, border, thickness);
            }
            if (_nameLabel != null)
            {
                _nameLabel.text = VALUES[_selected].GetDisplayName().Trim();
            }
        }
    }
}
