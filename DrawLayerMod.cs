using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.github.lhervier.ksp {
	
	[KSPAddon(KSPAddon.Startup.PSystemSpawn, true)]
    public class DrawLayerMod : MonoBehaviour {
        
        // Composants du mod
        private UIManager uiManager;
        private MarkerRenderer markerRenderer;
        private ConfigManager configManager;
        
        // Application launcher
        private ApplicationLauncherButton appLauncherButton;

        private void InitDebugMode() {
            try {
                // Utiliser le ConfigManager pour lire la configuration
                if (configManager != null) {
                    bool debugMode = configManager.DebugMode;
                    Logger.SetDebugMode(debugMode);
                    Logger.LogInfo($"Debug mode initialized: {debugMode}");
                } else {
                    Logger.LogError("ConfigManager not available for debug mode initialization");
                }
            }
            catch (Exception ex) {
                Logger.LogError($"Error initializing debug mode: {ex.Message}");
            }
        }

        protected void Awake() 
        {
            Logger.LogInfo("Awaked");
            DontDestroyOnLoad(this);
            
            // Initialiser les composants
            configManager = new ConfigManager();
            markerRenderer = new MarkerRenderer();
            uiManager = new UIManager(configManager);
            
            // Initialiser le mode debug après avoir créé le ConfigManager
            InitDebugMode();
        }

        public void Start() {
            Logger.LogInfo("Plugin started");
            configManager.LoadConfig();
            
            // Ajouter le bouton à l'Application Launcher
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIApplicationLauncherDestroyed);
        }

        public void OnDestroy() {
            Logger.LogInfo("Plugin stopped");
            configManager.SaveConfig();
            
            // Nettoyer les composants
            markerRenderer?.Dispose();
            
            // Supprimer le bouton de l'Application Launcher
            RemoveAppLauncherButton();
        }
        
        private void OnGUIApplicationLauncherReady() {
            if (ApplicationLauncher.Instance != null && appLauncherButton == null) {
                // Créer une texture pour l'icône (cercle simple)
                Texture2D iconTexture = CreateIconTexture();
                
                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                    OnAppLauncherTrue,    // Callback quand le bouton est activé
                    OnAppLauncherFalse,   // Callback quand le bouton est désactivé
                    null,                 // Callback quand la souris survole le bouton
                    null,                 // Callback quand la souris quitte le bouton
                    null,                 // Callback quand le bouton est cliqué
                    null,                 // Callback quand le bouton est cliqué avec le bouton droit
                    ApplicationLauncher.AppScenes.ALWAYS,
                    iconTexture
                );
                
                Logger.LogInfo("Application Launcher button added");
            }
        }
        
        private void OnGUIApplicationLauncherDestroyed() {
            RemoveAppLauncherButton();
        }
        
        private void RemoveAppLauncherButton() {
            if (appLauncherButton != null) {
                ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
                appLauncherButton = null;
                Logger.LogInfo("Application Launcher button removed");
            }
        }
        
        private void OnAppLauncherTrue() {
            uiManager.ShowUI = true;
            Logger.LogDebug("UI opened via Application Launcher");
        }
        
        private void OnAppLauncherFalse() {
            uiManager.ShowUI = false;
            Logger.LogDebug("UI closed via Application Launcher");
        }
        
        private Texture2D CreateIconTexture() {
            // Créer une texture 24x24 pixels
            Texture2D texture = new Texture2D(24, 24);
            Color[] pixels = new Color[24 * 24];
            
            // Couleur de fond (transparente)
            Color backgroundColor = new Color(0, 0, 0, 0);
            // Couleur de l'icône (blanc)
            Color iconColor = Color.white;
            
            // Remplir avec la couleur de fond
            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = backgroundColor;
            }
            
            // Dessiner un cercle simple
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
            
            // Ajouter quelques points pour représenter des repères
            pixels[6 * 24 + 12] = iconColor;  // Point central vertical
            pixels[12 * 24 + 6] = iconColor;  // Point central horizontal
            pixels[12 * 24 + 18] = iconColor; // Point central horizontal
            pixels[18 * 24 + 12] = iconColor; // Point central vertical
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return texture;
        }
        
        public void OnGUI() {
            uiManager?.DrawUI();
        }
        
        public void OnRenderObject() {
            markerRenderer?.DrawMarkers(configManager.Markers, uiManager?.PreviewMarker);
        }
    }
}
