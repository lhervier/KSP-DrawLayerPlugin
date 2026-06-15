using UnityEngine;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.checkbox;

namespace com.github.lhervier.ksp.ui.ugui.settings
{
    /// <summary>
    /// Behaviour of the settings sub-view: the back button returns to the list, the checkbox drives the
    /// debug flag. The checkbox is synced from the ViewModel each time the view is shown.
    /// </summary>
    public class SettingsController : MonoBehaviour
    {
        private DrawLayerViewModel _viewModel;
        public SettingsController WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private ButtonController _backButton;
        private CheckboxController _debugCheckbox;
        public SettingsController WithControls(ButtonController backButton, CheckboxController debugCheckbox)
        {
            this._backButton = backButton;
            this._debugCheckbox = debugCheckbox;
            return this;
        }

        public void Start()
        {
            _backButton.OnClick.Add(OnBack);
            _debugCheckbox.OnToggled.Add(OnDebugToggled);
        }

        public void OnDestroy()
        {
            if (_backButton != null) _backButton.OnClick.Remove(OnBack);
            if (_debugCheckbox != null) _debugCheckbox.OnToggled.Remove(OnDebugToggled);
        }

        // Sync the checkbox to the current flag each time the view is shown (guarded: OnEnable can fire
        // during AddComponent, before the ViewModel is injected).
        public void OnEnable()
        {
            if (_viewModel == null || _debugCheckbox == null) return;
            _debugCheckbox.SetChecked(_viewModel.DebugMode);
        }

        private void OnBack()
        {
            _viewModel.BackToList();
        }

        private void OnDebugToggled(bool value)
        {
            _viewModel.DebugMode = value;
        }
    }
}
