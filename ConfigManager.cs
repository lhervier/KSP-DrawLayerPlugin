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
            string colorStr = markerNode.GetValue("color");
            if (!string.IsNullOrEmpty(colorStr) && Enum.TryParse<PredefinedColors>(colorStr, out PredefinedColors color)) {
                marker.color = color;
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
                
                // Main graduation divisions
                string divisionsStr = markerNode.GetValue("divisions");
                if (!string.IsNullOrEmpty(divisionsStr) && int.TryParse(divisionsStr, out int divisions)) {
                    marker.divisions = divisions;
                }
            }

            return marker;
        }

        // =====================================================================
        
        public void SaveConfig() {
            Logger.LogInfo($"Saving configuration. Markers count: {markers.Count}");
            Logger.LogInfo($"Config file path: {configFilePath}");
            
            // Create the main configuration node
            ConfigNode configNode = new ConfigNode();
            
            // General section
            configNode.AddNode(GenerateGeneralNode());
            
            // Markers section
            ConfigNode markersNode = new ConfigNode(MARKERS_NODE);
            configNode.AddNode(markersNode);
            
            for (int i = 0; i < markers.Count; i++) {
                var marker = markers[i];
                Logger.LogInfo($"Saving marker {i}: {marker.name}, type: {marker.type}");
                markersNode.AddNode(
                    GenerateMarkerNode(marker, i)
                );
            }
            
            // Save the configuration file
            configNode.Save(configFilePath);
            Logger.LogInfo($"Configuration saved to {configFilePath}");
        }

        private ConfigNode GenerateGeneralNode() {
            ConfigNode generalNode = new ConfigNode(GENERAL_NODE);
            generalNode.SetValue("debug", debugMode, true);
            return generalNode;
        }

        private ConfigNode GenerateMarkerNode(VisualMarker marker, int index) {
            ConfigNode markerNode = new ConfigNode($"MARKER_{index}");
            
            markerNode.SetValue("name", marker.name, true);
            markerNode.SetValue("type", marker.type.ToString(), true);
            markerNode.SetValue("positionX", marker.positionX, true);
            markerNode.SetValue("positionY", marker.positionY, true);
            markerNode.SetValue("radius", marker.radius, true);
            markerNode.SetValue("divisions", marker.divisions, true);
            markerNode.SetValue("color", marker.color.GetName(), true);
            markerNode.SetValue("visible", marker.visible, true);

            return markerNode;
        }

        // =====================================================================
        
        public void AddMarker(VisualMarker marker) {
            Logger.LogInfo($"Adding marker: {marker.name}, type: {marker.type}");
            markers.Add(marker);
            SaveConfig();
        }
        
        public void RemoveMarker(int index) {
            if (index < 0 ) return;
            if( index >= markers.Count) return;
            markers.RemoveAt(index);
            SaveConfig();
        }
        
        public void UpdateMarker(int index, VisualMarker marker) {
            if (index < 0 ) return;
            if( index >= markers.Count ) return;
            markers[index] = marker;
            SaveConfig();
        }
    }
} 