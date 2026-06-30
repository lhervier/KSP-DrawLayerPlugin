using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.ui.ugui.list
{
    /// <summary>
    /// Builds and refreshes the marker rows. Rebuilds the whole list on OnMarkersChanged (add / remove /
    /// update / visibility toggle): the list is small, so a full rebuild keeps the code simple and the
    /// rows always consistent with the saved order/indices.
    /// </summary>
    public class ListController : MonoBehaviour
    {
        private readonly List<MarkerRowController> _rows = new List<MarkerRowController>();

        private DrawLayerViewModel _viewModel;
        public ListController WithViewModel(DrawLayerViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public void Start()
        {
            if (_viewModel != null)
            {
                _viewModel.OnMarkersChanged.Add(Rebuild);
            }
            Rebuild();
        }

        public void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.OnMarkersChanged.Remove(Rebuild);
            }
        }

        private void Rebuild()
        {
            _rows.Clear();
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            IReadOnlyList<VisualMarker> markers = _viewModel.Markers;
            if (markers.Count == 0)
            {
                BuildEmptyState();
                return;
            }

            for (int i = 0; i < markers.Count; i++)
            {
                MarkerRowController row = new MarkerRowBuilder()
                    .WithViewModel(_viewModel)
                    .WithVisualMarker(markers[i])
                    .WithIndex(i)
                    .Build();
                row.transform.SetParent(transform, false);
                _rows.Add(row);
            }
        }

        private void BuildEmptyState()
        {
            var go = new GameObject("Empty", typeof(RectTransform));
            go.transform.SetParent(transform, false);

            var layout = go.AddComponent<VerticalLayoutGroup>();
            int pad = Mathf.RoundToInt(DrawLayerPalette.EmptyPadding);
            layout.padding = new RectOffset(pad, pad, pad, pad);
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var titleGo = new GameObject("Title", typeof(RectTransform));
            titleGo.transform.SetParent(go.transform, false);
            var title = UGUILabels.AddLabel(titleGo);
            title.text = ModLocalization.GetString("listEmptyTitle").ToUpperInvariant();
            title.fontSize = DrawLayerPalette.EmptyTitleFontSize;
            title.fontStyle = FontStyles.Bold;
            title.color = DrawLayerPalette.EmptyTitleColor;
            title.alignment = TextAlignmentOptions.Center;

            var hintGo = new GameObject("Hint", typeof(RectTransform));
            hintGo.transform.SetParent(go.transform, false);
            var hint = UGUILabels.AddLabel(hintGo);
            hint.text = ModLocalization.GetString("listEmptyHint");
            hint.fontSize = DrawLayerPalette.EmptyHintFontSize;
            hint.color = DrawLayerPalette.EmptyHintColor;
            hint.alignment = TextAlignmentOptions.Center;
            hint.enableWordWrapping = true;
        }
    }
}
