using UnityEngine;
using TMPro;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.combo;
using com.github.lhervier.ksp.shared.ugui.slider;
using com.github.lhervier.ksp.ui.ugui.editor;
using com.github.lhervier.ksp.shared.ugui.textfield;

namespace com.github.lhervier.ksp.ui.ugui.editor
{
    /// <summary>
    /// Binds the editor form fields to the ViewModel's editing draft. Field-change handlers (wired once
    /// in Start) mutate ViewModel.EditingMarker live — so the full-screen renderer previews the draft as
    /// the user edits. On each open (OnEnable) the fields are refreshed from the current draft.
    /// </summary>
    public class EditorFormController : MonoBehaviour
    {
        private DrawLayerViewModel _viewModel;
        public EditorFormController WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private TextFieldController _name;
        private TypeSelectorController _type;
        private SliderController _xSlider;
        private TextMeshProUGUI _xValue;
        private SliderController _ySlider;
        private TextMeshProUGUI _yValue;
        private ButtonController _recenter;
        private GameObject _circleSection;
        private SliderController _rSlider;
        private TextMeshProUGUI _rValue;
        private ComboController _divisions;
        private ColorGridController _colorGrid;

        public EditorFormController WithControls(
            TextFieldController name,
            TypeSelectorController type,
            SliderController xSlider, TextMeshProUGUI xValue,
            SliderController ySlider, TextMeshProUGUI yValue,
            ButtonController recenter,
            GameObject circleSection,
            SliderController rSlider, TextMeshProUGUI rValue,
            ComboController divisions,
            ColorGridController colorGrid)
        {
            _name = name;
            _type = type;
            _xSlider = xSlider; _xValue = xValue;
            _ySlider = ySlider; _yValue = yValue;
            _recenter = recenter;
            _circleSection = circleSection;
            _rSlider = rSlider; _rValue = rValue;
            _divisions = divisions;
            _colorGrid = colorGrid;
            return this;
        }

        public void Start()
        {
            _name.OnValueChanged.Add(OnNameChanged);
            _type.OnSelected.Add(OnTypeChanged);
            _xSlider.OnValueChanged.Add(OnXChanged);
            _ySlider.OnValueChanged.Add(OnYChanged);
            _rSlider.OnValueChanged.Add(OnRadiusChanged);
            _divisions.OnSelect.Add(OnDivisionsChanged);
            _colorGrid.OnSelected.Add(OnColorChanged);
            _recenter.OnClick.Add(OnRecenter);
        }

        public void OnDestroy()
        {
            _name.OnValueChanged.Remove(OnNameChanged);
            _type.OnSelected.Remove(OnTypeChanged);
            _xSlider.OnValueChanged.Remove(OnXChanged);
            _ySlider.OnValueChanged.Remove(OnYChanged);
            _rSlider.OnValueChanged.Remove(OnRadiusChanged);
            _divisions.OnSelect.Remove(OnDivisionsChanged);
            _colorGrid.OnSelected.Remove(OnColorChanged);
            _recenter.OnClick.Remove(OnRecenter);
        }

        // Refresh the fields from the current draft each time the editor is shown.
        public void OnEnable()
        {
            VisualMarker d = _viewModel != null ? _viewModel.EditingMarker : null;
            if (d == null) return;

            _name.SetText(d.name);
            _type.SetSelected((int)d.type);
            _xSlider.SetValue(d.positionX);
            _ySlider.SetValue(d.positionY);
            _rSlider.SetValue(d.radius);
            _divisions.SetOptions(EditorFormBuilder.DivisionOptions, d.divisions.ToString());
            _colorGrid.SetSelected(d.color);

            UpdateValueLabel(_xValue, d.positionX);
            UpdateValueLabel(_yValue, d.positionY);
            UpdateValueLabel(_rValue, d.radius);
            UpdateCircleSection();
        }

        // ==========================================================
        // Field handlers (mutate the live draft)
        // ==========================================================

        private void OnNameChanged(string value)
        {
            if (_viewModel.EditingMarker != null) _viewModel.EditingMarker.name = value;
        }

        private void OnTypeChanged(int index)
        {
            if (_viewModel.EditingMarker == null) return;
            _viewModel.EditingMarker.type = (MarkerType)index;
            UpdateCircleSection();
        }

        private void OnXChanged(float value)
        {
            if (_viewModel.EditingMarker == null) return;
            _viewModel.EditingMarker.positionX = value;
            UpdateValueLabel(_xValue, value);
        }

        private void OnYChanged(float value)
        {
            if (_viewModel.EditingMarker == null) return;
            _viewModel.EditingMarker.positionY = value;
            UpdateValueLabel(_yValue, value);
        }

        private void OnRadiusChanged(float value)
        {
            if (_viewModel.EditingMarker == null) return;
            _viewModel.EditingMarker.radius = value;
            UpdateValueLabel(_rValue, value);
        }

        private void OnDivisionsChanged(string value)
        {
            if (_viewModel.EditingMarker == null) return;
            if (int.TryParse(value, out int n)) _viewModel.EditingMarker.divisions = n;
        }

        private void OnColorChanged(PredefinedColors color)
        {
            if (_viewModel.EditingMarker != null) _viewModel.EditingMarker.color = color;
        }

        private void OnRecenter()
        {
            if (_viewModel.EditingMarker == null) return;
            _viewModel.EditingMarker.positionX = 50f;
            _viewModel.EditingMarker.positionY = 50f;
            _xSlider.SetValue(50f);
            _ySlider.SetValue(50f);
            UpdateValueLabel(_xValue, 50f);
            UpdateValueLabel(_yValue, 50f);
        }

        // ==========================================================
        // Helpers
        // ==========================================================

        private void UpdateCircleSection()
        {
            if (_circleSection == null || _viewModel.EditingMarker == null) return;
            _circleSection.SetActive(_viewModel.EditingMarker.type == MarkerType.Circle);
        }

        private static void UpdateValueLabel(TextMeshProUGUI label, float value)
        {
            if (label != null) label.text = ModLocalization.GetString("valuePercent", value.ToString("0.#"));
        }
    }
}
