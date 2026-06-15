using UnityEngine;
using com.github.lhervier.ksp.ui.ugui.editor;
using com.github.lhervier.ksp.ui.ugui.list;
using com.github.lhervier.ksp.ui.ugui.settings;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.scrollableview;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>
    /// Popup content (everything below the shared title bar): the three sub-views (scrollable marker
    /// list, editor form, settings) stacked on top of each other and filling the content host. Only one
    /// is active at a time; the ContentController toggles them on ViewModel.CurrentView, reproducing the
    /// "replacing views" navigation of the validated mockup. Mounted (and stretched to fill) by PopupBuilder.
    /// </summary>
    public class ContentBuilder : IUGUIBuilder<ContentController>
    {
        private DrawLayerViewModel _viewModel;
        public ContentBuilder ViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public ContentController Build()
        {
            var go = new GameObject("DrawLayer.Content", typeof(RectTransform));

            // Scrollable marker list
            ScrollableViewController list = new ScrollableViewBuilder<ListController>()
                .ObjectName("DrawLayer.ListView")
                .ContentBuilder(new ListBuilder().ViewModel(_viewModel))
                .Build();
            Fill(list.gameObject, go.transform);

            // Editor form
            EditorController editor = new EditorBuilder().ViewModel(_viewModel).Build();
            Fill(editor.gameObject, go.transform);

            // Settings
            SettingsController settings = new SettingsBuilder().ViewModel(_viewModel).Build();
            Fill(settings.gameObject, go.transform);

            return go
                .AddComponent<ContentController>()
                .ViewModel(_viewModel)
                .Views(list.gameObject, editor.gameObject, settings.gameObject);
        }

        // Stretch a child to fill the content root.
        private static void Fill(GameObject child, Transform parent)
        {
            child.transform.SetParent(parent, false);
            var rect = child.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
