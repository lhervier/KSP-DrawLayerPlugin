using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using UnityEngine;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui.popup;
using com.github.lhervier.ksp.ui;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui;
using com.github.lhervier.ksp.ui.ugui.titleBar;

namespace com.github.lhervier.ksp {

    [KSPAddon(KSPAddon.Startup.PSystemSpawn, true)]
    public class DrawLayerMod : MonoBehaviour {

        private static readonly ModLogger LOGGER = new ModLogger("DrawLayerMod");
        private const string DIALOG_ID = "DrawLayerUGUI";

        // Mod components
        private ConfigManager configManager;
        private MarkerRenderer markerRenderer;
        private DrawLayerViewModel viewModel;

        // uGUI window: a (shared) PopupController living on this GameObject that owns its own lazy
        // spawn, position, and open state. We only Show/Hide it and react to OnOpenChanged.
        private PopupController popupController;

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

            // The popup controller is a component on THIS GameObject: it survives KSP destroying the
            // window (Escape) and persists its own open state, so we no longer track visibility ourselves.
            // No overlay in DrawLayer (deletion is immediate, the type picker is inline, and list / editor /
            // settings are replacing views): O is a bare MonoBehaviour, WithOverlayBuilder skipped.
            popupController = new PopupBuilder<TitleBarController, ContentController, MonoBehaviour>()
                .WithHost(gameObject)
                .WithPopupID(DIALOG_ID)
                .WithTitle(ModLocalization.GetString("windowTitle"))
                .WithTitleBarBuilder(new TitleBarBuilder().WithViewModel(viewModel))
                .WithContentBuilder(new ContentBuilder().WithViewModel(viewModel))
                .WithSize(new Vector2(DrawLayerPalette.WindowWidth, DrawLayerPalette.WindowHeight))
                .Build();
            // The controller restores its own open state (in its Start, after this method returns), so we
            // only subscribe: a restored-open window then syncs the toolbar through this handler.
            if (popupController != null) {
                popupController.OnOpenChanged.Add(OnPopupOpenChanged);
            }

            // Add the button to the Application Launcher
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIApplicationLauncherDestroyed);
        }

        public void OnDestroy() {
            LOGGER.LogInfo("Plugin stopped");
            configManager.SaveConfig();

            markerRenderer?.Dispose();

            // popupController is a component on this GO: Unity destroys it with us, and it dismisses a still-
            // open window in its own OnDestroy. We only drop our reference and unsubscribe.
            if (popupController != null) {
                popupController.OnOpenChanged.Remove(OnPopupOpenChanged);
                popupController = null;
            }

            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Remove(OnGUIApplicationLauncherDestroyed);
            RemoveAppLauncherButton();
        }

        // ==========================================================================
        // Window visibility
        // ==========================================================================

        // The window's open state changed (button, ×, Escape, or restore-at-load): sync the toolbar
        // button, and on close reset to the list view (drops any editing draft, stops its live preview).
        private void OnPopupOpenChanged() {
            bool open = popupController != null && popupController.IsOpen;
            // Keep the toolbar button pressed state in sync, notably when the change is driven by KSP
            // (Escape) or by restore-at-load rather than by a click. SetTrue/SetFalse(false): do not
            // re-fire the toggle callbacks.
            if (appLauncherButton != null) {
                if (open) {
                    appLauncherButton.SetTrue(false);
                } else {
                    appLauncherButton.SetFalse(false);
                }
            }
            if (!open) {
                viewModel.BackToList();
            }
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
                // The launcher may become ready after the window state was restored at load: press the
                // button now to reflect an already-open window (false: no callback).
                if (appLauncherButton != null && popupController != null && popupController.IsOpen) {
                    appLauncherButton.SetTrue(false);
                }
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
            if (popupController != null) popupController.Show();
            LOGGER.LogDebug("UI opened via Application Launcher");
        }

        private void OnAppLauncherFalse() {
            if (popupController != null) popupController.Hide();
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
