using UnityEngine;
using TMPro;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.popin;

namespace com.github.lhervier.ksp.ui.ugui.overlays.remove
{
    /// <summary>
    /// Orchestrates the remove-confirmation internal popup: shows/closes it on ViewModel.HasPendingRemoval,
    /// fills the message with the pending marker's name, and wires the footer buttons (Remove / Cancel).
    /// Lives on the popup's always-active root so its lifecycle runs even while the popup is closed.
    /// </summary>
    public class RemoveConfirmOverlayController : MonoBehaviour
    {
        private DrawLayerViewModel _viewModel;
        public RemoveConfirmOverlayController WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private PopinController _popup;
        public RemoveConfirmOverlayController WithPopupController(PopinController popup)
        {
            this._popup = popup;
            return this;
        }

        private TextMeshProUGUI _message;
        public RemoveConfirmOverlayController WithMessageComponent(TextMeshProUGUI message)
        {
            this._message = message;
            return this;
        }

        private ButtonController _cancelButtonController;
        public RemoveConfirmOverlayController WithCancelButtonController(ButtonController cancelButtonController)
        {
            _cancelButtonController = cancelButtonController;
            return this;
        }

        private ButtonController _removeButtonController;
        public RemoveConfirmOverlayController WithRemoveButtonController(ButtonController removeButtonController)
        {
            _removeButtonController = removeButtonController;
            return this;
        }

        public void Start()
        {
            _viewModel.OnPendingRemovalChanged.Add(OnPendingRemovalChanged);
            OnPendingRemovalChanged();

            if (_cancelButtonController != null)
            {
                _cancelButtonController.OnClick.Add(_viewModel.CancelPendingRemoval);
            }
            if (_removeButtonController != null)
            {
                _removeButtonController.OnClick.Add(_viewModel.ConfirmPendingRemoval);
            }
        }

        public void OnDestroy()
        {
            if (_removeButtonController != null)
            {
                _removeButtonController.OnClick.Remove(_viewModel.ConfirmPendingRemoval);
            }
            if (_cancelButtonController != null)
            {
                _cancelButtonController.OnClick.Remove(_viewModel.CancelPendingRemoval);
            }

            _viewModel?.OnPendingRemovalChanged.Remove(OnPendingRemovalChanged);
        }

        private void OnPendingRemovalChanged()
        {
            VisualMarker pending = _viewModel.PendingRemovalMarker;
            if (pending != null)
            {
                if (_message != null)
                {
                    _message.text = ModLocalization.GetString("dialogRemoveMessageWithName", pending.name);
                }
                _popup?.Show();
            }
            else
            {
                _popup?.Close();
            }
        }
    }
}
