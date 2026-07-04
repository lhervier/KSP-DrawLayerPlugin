using UnityEngine;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.titleBar;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.popup;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>
    /// Spawns the DrawLayer window on top of the shared PopupBuilder: supplies the title (left), the
    /// title bar's right column (count + new + settings buttons) and the content (list / editor /
    /// settings sub-views). Returns the shared PopupController the caller drives, or null if KSP failed
    /// to spawn the popup.
    /// </summary>
    public class ModPopupBuilder : IUGUIBuilder<PopupController>
    {
        private const string DIALOG_ID = "DrawLayerUGUI";

        // =============================================
        // Build parameters
        // =============================================

        private DrawLayerViewModel _viewModel;
        public ModPopupBuilder WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // =============================================
        // Builder
        // =============================================

        public PopupController Build()
        {
            // No overlay in DrawLayer (deletion is immediate, the type picker is inline, and list / editor
            // / settings are replacing views): O is a bare MonoBehaviour and WithOverlayBuilder is skipped.
            var popupBuilder = new PopupBuilder<TitleBarController, ContentController, MonoBehaviour>()
                .WithPopupID(DIALOG_ID)
                .WithTitle(ModLocalization.GetString("windowTitle"))
                .WithTitleBarBuilder(
                    new TitleBarBuilder().WithViewModel(_viewModel)
                )
                .WithContentBuilder(
                    new ContentBuilder().WithViewModel(_viewModel)
                )
                .WithSize(new Vector2(DrawLayerPalette.WindowWidth, DrawLayerPalette.WindowHeight));
            return popupBuilder.Build();
        }
    }
}
