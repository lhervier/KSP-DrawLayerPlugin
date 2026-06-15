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
        public ModPopupBuilder ViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private Vector2 _position;
        private bool _hasPosition;
        public ModPopupBuilder Position(Vector2 position)
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
                .PopupID(DIALOG_ID)
                .Title(ModLocalization.GetString("windowTitle"))
                .TitleBarBuilder(
                    new TitleBarBuilder().ViewModel(_viewModel)
                )
                .ContentBuilder(
                    new ContentBuilder().ViewModel(_viewModel)
                )
                .Size(new Vector2(DrawLayerPalette.WindowWidth, DrawLayerPalette.WindowHeight));
            if (this._hasPosition)
            {
                popupBuilder = popupBuilder.Position(this._position);
            }
            return popupBuilder.Build();
        }
    }
}
