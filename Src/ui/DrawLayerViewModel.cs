using System.Collections.Generic;
using UnityEngine;

namespace com.github.lhervier.ksp.ui
{
    /// <summary>
    /// UI state of the DrawLayer window, wrapping the ConfigManager (markers + debug flag). Mirrors the
    /// VesselBookmark ViewModel pattern: plain properties that fire KSP EventVoid on change, which the
    /// uGUI controllers subscribe to. The marker being edited is exposed as a draft (a copy) so the
    /// full-screen renderer can preview it live without touching the saved list.
    /// </summary>
    public class DrawLayerViewModel : MonoBehaviour
    {
        private ConfigManager _config;

        // =============================================================
        // Window visibility (drives the PopupDialog spawn/despawn)
        // =============================================================

        public bool WindowVisible
        {
            get => _windowVisible;
            set
            {
                if (_windowVisible == value) return;
                _windowVisible = value;
                OnWindowVisibleChanged.Fire();
            }
        }
        private bool _windowVisible = false;
        public readonly EventVoid OnWindowVisibleChanged = new EventVoid("DrawLayerViewModel.OnWindowVisibleChanged");

        // =============================================================
        // Current sub-view (list / editor / settings)
        // =============================================================

        public DrawLayerView CurrentView
        {
            get => _currentView;
            private set
            {
                if (_currentView == value) return;
                _currentView = value;
                OnCurrentViewChanged.Fire();
            }
        }
        private DrawLayerView _currentView = DrawLayerView.List;
        public readonly EventVoid OnCurrentViewChanged = new EventVoid("DrawLayerViewModel.OnCurrentViewChanged");

        // =============================================================
        // Markers
        // =============================================================

        /// <summary>The saved markers (live reference owned by the ConfigManager).</summary>
        public IReadOnlyList<VisualMarker> Markers => _config.Markers;

        /// <summary>Fired whenever the marker list changes (add / remove / update / visibility toggle).</summary>
        public readonly EventVoid OnMarkersChanged = new EventVoid("DrawLayerViewModel.OnMarkersChanged");

        public int TotalMarkersCount => _config.Markers.Count;

        public int VisibleMarkersCount
        {
            get
            {
                int n = 0;
                foreach (var m in _config.Markers)
                {
                    if (m.visible) n++;
                }
                return n;
            }
        }

        // =============================================================
        // Marker edition (draft)
        // =============================================================

        /// <summary>The marker currently being edited (a draft copy), or null when not editing.</summary>
        public VisualMarker EditingMarker => _editingMarker;
        private VisualMarker _editingMarker = null;

        /// <summary>Index of the edited marker in the saved list, or -1 when creating a new one.</summary>
        public int EditingMarkerIndex => _editingMarkerIndex;
        private int _editingMarkerIndex = -1;

        /// <summary>Whether the editor is creating a new marker (vs editing an existing one).</summary>
        public bool IsCreatingMarker => _editingMarkerIndex < 0;

        // =============================================================
        // Debug flag
        // =============================================================

        public bool DebugMode
        {
            get => _config.DebugMode;
            set
            {
                if (_config.DebugMode == value) return;
                _config.SetDebugMode(value);
                OnDebugModeChanged.Fire();
            }
        }
        public readonly EventVoid OnDebugModeChanged = new EventVoid("DrawLayerViewModel.OnDebugModeChanged");

        // =============================================================
        // Lifecycle
        // =============================================================

        public void Initialize(ConfigManager config)
        {
            this._config = config;
        }

        // =============================================================
        // Navigation
        // =============================================================

        public void OpenSettings()
        {
            CurrentView = DrawLayerView.Settings;
        }

        public void BackToList()
        {
            _editingMarker = null;
            _editingMarkerIndex = -1;
            CurrentView = DrawLayerView.List;
        }

        // =============================================================
        // Marker actions
        // =============================================================

        /// <summary>Start creating a new marker and switch to the editor.</summary>
        public void NewMarker()
        {
            _editingMarkerIndex = -1;
            _editingMarker = new VisualMarker();
            CurrentView = DrawLayerView.Editor;
        }

        /// <summary>Start editing the marker at the given index (on a draft copy) and switch to the editor.</summary>
        public void EditMarker(int index)
        {
            if (index < 0 || index >= _config.Markers.Count) return;
            _editingMarkerIndex = index;
            _editingMarker = new VisualMarker(_config.Markers[index]);
            CurrentView = DrawLayerView.Editor;
        }

        /// <summary>Commit the draft (create or update), then go back to the list.</summary>
        public void SaveEditing()
        {
            if (_editingMarker == null) return;
            if (IsCreatingMarker)
            {
                _config.AddMarker(_editingMarker);
            }
            else
            {
                _config.UpdateMarker(_editingMarkerIndex, _editingMarker);
            }
            _editingMarker = null;
            _editingMarkerIndex = -1;
            CurrentView = DrawLayerView.List;
            OnMarkersChanged.Fire();
        }

        /// <summary>Discard the draft and go back to the list.</summary>
        public void CancelEditing()
        {
            BackToList();
        }

        public void RemoveMarker(int index)
        {
            if (index < 0 || index >= _config.Markers.Count) return;
            _config.RemoveMarker(index);
            OnMarkersChanged.Fire();
        }

        /// <summary>Flip the visibility of the marker at the given index and persist it.</summary>
        public void ToggleVisibility(int index)
        {
            if (index < 0 || index >= _config.Markers.Count) return;
            VisualMarker marker = _config.Markers[index];
            marker.visible = !marker.visible;
            _config.UpdateMarker(index, marker);
            OnMarkersChanged.Fire();
        }
    }
}
