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
        private VisualMarker previewMarker = null; // Marqueur temporaire pour l'aperçu
        
        public bool ShowUI {
            get => showUI;
            set => showUI = value;
        }
        
        public VisualMarker PreviewMarker => previewMarker;
        
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
                // Créer un marqueur temporaire pour l'aperçu
                previewMarker = new VisualMarker(newMarker);
            }
            
            GUILayout.Space(10);
            
            // Interface de création/édition
            if (isCreatingNewMarker) {
                DrawMarkerEditor(newMarker, true);
                // Mettre à jour le marqueur d'aperçu en temps réel
                if (previewMarker != null) {
                    previewMarker.name = newMarker.name;
                    previewMarker.type = newMarker.type;
                    previewMarker.positionX = newMarker.positionX;
                    previewMarker.positionY = newMarker.positionY;
                    previewMarker.radius = newMarker.radius;
                                         previewMarker.showGraduations = newMarker.showGraduations;
                     previewMarker.mainGraduationDivisions = newMarker.mainGraduationDivisions;
                    previewMarker.subGraduationDivisions = newMarker.subGraduationDivisions;
                    previewMarker.color = newMarker.color;
                    previewMarker.visible = true;
                }
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
            
            // Nettoyer le marqueur d'aperçu si on n'est plus en mode création
            if (!isCreatingNewMarker && previewMarker != null) {
                previewMarker = null;
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
            
            GUILayout.Label("Name:");
            marker.name = GUILayout.TextField(marker.name, GUILayout.Width(200));
            
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
            
            // Bouton de réinitialisation de la position pour les lignes croisées
            if (marker.type == MarkerType.CrossLines) {
                if (GUILayout.Button("Reset Position to Center (50%, 50%)")) {
                    marker.positionX = 50f;
                    marker.positionY = 50f;
                }
            }
            
            // Paramètres spécifiques au type
            if (marker.type == MarkerType.CrossLines) {
                // Pas de paramètres supplémentaires pour les lignes croisées
            } else if (marker.type == MarkerType.Circle) {
                GUILayout.Label("Radius (% width):");
                marker.radius = GUILayout.HorizontalSlider(marker.radius, 1f, 50f);
                GUILayout.Label($"Radius: {marker.radius:F1}%");
                
                marker.showGraduations = GUILayout.Toggle(marker.showGraduations, "Show graduations");
                
                if (marker.showGraduations) {
                    GUILayout.Label("Main graduation divisions:");
                    // Slider pour les divisions prédéfinies (1, 2, 3, 4, 6, 8, 12, 36)
                    int[] divisions = { 1, 2, 3, 4, 6, 8, 12, 36 };
                    int currentIndex = System.Array.IndexOf(divisions, marker.mainGraduationDivisions);
                    if (currentIndex == -1) currentIndex = 2; // Valeur par défaut (4 divisions)
                    
                    currentIndex = (int)GUILayout.HorizontalSlider(currentIndex, 0, divisions.Length - 1);
                    marker.mainGraduationDivisions = divisions[currentIndex];
                    
                                         // Afficher les degrés correspondants
                     float degrees = marker.mainGraduationDivisions > 1 ? 360f / marker.mainGraduationDivisions : 0f;
                     string divisionText = marker.mainGraduationDivisions == 1 ? "No graduations" : $"{marker.mainGraduationDivisions} divisions ({degrees:F0}°)";
                     GUILayout.Label($"Divisions: {divisionText}");
                }
            }
            
            // Couleur
            GUILayout.Label("Color:");
            
            // Palette de couleurs prédéfinies
            Color[] predefinedColors = {
                Color.red,           // Rouge
                Color.green,         // Vert
                Color.blue,          // Bleu
                Color.yellow,        // Jaune
                Color.cyan,          // Cyan
                Color.magenta,       // Magenta
                Color.white,         // Blanc
                Color.black,         // Noir
                new Color(1f, 0.5f, 0f),    // Orange
                new Color(0.5f, 0f, 1f),    // Violet
                new Color(0f, 1f, 0.5f),    // Vert-bleu
                new Color(1f, 0f, 0.5f),    // Rose
                new Color(0.5f, 1f, 0f),    // Vert clair
                new Color(0f, 0.5f, 1f),    // Bleu clair
                new Color(1f, 0.5f, 0.5f),  // Rose clair
                new Color(0.5f, 0.5f, 0.5f) // Gris
            };
            
            string[] colorNames = {
                "Red", "Green", "Blue", "Yellow", "Cyan", "Magenta", 
                "White", "Black", "Orange", "Purple", "Teal", "Pink",
                "Lime", "Light Blue", "Light Pink", "Gray"
            };
            
            // Sélection de couleur par boutons
            int selectedColorIndex = -1;
            for (int i = 0; i < predefinedColors.Length; i++) {
                if (IsColorSimilar(marker.color, predefinedColors[i])) {
                    selectedColorIndex = i;
                    break;
                }
            }
            
            // Grille de boutons de couleurs (4 colonnes)
            int newSelectedIndex = GUILayout.SelectionGrid(selectedColorIndex, colorNames, 4);
            if (newSelectedIndex != selectedColorIndex && newSelectedIndex >= 0) {
                marker.color = predefinedColors[newSelectedIndex];
            }
            
            // Aperçu de la couleur actuelle
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current color:");
            Color originalColor = GUI.color;
            GUI.color = marker.color;
            GUILayout.Box("", GUILayout.Height(20), GUILayout.Width(100));
            GUI.color = originalColor;
            GUILayout.EndHorizontal();
            
            // Boutons d'action
            GUILayout.BeginHorizontal();
            if (isNew) {
                if (GUILayout.Button("Create")) {
                    configManager.AddMarker(new VisualMarker(marker));
                    isCreatingNewMarker = false;
                    previewMarker = null; // Nettoyer l'aperçu
                }
                if (GUILayout.Button("Cancel")) {
                    isCreatingNewMarker = false;
                    previewMarker = null; // Nettoyer l'aperçu
                }
            } else {
                if (GUILayout.Button("Apply")) {
                    configManager.UpdateMarker(selectedMarkerIndex, marker);
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }
        
        // Méthode pour détecter si deux couleurs sont similaires
        private bool IsColorSimilar(Color color1, Color color2) {
            float tolerance = 0.1f; // Tolérance pour la comparaison
            return Mathf.Abs(color1.r - color2.r) < tolerance &&
                   Mathf.Abs(color1.g - color2.g) < tolerance &&
                   Mathf.Abs(color1.b - color2.b) < tolerance;
        }
    }
} 