using UnityEngine;
using com.github.lhervier.ksp.shared.ugui.popup;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>
    /// Manages the uGUI window lifecycle: lazy spawn through ModPopupBuilder, show/hide, in-session
    /// position memory, and OnClosed notification. The low-level mechanics (PopupDialog, position,
    /// Escape-to-close, scene change) are delegated to the shared PopupController.
    /// </summary>
    public sealed class DrawLayerWindow
    {
        private PopupController _popup = null;
        private DrawLayerViewModel _viewModel;
        private Vector2? _savedPosition;

        public EventVoid OnClosed = new EventVoid("DrawLayer.Window.OnClosed");

        public void Initialize(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public void Show()
        {
            // == null is sensitive to Unity destruction: after KSP closes the window (Escape), the
            // destroyed controller compares null here, which triggers a fresh spawn.
            if (_popup == null)
            {
                var builder = new ModPopupBuilder().WithViewModel(_viewModel);
                if (_savedPosition.HasValue)
                {
                    builder = builder.WithPosition(_savedPosition.Value);
                }
                _popup = builder.Build();
                if (_popup == null) return;   // KSP spawn failed
                _popup.OnClosed.Add(OnPopupClosed);
                _popup.OnPositionCaptured.Add(OnPopupPositionCaptured);
            }
            _popup.Show();
        }

        public void Hide()
        {
            // != is Unity's overloaded null check: a popup destroyed by KSP (Escape) compares null here
            // and is skipped (unlike ?. which would invoke Hide on the destroyed object).
            if (_popup != null)
            {
                _popup.Hide();
            }
        }

        public void Destroy()
        {
            if (_popup != null)
            {
                _popup.Dismiss();
                _popup = null;
            }
        }

        private void OnPopupClosed()
        {
            OnClosed.Fire();
        }

        private void OnPopupPositionCaptured(Vector2 position)
        {
            _savedPosition = position;
        }
    }
}
