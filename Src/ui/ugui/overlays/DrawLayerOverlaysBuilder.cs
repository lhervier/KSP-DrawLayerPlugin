using UnityEngine;
using com.github.lhervier.ksp.ui.ugui.overlays.remove;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.ui.ugui.overlays
{
    /// <summary>
    /// Builds the DrawLayer window overlays (currently the remove-confirmation internal popup) on a root.
    /// The PopupBuilder parents this root onto the window and stretches it full-window (above the content
    /// and title bar). Each overlay self-manages its visibility through the ViewModel, so nothing needs
    /// wiring after Build.
    /// </summary>
    public class DrawLayerOverlaysBuilder : IUGUIBuilder<DrawLayerOverlaysController>
    {
        private DrawLayerViewModel _viewModel;
        public DrawLayerOverlaysBuilder WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public DrawLayerOverlaysController Build()
        {
            var rootGo = new GameObject("DrawLayer.Overlays", typeof(RectTransform));

            new RemoveConfirmOverlayBuilder()
                .WithViewModel(_viewModel)
                .WithParent(rootGo.transform)
                .Build();

            return rootGo.AddComponent<DrawLayerOverlaysController>();
        }
    }
}
