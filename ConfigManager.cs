using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class ConfigManager {
        private static readonly string CONFIG_FILE = "draw_layer.cfg";
        
        private readonly string configFilePath;
        private readonly List<VisualMarker> markers;
        private bool debugMode = false;
        
        public List<VisualMarker> Markers => markers;
        
        public ConfigManager() {
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string modDirectory = Path.GetDirectoryName(dllPath);
            configFilePath = Path.Combine(modDirectory, CONFIG_FILE);
            markers = new List<VisualMarker>();
        }
        
        public bool GetDebugMode() {
            return debugMode;
        }
        
        public void LoadMarkers() {
            if (!File.Exists(configFilePath)) {
                Logger.LogInfo("No configuration file found, starting with empty list");
                return;
            }
            ConfigNode configNode = ConfigNode.Load(configFilePath);
            if (configNode == null) {
                Logger.LogWarning("Invalid configuration file, starting with empty list");
                return;
            }
            
            markers.Clear();
            
            // Charger la configuration générale
            LoadGeneralConfig(configNode);
            
            // Charger les marqueurs
            ConfigNode markersNode = configNode.GetNode("MARKERS");
            if (markersNode != null) {
                ConfigNode[] markerNodes = markersNode.GetNodes();
                
                foreach (ConfigNode markerNode in markerNodes) {
                    if (markerNode.name.StartsWith("MARKER_")) {
                        VisualMarker marker = ParseMarkerFromConfigNode(markerNode);
                        if (marker != null) {
                            markers.Add(marker);
                        }
                    }
                }
            }
            
            Logger.LogInfo($"Loaded {markers.Count} markers from {configFilePath}");
        }
        
        private void LoadGeneralConfig(ConfigNode configNode) {
            try {
                ConfigNode generalNode = configNode.GetNode("GENERAL");
                if (generalNode != null) {
                    string debugValue = generalNode.GetValue("debug");
                    if (!string.IsNullOrEmpty(debugValue)) {
                        debugMode = (debugValue.ToLower() == "true" || debugValue == "1" || debugValue == "yes");
                    }
                }
            }
            catch (Exception ex) {
                Logger.LogError($"Error loading general config: {ex.Message}");
            }
        }
        
        public void SaveMarkers() {
            try {
                Logger.LogInfo($"Starting SaveMarkers. Markers count: {markers.Count}");
                Logger.LogInfo($"Config file path: {configFilePath}");
                
                // Créer le nœud de configuration principal
                ConfigNode configNode = new ConfigNode();
                
                // Section configuration générale
                ConfigNode generalNode = new ConfigNode("GENERAL");
                generalNode.SetValue("debug", debugMode.ToString().ToLower());
                configNode.AddNode(generalNode);
                
                // Section des repères
                ConfigNode markersNode = new ConfigNode("MARKERS");
                
                for (int i = 0; i < markers.Count; i++) {
                    var marker = markers[i];
                    Logger.LogInfo($"Saving marker {i}: {marker.name}, type: {marker.type}, pos: ({marker.positionX}, {marker.positionY}), color: {marker.color}");
                    
                    ConfigNode markerNode = new ConfigNode($"MARKER_{i}");
                    
                    markerNode.SetValue("name", marker.name);
                    markerNode.SetValue("type", marker.type.ToString());
                    markerNode.SetValue("positionX", marker.positionX.ToString());
                    markerNode.SetValue("positionY", marker.positionY.ToString());
                    markerNode.SetValue("radius", marker.radius.ToString());
                    markerNode.SetValue("showGraduations", marker.showGraduations.ToString().ToLower());
                    markerNode.SetValue("mainGraduationDivisions", marker.mainGraduationDivisions.ToString());
                    markerNode.SetValue("subGraduationDivisions", marker.subGraduationDivisions.ToString());
                    markerNode.SetValue("colorR", marker.color.r.ToString());
                    markerNode.SetValue("colorG", marker.color.g.ToString());
                    markerNode.SetValue("colorB", marker.color.b.ToString());
                    markerNode.SetValue("visible", marker.visible.ToString().ToLower());
                    
                    markersNode.AddNode(markerNode);
                }
                
                configNode.AddNode(markersNode);
                
                // Sauvegarder le fichier
                configNode.Save(configFilePath);
                Logger.LogInfo($"Configuration saved to {configFilePath}");
            }
            catch (Exception ex) {
                Logger.LogError($"Error saving configuration: {ex.Message}");
            }
        }
        
        private VisualMarker ParseMarkerFromConfigNode(ConfigNode markerNode) {
            try {
                VisualMarker marker = new VisualMarker();
                
                // Propriétés de base
                marker.name = markerNode.GetValue("name") ?? "Marker";
                
                string typeStr = markerNode.GetValue("type");
                if (!string.IsNullOrEmpty(typeStr) && Enum.TryParse<MarkerType>(typeStr, out MarkerType type)) {
                    marker.type = type;
                }
                
                // Position
                string posXStr = markerNode.GetValue("positionX");
                if (!string.IsNullOrEmpty(posXStr) && float.TryParse(posXStr, out float posX)) {
                    marker.positionX = posX;
                }
                
                string posYStr = markerNode.GetValue("positionY");
                if (!string.IsNullOrEmpty(posYStr) && float.TryParse(posYStr, out float posY)) {
                    marker.positionY = posY;
                }
                
                // Rayon (pour les cercles)
                string radiusStr = markerNode.GetValue("radius");
                if (!string.IsNullOrEmpty(radiusStr) && float.TryParse(radiusStr, out float radius)) {
                    marker.radius = radius;
                }
                
                // Graduations
                string graduationsStr = markerNode.GetValue("showGraduations");
                if (!string.IsNullOrEmpty(graduationsStr)) {
                    marker.showGraduations = graduationsStr.ToLower() == "true";
                }
                
                // Divisions de la graduation principale
                string mainGraduationDivisionsStr = markerNode.GetValue("mainGraduationDivisions");
                if (!string.IsNullOrEmpty(mainGraduationDivisionsStr) && int.TryParse(mainGraduationDivisionsStr, out int mainGraduationDivisions)) {
                    marker.mainGraduationDivisions = mainGraduationDivisions;
                }
                
                // Divisions des sous-graduations
                string subGraduationDivisionsStr = markerNode.GetValue("subGraduationDivisions");
                if (!string.IsNullOrEmpty(subGraduationDivisionsStr) && int.TryParse(subGraduationDivisionsStr, out int subGraduationDivisions)) {
                    marker.subGraduationDivisions = subGraduationDivisions;
                }
                
                // Couleur
                string colorRStr = markerNode.GetValue("colorR");
                if (!string.IsNullOrEmpty(colorRStr) && float.TryParse(colorRStr, out float r)) {
                    marker.color.r = r;
                }
                
                string colorGStr = markerNode.GetValue("colorG");
                if (!string.IsNullOrEmpty(colorGStr) && float.TryParse(colorGStr, out float g)) {
                    marker.color.g = g;
                }
                
                string colorBStr = markerNode.GetValue("colorB");
                if (!string.IsNullOrEmpty(colorBStr) && float.TryParse(colorBStr, out float b)) {
                    marker.color.b = b;
                }
                
                // Visibilité
                string visibleStr = markerNode.GetValue("visible");
                if (!string.IsNullOrEmpty(visibleStr)) {
                    marker.visible = visibleStr.ToLower() == "true";
                }
                
                return marker;
            }
            catch (Exception ex) {
                Logger.LogError($"Error parsing marker from config node: {ex.Message}");
                return null;
            }
        }
        
        public void AddMarker(VisualMarker marker) {
            Logger.LogInfo($"Adding marker: {marker.name}, type: {marker.type}, pos: ({marker.positionX}, {marker.positionY})");
            markers.Add(marker);
            SaveMarkers();
        }
        
        public void RemoveMarker(int index) {
            if (index >= 0 && index < markers.Count) {
                markers.RemoveAt(index);
                SaveMarkers();
            }
        }
        
        public void UpdateMarker(int index, VisualMarker marker) {
            if (index >= 0 && index < markers.Count) {
                markers[index] = marker;
                SaveMarkers();
            }
        }
    }
} 