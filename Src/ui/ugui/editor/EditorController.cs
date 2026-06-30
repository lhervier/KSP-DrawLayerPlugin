using UnityEngine;
using TMPro;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui.button;

namespace com.github.lhervier.ksp.ui.ugui.editor
{
    /// <summary>
    /// Orchestrates the editor sub-view: wires the back/cancel/save navigation, and on each open refreshes
    /// the header title and the save-button label depending on whether a marker is being created or edited.
    /// The form fields bind themselves (see EditorFormController).
    /// </summary>
    public class EditorController : MonoBehaviour
    {
        private DrawLayerViewModel _viewModel;
        public EditorController WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private TextMeshProUGUI _titleLabel;
        public EditorController WithHeaderComponent(TextMeshProUGUI titleLabel)
        {
            this._titleLabel = titleLabel;
            return this;
        }

        private ButtonController _backButton;
        private ButtonController _cancelButton;
        private ButtonController _saveButton;
        public EditorController WithButtonControllers(ButtonController backButton, ButtonController cancelButton, ButtonController saveButton)
        {
            this._backButton = backButton;
            this._cancelButton = cancelButton;
            this._saveButton = saveButton;
            return this;
        }

        public void Start()
        {
            _backButton.OnClick.Add(OnCancel);
            _cancelButton.OnClick.Add(OnCancel);
            _saveButton.OnClick.Add(OnSave);
        }

        public void OnDestroy()
        {
            if (_backButton != null) _backButton.OnClick.Remove(OnCancel);
            if (_cancelButton != null) _cancelButton.OnClick.Remove(OnCancel);
            if (_saveButton != null) _saveButton.OnClick.Remove(OnSave);
        }

        // Refresh title + save label on each open (guarded: OnEnable can fire during AddComponent,
        // before the ViewModel is injected).
        public void OnEnable()
        {
            if (_viewModel == null) return;
            bool creating = _viewModel.IsCreatingMarker;
            if (_titleLabel != null)
            {
                _titleLabel.text = ModLocalization.GetString(creating ? "DLM_editorTitleNew" : "DLM_editorTitleEdit").ToUpperInvariant();
            }
            if (_saveButton != null)
            {
                _saveButton.SetLabel(ModLocalization.GetString(creating ? "DLM_buttonCreate" : "DLM_buttonSave"));
            }
        }

        private void OnCancel()
        {
            _viewModel.CancelEditing();
        }

        private void OnSave()
        {
            _viewModel.SaveEditing();
        }
    }
}
