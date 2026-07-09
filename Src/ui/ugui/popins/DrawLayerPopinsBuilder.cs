using UnityEngine;
using com.github.lhervier.ksp.ui.ugui.popins.remove;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.ui.ugui.popins
{
    /// <summary>
    /// Builds the DrawLayer window popins (currently the remove-confirmation internal popup) on a root.
    /// The PopupBuilder parents this root onto the window and stretches it full-window (above the content
    /// and title bar). Each popin self-manages its visibility through the ViewModel, so nothing needs
    /// wiring after Build.
    /// </summary>
    public class DrawLayerPopinsBuilder : IUGUIBuilder<DrawLayerPopinsController>
    {
        private DrawLayerViewModel _viewModel;
        public DrawLayerPopinsBuilder WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public DrawLayerPopinsController Build()
        {
            var rootGo = new GameObject("DrawLayer.Popins", typeof(RectTransform));

            new RemoveConfirmPopinBuilder()
                .WithViewModel(_viewModel)
                .WithParent(rootGo.transform)
                .Build();

            return rootGo.AddComponent<DrawLayerPopinsController>();
        }
    }
}
