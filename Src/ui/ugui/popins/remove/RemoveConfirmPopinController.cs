using UnityEngine;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui.popin;

namespace com.github.lhervier.ksp.ui.ugui.popins.remove
{
    /// <summary>
    /// Orchestrates the remove-confirmation internal popin: on ViewModel.OnRemovalRequested, fills the
    /// shared confirm popin with the marker's name, wires its OK action to remove that marker, and shows
    /// it. Lives on the popin's always-active root so its lifecycle runs even while the popin is closed.
    /// </summary>
    public class RemoveConfirmPopinController : MonoBehaviour
    {
        private DrawLayerViewModel _viewModel;
        public RemoveConfirmPopinController WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private ConfirmPopinController _confirm;
        public RemoveConfirmPopinController WithConfirmPopin(ConfirmPopinController confirm)
        {
            this._confirm = confirm;
            return this;
        }

        public void Start()
        {
            _viewModel.OnRemovalRequested.Add(OnRemovalRequested);
        }

        public void OnDestroy()
        {
            _viewModel?.OnRemovalRequested.Remove(OnRemovalRequested);
        }

        public void OnDisable()
        {
            // Closing the window only deactivates it (it is not destroyed), which cascades here. Close the
            // popin so a reopened window never comes back with a stale confirmation still up.
            _confirm?.Close();
        }

        private void OnRemovalRequested(VisualMarker marker)
        {
            _confirm.SetMessage(ModLocalization.GetString("dialogRemoveMessageWithName", marker.name));
            _confirm.SetOkAction(() => _viewModel.RemoveMarker(marker));
            _confirm.Show();
        }
    }
}
