using System.Collections.Generic;
using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class UIManager {
        private readonly ConfigManager configManager;
        
        // Interface utilisateur - Fenêtre principale
        private bool showUI = false;
        private Rect mainWindowRect = new Rect(50, 50, 300, 200);
        
        // Interface utilisateur - Fenêtre d'édition
        private bool showEditorWindow = false;
        private Rect editorWindowRect = new Rect(400, 50, 400, 700);
        
        // État de l'interface
        private int selectedMarkerIndex = -1;
        private VisualMarker editingMarker = new VisualMarker();
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
                mainWindowRect = GUI.Window(12345, mainWindowRect, DrawMainWindow, "DrawLayer - Visual Markers");
                
                if (showEditorWindow) {
                    editorWindowRect = GUI.Window(12346, editorWindowRect, DrawEditorWindow, "Marker Editor");
                }
            }
        }
        
        private void DrawMainWindow(int windowID) {
            GUILayout.Label("Visual Markers", GUI.skin.box);
            GUILayout.Space(10);
            
            // Bouton pour créer un nouveau repère
            if (GUILayout.Button("New Marker")) {
                isCreatingNewMarker = true;
                editingMarker = new VisualMarker();
                editingMarker.name = "New Marker";
                showEditorWindow = true;
                // Créer un marqueur temporaire pour l'aperçu
                previewMarker = new VisualMarker(editingMarker);
            }
            
            GUILayout.Space(10);
            
            // Liste des repères existants
            GUILayout.Label("Existing markers:", GUI.skin.box);
            var markers = configManager.Markers;
            for (int i = 0; i < markers.Count; i++) {
                GUILayout.BeginHorizontal();
                
                markers[i].visible = GUILayout.Toggle(markers[i].visible, "");
                
                if (GUILayout.Button(markers[i].name, GUILayout.ExpandWidth(true))) {
                    selectedMarkerIndex = i;
                    isCreatingNewMarker = false;
                    editingMarker = new VisualMarker(markers[i]);
                    showEditorWindow = true;
                }
                
                if (GUILayout.Button("Del", GUILayout.Width(50))) {
                    configManager.RemoveMarker(i);
                    if (selectedMarkerIndex == i) {
                        selectedMarkerIndex = -1;
                        showEditorWindow = false;
                    } else if (selectedMarkerIndex > i) {
                        selectedMarkerIndex--;
                    }
                    break;
                }
                
                GUILayout.EndHorizontal();
            }
            
            GUI.DragWindow();
        }
        
        private void DrawEditorWindow(int windowID) {
            // Mettre à jour le marqueur d'aperçu en temps réel
            if (previewMarker != null) {
                previewMarker.name = editingMarker.name;
                previewMarker.type = editingMarker.type;
                previewMarker.positionX = editingMarker.positionX;
                previewMarker.positionY = editingMarker.positionY;
                previewMarker.radius = editingMarker.radius;
                previewMarker.mainGraduationDivisions = editingMarker.mainGraduationDivisions;
                previewMarker.color = editingMarker.color;
                previewMarker.visible = true;
            }
            
            DrawMarkerEditor(editingMarker, isCreatingNewMarker);
            
            GUI.DragWindow();
        }
        
        private void DrawMarkerEditor(VisualMarker marker, bool isNew) {
            GUILayout.BeginVertical(GUI.skin.box);
            
            // ===== PROPRIÉTÉS COMMUNES =====
            GUILayout.Label("Common Properties:", GUI.skin.box);
            
            // Nom
            GUILayout.Label("Name:");
            marker.name = GUILayout.TextField(marker.name, GUILayout.Width(200));
            
            // Position
            GUILayout.Label("Position (% width, % height):");
            GUILayout.BeginHorizontal();
            marker.positionX = GUILayout.HorizontalSlider(marker.positionX, 0f, 100f);
            marker.positionY = GUILayout.HorizontalSlider(marker.positionY, 0f, 100f);
            GUILayout.EndHorizontal();
            GUILayout.Label($"X: {marker.positionX:F1}%, Y: {marker.positionY:F1}%");
            
            // Bouton de réinitialisation de la position (commun aux deux types)
            if (GUILayout.Button("Reset Position to Center (50%, 50%)")) {
                marker.positionX = 50f;
                marker.positionY = 50f;
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
            
            GUILayout.Space(10);
            
            // ===== TYPE DE MARQUEUR =====
            GUILayout.Label("Marker Type:", GUI.skin.box);
            marker.type = (MarkerType)GUILayout.SelectionGrid((int)marker.type, 
                new string[] { "Cross Lines", "Circle" }, 2);
            
            GUILayout.Space(10);
            
            // ===== PROPRIÉTÉS SPÉCIFIQUES AU TYPE =====
            if (marker.type == MarkerType.CrossLines) {
                GUILayout.Label("Cross Lines Properties:", GUI.skin.box);
                GUILayout.Label("No additional properties for cross lines.");
            } else if (marker.type == MarkerType.Circle) {
                GUILayout.Label("Circle Properties:", GUI.skin.box);
                
                GUILayout.Label("Radius (% width):");
                marker.radius = GUILayout.HorizontalSlider(marker.radius, 1f, 50f);
                GUILayout.Label($"Radius: {marker.radius:F1}%");
                
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
            
            // Boutons d'action
            GUILayout.BeginHorizontal();
            if (isNew) {
                if (GUILayout.Button("Create")) {
                    configManager.AddMarker(new VisualMarker(marker));
                    isCreatingNewMarker = false;
                    showEditorWindow = false;
                    previewMarker = null; // Nettoyer l'aperçu
                }
                if (GUILayout.Button("Cancel")) {
                    isCreatingNewMarker = false;
                    showEditorWindow = false;
                    previewMarker = null; // Nettoyer l'aperçu
                }
            } else {
                if (GUILayout.Button("Apply")) {
                    // Créer une copie du marqueur modifié pour la sauvegarde
                    VisualMarker updatedMarker = new VisualMarker(marker);
                    configManager.UpdateMarker(selectedMarkerIndex, updatedMarker);
                }
                if (GUILayout.Button("Cancel")) {
                    showEditorWindow = false;
                    previewMarker = null; // Nettoyer l'aperçu
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