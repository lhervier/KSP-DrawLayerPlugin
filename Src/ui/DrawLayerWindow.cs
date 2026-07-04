using UnityEngine;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.titleBar;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui.popup;
using com.github.lhervier.ksp.ui.ugui;

namespace com.github.lhervier.ksp.ui
{
    /// <summary>
    /// Manages the uGUI window lifecycle: lazy spawn, show/hide, in-session position memory, and OnClosed
    /// notification. The low-level mechanics (PopupDialog, position, Escape-to-close, scene change) are
    /// delegated to the shared PopupController.
    /// </summary>
    public sealed class DrawLayerWindow
    {
        private const string DIALOG_ID = "DrawLayerUGUI";

        private PopupController _popupController = null;
        private DrawLayerViewModel _viewModel;

        public EventVoid OnClosed = new EventVoid("DrawLayer.Window.OnClosed");

        public void Initialize(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public void Show()
        {
            // == null is sensitive to Unity destruction: after KSP closes the window (Escape), the
            // destroyed controller compares null here, which triggers a fresh spawn.
            if (_popupController == null)
            {
                // No overlay in DrawLayer (deletion is immediate, the type picker is inline, and list /
                // editor / settings are replacing views): O is a bare MonoBehaviour, WithOverlayBuilder skipped.
                _popupController = new PopupBuilder<TitleBarController, ContentController, MonoBehaviour>()
                    .WithPopupID(DIALOG_ID)
                    .WithTitle(ModLocalization.GetString("windowTitle"))
                    .WithTitleBarBuilder(
                        new TitleBarBuilder().WithViewModel(_viewModel)
                    )
                    .WithContentBuilder(
                        new ContentBuilder().WithViewModel(_viewModel)
                    )
                    .WithSize(new Vector2(DrawLayerPalette.WindowWidth, DrawLayerPalette.WindowHeight))
                    .Build();
                if (_popupController == null) return;   // KSP spawn failed
                _popupController.OnClosed.Add(OnPopupClosed);
            }
            _popupController.Show();
        }

        public void Hide()
        {
            // != is Unity's overloaded null check: a popup destroyed by KSP (Escape) compares null here
            // and is skipped (unlike ?. which would invoke Hide on the destroyed object).
            if (_popupController != null)
            {
                _popupController.Hide();
            }
        }

        public void Destroy()
        {
            if (_popupController != null)
            {
                _popupController.Dismiss();
                _popupController = null;
            }
        }

        private void OnPopupClosed()
        {
            OnClosed.Fire();
        }
    }
}
