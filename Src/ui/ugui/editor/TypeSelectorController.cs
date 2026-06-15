using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui.editor
{
    /// <summary>
    /// Drives the two-cell type selector. Index 0 = Cross, 1 = Circle (matches MarkerType order).
    /// </summary>
    public class TypeSelectorController : MonoBehaviour
    {
        /// <summary>Fired when the user picks a type (0 = Cross, 1 = Circle).</summary>
        public EventData<int> OnSelected = new EventData<int>("DrawLayer.TypeSelector.OnSelected");

        private Image _crossBg;
        private TextMeshProUGUI _crossLabel;
        private Image _circleBg;
        private TextMeshProUGUI _circleLabel;
        private int _selected = 0;

        public TypeSelectorController Segments(Image crossBg, TextMeshProUGUI crossLabel, Image circleBg, TextMeshProUGUI circleLabel)
        {
            this._crossBg = crossBg;
            this._crossLabel = crossLabel;
            this._circleBg = circleBg;
            this._circleLabel = circleLabel;
            return this;
        }

        public void Select(int index)
        {
            _selected = index;
            UpdateVisuals();
            OnSelected.Fire(index);
        }

        /// <summary>Set the selection without firing OnSelected (model-to-view sync).</summary>
        public void SetSelected(int index)
        {
            _selected = index;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            ApplySegment(_crossBg, _crossLabel, _selected == 0);
            ApplySegment(_circleBg, _circleLabel, _selected == 1);
        }

        private static void ApplySegment(Image bg, TextMeshProUGUI label, bool selected)
        {
            if (bg != null) bg.color = selected ? DrawLayerPalette.SegmentSelectedColor : Color.clear;
            if (label != null) label.color = selected ? DrawLayerPalette.SegmentSelectedTextColor : DrawLayerPalette.SegmentTextColor;
        }
    }
}
