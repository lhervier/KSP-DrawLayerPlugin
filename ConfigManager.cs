using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class ConfigManager {
        private static readonly string CONFIG_FILE = "draw_layer.cfg";
        private static readonly string GENERAL_NODE = "GENERAL";
        private static readonly string MARKERS_NODE = "MARKERS";
        
        private readonly string configFilePath;
        private readonly List<VisualMarker> markers;
        private bool debugMode = false;
        
        public List<VisualMarker> Markers => markers;
        public bool DebugMode => debugMode;
        
        public ConfigManager() {
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string modDirectory = Path.GetDirectoryName(dllPath);
            configFilePath = Path.Combine(modDirectory, CONFIG_FILE);
            markers = new List<VisualMarker>();
            ResetConfig();
        }

        private void ResetConfig() {
            markers.Clear();
            debugMode = false;
        }
        
        // =====================================================================

        public void LoadConfig() {
            // Reset the configuration to default values
            ResetConfig();
            
            // Load the configuration file
            if (!File.Exists(configFilePath)) {
                Logger.LogInfo("No configuration file found, starting with empty list");
                return;
            }
            ConfigNode configNode = ConfigNode.Load(configFilePath);
            if (configNode == null) {
                Logger.LogWarning("Invalid configuration file, starting with empty list");
                return;
            }

            // Load general config
            ParseGeneralConfig(configNode);
            
            // Load markers
            ConfigNode markersNode = configNode.GetNode(MARKERS_NODE);
            if (markersNode == null) {
                Logger.LogInfo("No markers found in configuration file, starting with empty list");
                return;
            }
            ConfigNode[] markerNodes = markersNode.GetNodes();
            foreach (ConfigNode markerNode in markerNodes) {
                if (!markerNode.name.StartsWith("MARKER_")) continue;
                VisualMarker marker = ParseMarkerConfig(markerNode);
                if (marker != null) {
                    markers.Add(marker);
                }
            }
            
            Logger.LogInfo($"Loaded {markers.Count} markers from {configFilePath}");
        }
        
        private void ParseGeneralConfig(ConfigNode configNode) {
            ConfigNode generalNode = configNode.GetNode(GENERAL_NODE);
            if (generalNode == null) return;
            string debugValue = generalNode.GetValue("debug");
            if (string.IsNullOrEmpty(debugValue)) return;
            debugMode = (debugValue.ToLower() == "true" || debugValue == "1" || debugValue.ToLower() == "yes");
        }
        
        private VisualMarker ParseMarkerConfig(ConfigNode markerNode) {
            VisualMarker marker = new VisualMarker() {
                name = markerNode.GetValue("name") ?? "New Marker"
            };
            
            // =========================
            // Common properties
            // =========================

            // Position
            string posXStr = markerNode.GetValue("positionX");
            if (!string.IsNullOrEmpty(posXStr) && float.TryParse(posXStr, out float posX)) {
                marker.positionX = posX;
            }
            string posYStr = markerNode.GetValue("positionY");
            if (!string.IsNullOrEmpty(posYStr) && float.TryParse(posYStr, out float posY)) {
                marker.positionY = posY;
            }

            // Color
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
            
            // Visibility
            string visibleStr = markerNode.GetValue("visible");
            if (!string.IsNullOrEmpty(visibleStr)) {
                marker.visible = visibleStr.ToLower() == "true";
            }
            
            // Type
            string typeStr = markerNode.GetValue("type");
            if (!string.IsNullOrEmpty(typeStr) && Enum.TryParse<MarkerType>(typeStr, out MarkerType type)) {
                marker.type = type;
            }

            // =========================
            // Line properties
            // =========================
            if(marker.type == MarkerType.CrossLines) {
                // No additional properties
            }
            
            // =========================
            // Circle properties
            // =========================
            if(marker.type == MarkerType.Circle) {
            
                // Radius
                string radiusStr = markerNode.GetValue("radius");
                if (!string.IsNullOrEmpty(radiusStr) && float.TryParse(radiusStr, out float radius)) {
                    marker.radius = radius;
                }
                
                // Graduations
                string graduationsStr = markerNode.GetValue("showGraduations");
                if (!string.IsNullOrEmpty(graduationsStr)) {
                    marker.showGraduations = graduationsStr.ToLower() == "true";
                }
                
                // Main graduation divisions
                string mainGraduationDivisionsStr = markerNode.GetValue("mainGraduationDivisions");
                if (!string.IsNullOrEmpty(mainGraduationDivisionsStr) && int.TryParse(mainGraduationDivisionsStr, out int mainGraduationDivisions)) {
                    marker.mainGraduationDivisions = mainGraduationDivisions;
                }
                
                // Sub graduation divisions
                string subGraduationDivisionsStr = markerNode.GetValue("subGraduationDivisions");
                if (!string.IsNullOrEmpty(subGraduationDivisionsStr) && int.TryParse(subGraduationDivisionsStr, out int subGraduationDivisions)) {
                    marker.subGraduationDivisions = subGraduationDivisions;
                }
            }

            return marker;
        }

        // =====================================================================
        
        public void SaveConfig() {
            Logger.LogInfo($"Saving configuration. Markers count: {markers.Count}");
            Logger.LogInfo($"Config file path: {configFilePath}");
            
            // Créer le nœud de configuration principal
            ConfigNode configNode = new ConfigNode();
            
            // Section configuration générale
            configNode.AddNode(GenerateGeneralNode());
            
            // Section des repères
            ConfigNode markersNode = new ConfigNode(MARKERS_NODE);
            
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

        private ConfigNode GenerateGeneralNode() {
            ConfigNode generalNode = new ConfigNode(GENERAL_NODE);
            generalNode.SetValue("debug", debugMode.ToString().ToLower());
            return generalNode;
        }
        
        public void AddMarker(VisualMarker marker) {
            Logger.LogInfo($"Adding marker: {marker.name}, type: {marker.type}, pos: ({marker.positionX}, {marker.positionY})");
            markers.Add(marker);
            SaveConfig();
        }
        
        public void RemoveMarker(int index) {
            if (index >= 0 && index < markers.Count) {
                markers.RemoveAt(index);
                SaveConfig();
            }
        }
        
        public void UpdateMarker(int index, VisualMarker marker) {
            if (index >= 0 && index < markers.Count) {
                markers[index] = marker;
                SaveConfig();
            }
        }
    }
} 