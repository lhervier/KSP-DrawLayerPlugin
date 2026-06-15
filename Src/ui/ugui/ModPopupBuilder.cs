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

        private Vector2 _position;
        private bool _hasPosition;
        public ModPopupBuilder WithPosition(Vector2 position)
        {
            this._position = position;
            this._hasPosition = true;
            return this;
        }

        // =============================================
        // Builder
        // =============================================

        public PopupController Build()
        {
            var popupBuilder = new PopupBuilder<TitleBarController, ContentController>()
                .WithPopupID(DIALOG_ID)
                .WithTitle(ModLocalization.GetString("windowTitle"))
                .WithTitleBarBuilder(
                    new TitleBarBuilder().WithViewModel(_viewModel)
                )
                .WithContentBuilder(
                    new ContentBuilder().WithViewModel(_viewModel)
                )
                .WithSize(new Vector2(DrawLayerPalette.WindowWidth, DrawLayerPalette.WindowHeight));
            if (this._hasPosition)
            {
                popupBuilder = popupBuilder.WithPosition(this._position);
            }
            return popupBuilder.Build();
        }
    }
}
