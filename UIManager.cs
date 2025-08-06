using System.Collections.Generic;
using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class UIManager {
        private readonly ConfigManager configManager;
        
        // Interface utilisateur
        private bool showUI = false;
        private Rect windowRect = new Rect(50, 50, 400, 600);
        private Vector2 scrollPosition = Vector2.zero;
        
        // État de l'interface
        private int selectedMarkerIndex = -1;
        private VisualMarker newMarker = new VisualMarker();
        private bool isCreatingNewMarker = false;
        
        public bool ShowUI {
            get => showUI;
            set => showUI = value;
        }
        
        public UIManager(ConfigManager configManager) {
            this.configManager = configManager;
        }
        
        public void DrawUI() {
            if (showUI) {
                windowRect = GUI.Window(12345, windowRect, DrawUIWindow, "DrawLayer - Visual Markers");
            }
        }
        
        private void DrawUIWindow(int windowID) {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label("Visual Markers", GUI.skin.box);
            GUILayout.Space(10);
            
            // Bouton pour créer un nouveau repère
            if (GUILayout.Button("New Marker")) {
                isCreatingNewMarker = true;
                newMarker = new VisualMarker();
                newMarker.name = "New Marker";
            }
            
            GUILayout.Space(10);
            
            // Interface de création/édition
            if (isCreatingNewMarker) {
                DrawMarkerEditor(newMarker, true);
            }
            
            // Liste des repères existants
            GUILayout.Label("Existing markers:", GUI.skin.box);
            var markers = configManager.Markers;
            for (int i = 0; i < markers.Count; i++) {
                GUILayout.BeginHorizontal();
                
                markers[i].visible = GUILayout.Toggle(markers[i].visible, "");
                
                if (GUILayout.Button(markers[i].name, GUILayout.ExpandWidth(true))) {
                    selectedMarkerIndex = i;
                    isCreatingNewMarker = false;
                }
                
                if (GUILayout.Button("Del", GUILayout.Width(50))) {
                    configManager.RemoveMarker(i);
                    if (selectedMarkerIndex == i) {
                        selectedMarkerIndex = -1;
                    } else if (selectedMarkerIndex > i) {
                        selectedMarkerIndex--;
                    }
                    break;
                }
                
                GUILayout.EndHorizontal();
            }
            
            // Édition du repère sélectionné
            if (selectedMarkerIndex >= 0 && selectedMarkerIndex < markers.Count) {
                GUILayout.Space(10);
                GUILayout.Label("Edit:", GUI.skin.box);
                DrawMarkerEditor(markers[selectedMarkerIndex], false);
            }
            
            GUILayout.EndScrollView();
            
            // Bouton pour fermer l'interface
            if (GUILayout.Button("Close")) {
                showUI = false;
            }
            
            GUI.DragWindow();
        }
        
        private void DrawMarkerEditor(VisualMarker marker, bool isNew) {
            GUILayout.BeginVertical(GUI.skin.box);
            
            marker.name = GUILayout.TextField("Name:", marker.name);
            
            // Type de repère
            GUILayout.Label("Type:");
            marker.type = (MarkerType)GUILayout.SelectionGrid((int)marker.type, 
                new string[] { "Cross Lines", "Circle" }, 2);
            
            // Position
            GUILayout.Label("Position (% width, % height):");
            GUILayout.BeginHorizontal();
            marker.positionX = GUILayout.HorizontalSlider(marker.positionX, 0f, 100f);
            marker.positionY = GUILayout.HorizontalSlider(marker.positionY, 0f, 100f);
            GUILayout.EndHorizontal();
            GUILayout.Label($"X: {marker.positionX:F1}%, Y: {marker.positionY:F1}%");
            
            // Paramètres spécifiques au type
            if (marker.type == MarkerType.CrossLines) {
                // Pas de paramètres supplémentaires pour les lignes croisées
            } else if (marker.type == MarkerType.Circle) {
                GUILayout.Label("Radius (% width):");
                marker.radius = GUILayout.HorizontalSlider(marker.radius, 1f, 50f);
                GUILayout.Label($"Radius: {marker.radius:F1}%");
                
                marker.showGraduations = GUILayout.Toggle(marker.showGraduations, "Show graduations");
                
                if (marker.showGraduations) {
                    GUILayout.Label("Main graduation angle (degrees):");
                    marker.mainGraduationAngle = GUILayout.HorizontalSlider(marker.mainGraduationAngle, 0f, 360f);
                    GUILayout.Label($"Angle: {marker.mainGraduationAngle:F1}°");
                }
            }
            
            // Couleur
            GUILayout.Label("Color (R, G, B):");
            GUILayout.BeginHorizontal();
            marker.color.r = GUILayout.HorizontalSlider(marker.color.r, 0f, 1f);
            marker.color.g = GUILayout.HorizontalSlider(marker.color.g, 0f, 1f);
            marker.color.b = GUILayout.HorizontalSlider(marker.color.b, 0f, 1f);
            GUILayout.EndHorizontal();
            
            // Aperçu de la couleur
            Color originalColor = GUI.color;
            GUI.color = marker.color;
            GUILayout.Box("", GUILayout.Height(20));
            GUI.color = originalColor;
            
            // Boutons d'action
            GUILayout.BeginHorizontal();
            if (isNew) {
                if (GUILayout.Button("Create")) {
                    configManager.AddMarker(new VisualMarker(marker));
                    isCreatingNewMarker = false;
                }
                if (GUILayout.Button("Cancel")) {
                    isCreatingNewMarker = false;
                }
            } else {
                if (GUILayout.Button("Apply")) {
                    configManager.UpdateMarker(selectedMarkerIndex, marker);
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }
    }
} 