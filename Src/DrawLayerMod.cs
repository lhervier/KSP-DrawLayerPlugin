using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using UnityEngine;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.ui;
using com.github.lhervier.ksp.ui.ugui;

namespace com.github.lhervier.ksp {

    [KSPAddon(KSPAddon.Startup.PSystemSpawn, true)]
    public class DrawLayerMod : MonoBehaviour {

        private static readonly ModLogger LOGGER = new ModLogger("DrawLayerMod");

        // Mod components
        private ConfigManager configManager;
        private MarkerRenderer markerRenderer;
        private DrawLayerViewModel viewModel;
        private DrawLayerWindow window;

        // Application launcher
        private ApplicationLauncherButton appLauncherButton;

        private void InitDebugMode() {
            try {
                if (configManager != null) {
                    bool debugMode = configManager.DebugMode;
                    ModLogger.SetLogLevel(debugMode ? LogLevel.Debug : LogLevel.Info);
                    LOGGER.LogInfo($"Debug mode initialized: {debugMode}");
                } else {
                    LOGGER.LogError("ConfigManager not available for debug mode initialization");
                }
            }
            catch (Exception ex) {
                LOGGER.LogError($"Error initializing debug mode: {ex.Message}");
            }
        }

        protected void Awake()
        {
            LOGGER.LogInfo("Awaked");
            DontDestroyOnLoad(this);

            configManager = new ConfigManager();
            markerRenderer = new MarkerRenderer();

            viewModel = gameObject.AddComponent<DrawLayerViewModel>();
            viewModel.Initialize(configManager);

            InitDebugMode();
        }

        public void Start() {
            LOGGER.LogInfo("Plugin started");
            configManager.LoadConfig();
            InitDebugMode();

            window = new DrawLayerWindow();
            window.Initialize(viewModel);
            window.OnClosed.Add(OnWindowClosed);
            viewModel.OnWindowVisibleChanged.Add(OnWindowVisibleChanged);

            // Add the button to the Application Launcher
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIApplicationLauncherDestroyed);
        }

        public void OnDestroy() {
            LOGGER.LogInfo("Plugin stopped");
            configManager.SaveConfig();

            markerRenderer?.Dispose();

            if (viewModel != null) {
                viewModel.OnWindowVisibleChanged.Remove(OnWindowVisibleChanged);
            }
            if (window != null) {
                window.OnClosed.Remove(OnWindowClosed);
                window.Destroy();
                window = null;
            }

            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Remove(OnGUIApplicationLauncherDestroyed);
            RemoveAppLauncherButton();
        }

        // ==========================================================================
        // Window visibility
        // ==========================================================================

        private void OnWindowVisibleChanged() {
            if (viewModel.WindowVisible) {
                window.Show();
            } else {
                // Reset to the list view (drops any editing draft, stops its live preview), then hide.
                viewModel.BackToList();
                window.Hide();
                if (appLauncherButton != null) {
                    appLauncherButton.SetFalse(false);
                }
            }
        }

        // The window was closed by KSP (Escape, scene change): fold the shared state back, which releases
        // the toolbar button via OnWindowVisibleChanged.
        private void OnWindowClosed() {
            viewModel.WindowVisible = false;
        }

        // ==========================================================================
        // Toolbar
        // ==========================================================================

        private void OnGUIApplicationLauncherReady() {
            if (ApplicationLauncher.Instance != null && appLauncherButton == null) {
                Texture2D iconTexture = CreateIconTexture();
                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                    OnAppLauncherTrue,
                    OnAppLauncherFalse,
                    null, null, null, null,
                    ApplicationLauncher.AppScenes.ALWAYS,
                    iconTexture
                );
                LOGGER.LogInfo("Application Launcher button added");
            }
        }

        private void OnGUIApplicationLauncherDestroyed() {
            RemoveAppLauncherButton();
        }

        private void RemoveAppLauncherButton() {
            if (appLauncherButton != null) {
                ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
                appLauncherButton = null;
                LOGGER.LogInfo("Application Launcher button removed");
            }
        }

        private void OnAppLauncherTrue() {
            viewModel.WindowVisible = true;
            LOGGER.LogDebug("UI opened via Application Launcher");
        }

        private void OnAppLauncherFalse() {
            viewModel.WindowVisible = false;
            LOGGER.LogDebug("UI closed via Application Launcher");
        }

        private Texture2D CreateIconTexture() {
            // 24x24 icon: a simple circle with four markers.
            Texture2D texture = new Texture2D(24, 24);
            Color[] pixels = new Color[24 * 24];

            Color backgroundColor = new Color(0, 0, 0, 0);
            Color iconColor = Color.white;

            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = backgroundColor;
            }

            Vector2 center = new Vector2(12, 12);
            float radius = 8f;
            for (int x = 0; x < 24; x++) {
                for (int y = 0; y < 24; y++) {
                    Vector2 pos = new Vector2(x, y);
                    float distance = Vector2.Distance(pos, center);
                    if (distance <= radius && distance >= radius - 2) {
                        pixels[y * 24 + x] = iconColor;
                    }
                }
            }

            pixels[6 * 24 + 12] = iconColor;
            pixels[12 * 24 + 6] = iconColor;
            pixels[12 * 24 + 18] = iconColor;
            pixels[18 * 24 + 12] = iconColor;

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        // ==========================================================================
        // Full-screen marker rendering
        // ==========================================================================

        public void OnRenderObject() {
            if (markerRenderer == null || viewModel == null) return;

            // Draw all saved markers, excluding the one currently being edited (its live draft is drawn
            // instead, so the editor previews the changes on screen).
            List<VisualMarker> markers = new List<VisualMarker>(configManager.Markers);
            VisualMarker editing = viewModel.EditingMarker;
            int editIndex = viewModel.EditingMarkerIndex;
            if (editIndex >= 0 && editIndex < markers.Count) {
                markers.RemoveAt(editIndex);
            }

            markerRenderer.DrawMarkers(markers, editing);
        }
    }
}
