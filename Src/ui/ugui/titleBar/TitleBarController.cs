using UnityEngine;
using TMPro;

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

        private TextMeshProUGUI _countLabel;
        public TitleBarController WithCountLabelComponent(TextMeshProUGUI label)
        {
            this._countLabel = label;
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
            if (_countLabel == null) return;
            _countLabel.text = $"{_viewModel.VisibleMarkersCount} / {_viewModel.TotalMarkersCount}";
        }
    }
}
