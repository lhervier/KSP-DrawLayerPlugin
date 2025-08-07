using System.Collections.Generic;
using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class UIManager {
        private readonly ConfigManager configManager;
        
        // UI - Main window
        private bool showUI = false;
        private Rect mainWindowRect = new Rect(50, 50, 300, 200);
        
        // UI - Editor window
        private bool showEditorWindow = false;
        private Rect editorWindowRect = new Rect(400, 50, 400, 700);
        
        // UI - State
        private int editingMarkerIndex = -1;
        private VisualMarker editingMarker = null;
        private bool isCreatingNewMarker = false;
        
        public bool ShowUI {
            get => showUI;
            set => showUI = value;
        }

        public VisualMarker EditingMarker => editingMarker;
        public int EditingMarkerIndex => editingMarkerIndex;
        
        public UIManager(ConfigManager configManager) {
            this.configManager = configManager;
        }
        
        public void DrawUI() {
            if (!showUI) return;
            mainWindowRect = GUI.Window(12345, mainWindowRect, DrawMainWindow, "DrawLayer - Visual Markers");
            
            if (showEditorWindow) {
                editorWindowRect = GUI.Window(12346, editorWindowRect, DrawEditorWindow, "Marker Editor");
            }
        }
        
        private void DrawMainWindow(int windowID) {
            GUILayout.Label("Visual Markers", GUI.skin.box);
            GUILayout.Space(10);
            
            // Button to create a new marker
            if (GUILayout.Button("Create Marker")) {
                isCreatingNewMarker = true;
                editingMarkerIndex = -1;
                editingMarker = new VisualMarker();
                showEditorWindow = true;
            }
            
            GUILayout.Space(10);
            
            // List of existing markers
            GUILayout.Label("Existing markers:", GUI.skin.box);
            var markers = configManager.Markers;
            for (int i = 0; i < markers.Count; i++) {
                GUILayout.BeginHorizontal();
                
                markers[i].visible = GUILayout.Toggle(markers[i].visible, "");
                
                if (GUILayout.Button($"Edit {markers[i].name}", GUILayout.ExpandWidth(true))) {
                    editingMarkerIndex = i;
                    isCreatingNewMarker = false;
                    editingMarker = new VisualMarker(markers[i]);
                    showEditorWindow = true;
                }
                
                if (GUILayout.Button("Del", GUILayout.Width(50))) {
                    configManager.RemoveMarker(i);
                    if (editingMarkerIndex == i) {
                        editingMarkerIndex = -1;
                        editingMarker = null;
                        showEditorWindow = false;
                    } else if (editingMarkerIndex > i) {
                        editingMarkerIndex--;
                    }
                    break;
                }
                
                GUILayout.EndHorizontal();
            }
            
            GUI.DragWindow();
        }
        
        private void DrawEditorWindow(int windowID) {
            GUILayout.BeginVertical(GUI.skin.box);
            
            // ===== COMMON PROPERTIES =====
            GUILayout.Label("Common Properties:", GUI.skin.box);
            
            // Nom
            GUILayout.Label("Name:");
            editingMarker.name = GUILayout.TextField(editingMarker.name, GUILayout.Width(200));
            
            // Position
            GUILayout.Label("Position (% width, % height):");
            GUILayout.BeginHorizontal();
            editingMarker.positionX = GUILayout.HorizontalSlider(editingMarker.positionX, 0f, 100f);
            editingMarker.positionY = GUILayout.HorizontalSlider(editingMarker.positionY, 0f, 100f);
            GUILayout.EndHorizontal();
            GUILayout.Label($"X: {editingMarker.positionX:F1}%, Y: {editingMarker.positionY:F1}%");
            
            // Button to reset the position (common to both types)
            if (GUILayout.Button("Reset Position to Center (50%, 50%)")) {
                editingMarker.positionX = 50f;
                editingMarker.positionY = 50f;
            }
            
            // Color
            GUILayout.Label("Color:");
            
            // Predefined color palette
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
            
            // Color selection by buttons
            int selectedColorIndex = -1;
            for (int i = 0; i < predefinedColors.Length; i++) {
                if (IsColorSimilar(editingMarker.color, predefinedColors[i])) {
                    selectedColorIndex = i;
                    break;
                }
            }
            
            // Color grid of buttons (4 columns)
            int newSelectedIndex = GUILayout.SelectionGrid(selectedColorIndex, colorNames, 4);
            if (newSelectedIndex != selectedColorIndex && newSelectedIndex >= 0) {
                editingMarker.color = predefinedColors[newSelectedIndex];
            }
            
            // Current color preview
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current color:");
            Color originalColor = GUI.color;
            GUI.color = editingMarker.color;
            GUILayout.Box("", GUILayout.Height(20), GUILayout.Width(100));
            GUI.color = originalColor;
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            // ===== MARKER TYPE =====
            GUILayout.Label("Marker Type:", GUI.skin.box);
            editingMarker.type = (MarkerType)GUILayout.SelectionGrid((int)editingMarker.type, 
                new string[] { "Cross Lines", "Circle" }, 2);
            
            GUILayout.Space(10);
            
            // ===== SPECIFIC PROPERTIES FOR TYPE =====
            if (editingMarker.type == MarkerType.CrossLines) {
                GUILayout.Label("Cross Lines Properties:", GUI.skin.box);
                GUILayout.Label("No additional properties for cross lines.");
            } else if (editingMarker.type == MarkerType.Circle) {
                GUILayout.Label("Circle Properties:", GUI.skin.box);
                
                GUILayout.Label("Radius (% width):");
                editingMarker.radius = GUILayout.HorizontalSlider(editingMarker.radius, 1f, 50f);
                GUILayout.Label($"Radius: {editingMarker.radius:F1}%");
                
                GUILayout.Label("Main graduation divisions:");
                // Slider for predefined divisions (1, 2, 3, 4, 6, 8, 12, 36)
                int[] divisions = { 1, 2, 3, 4, 6, 8, 12, 36 };
                int currentIndex = System.Array.IndexOf(divisions, editingMarker.divisions);
                if (currentIndex == -1) currentIndex = 2; // Default value (4 divisions)
                
                currentIndex = (int)GUILayout.HorizontalSlider(currentIndex, 0, divisions.Length - 1);
                editingMarker.divisions = divisions[currentIndex];
                
                // Display the corresponding degrees
                float degrees = editingMarker.divisions > 1 ? 360f / editingMarker.divisions : 0f;
                string divisionText = editingMarker.divisions == 1 ? "No graduations" : $"{editingMarker.divisions} divisions ({degrees:F0}°)";
                GUILayout.Label($"Divisions: {divisionText}");
            }
            
            // Action buttons
            GUILayout.BeginHorizontal();
            if (isCreatingNewMarker) {
                if (GUILayout.Button("Create")) {
                    configManager.AddMarker(editingMarker);
                    editingMarker = null;
                    isCreatingNewMarker = false;
                    showEditorWindow = false;
                    editingMarkerIndex = -1;
                }
                if (GUILayout.Button("Cancel")) {
                    editingMarker = null;
                    isCreatingNewMarker = false;
                    showEditorWindow = false;
                    editingMarkerIndex = -1;
                }
            } else {
                if (GUILayout.Button("Update")) {
                    configManager.UpdateMarker(editingMarkerIndex, editingMarker);
                    editingMarker = null;
                    isCreatingNewMarker = false;
                    showEditorWindow = false;
                    editingMarkerIndex = -1;
                }
                if (GUILayout.Button("Cancel")) {
                    editingMarker = null;
                    isCreatingNewMarker = false;
                    showEditorWindow = false;
                    editingMarkerIndex = -1;
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();

            GUI.DragWindow();
        }
        

        
        // Method to detect if two colors are similar
        private bool IsColorSimilar(Color color1, Color color2) {
            float tolerance = 0.1f; // Tolerance for comparison
            return Mathf.Abs(color1.r - color2.r) < tolerance &&
                   Mathf.Abs(color1.g - color2.g) < tolerance &&
                   Mathf.Abs(color1.b - color2.b) < tolerance;
        }
    }
} 