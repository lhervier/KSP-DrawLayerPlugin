using UnityEngine;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>
    /// Toggles the three sub-views (list / editor / settings) so that only the one matching
    /// ViewModel.CurrentView is active.
    /// </summary>
    public class ContentController : MonoBehaviour
    {
        private DrawLayerViewModel _viewModel;
        public ContentController WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private GameObject _listView;
        private GameObject _editorView;
        private GameObject _settingsView;
        public ContentController Views(GameObject listView, GameObject editorView, GameObject settingsView)
        {
            this._listView = listView;
            this._editorView = editorView;
            this._settingsView = settingsView;
            return this;
        }

        public void Start()
        {
            if (_viewModel != null)
            {
                _viewModel.OnCurrentViewChanged.Add(Apply);
            }
            Apply();
        }

        public void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.OnCurrentViewChanged.Remove(Apply);
            }
        }

        private void Apply()
        {
            DrawLayerView view = _viewModel.CurrentView;
            if (_listView != null) _listView.SetActive(view == DrawLayerView.List);
            if (_editorView != null) _editorView.SetActive(view == DrawLayerView.Editor);
            if (_settingsView != null) _settingsView.SetActive(view == DrawLayerView.Settings);
        }
    }
}
