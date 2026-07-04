using UnityEngine;
using com.github.lhervier.ksp.shared.ugui.badge;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class TitleBarController : MonoBehaviour
    {
        private DrawLayerViewModel _viewModel;
        public TitleBarController WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private BadgeController _countBadge;
        public TitleBarController WithCountBadge(BadgeController badge)
        {
            this._countBadge = badge;
            return this;
        }

        public void Start()
        {
            if (_viewModel != null)
            {
                _viewModel.OnMarkersChanged.Add(UpdateCount);
                UpdateCount();
            }
        }

        public void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.OnMarkersChanged.Remove(UpdateCount);
            }
        }

        private void UpdateCount()
        {
            if (_countBadge == null) return;
            _countBadge.SetText($"{_viewModel.VisibleMarkersCount} / {_viewModel.TotalMarkersCount}");
        }
    }
}
