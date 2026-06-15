using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.ui.ugui.list
{
    /// <summary>
    /// Scrolled content of the marker list: a vertical stack rebuilt from the ViewModel. Mounted by the
    /// shared ScrollableView as its content. Shows one row per marker, or an empty-state message.
    /// </summary>
    public class ListBuilder : IUGUIBuilder<ListController>
    {
        private DrawLayerViewModel _viewModel;
        public ListBuilder WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public ListController Build()
        {
            var go = new GameObject("DrawLayer.List", typeof(RectTransform));

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            return go
                .AddComponent<ListController>()
                .WithViewModel(_viewModel);
        }
    }
}
