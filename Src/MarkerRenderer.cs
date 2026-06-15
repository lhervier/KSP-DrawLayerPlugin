using System.Collections.Generic;
using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class MarkerRenderer {
        private Material lineMaterial;
        
        public MarkerRenderer() {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            if( shader == null ) {
                Logger.LogError("Shader 'Hidden/Internal-Colored' not found");
                return;
            }

            lineMaterial = new Material(shader) {
                hideFlags = HideFlags.HideAndDontSave
            };
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
        
        public void DrawMarkers(
            List<VisualMarker> markers, 
            VisualMarker editingMarker = null
        ) {
            if (lineMaterial == null || markers == null) return;
            
            lineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            
            foreach (var marker in markers) {
                if (!marker.visible) continue;
                
                DrawMarker(marker);
            }
            
            // Dessiner le marqueur d'aperçu s'il existe
            if (editingMarker != null ) {
                DrawMarker(editingMarker);
            }
            
            GL.PopMatrix();
        }
        
        private void DrawMarker(VisualMarker marker) {
            Vector2 screenPos = new Vector2(
                Screen.width * marker.positionX / 100f,
                Screen.height * marker.positionY / 100f
            );
            
            if (marker.type == MarkerType.CrossLines) {
                var crossLinesMarker = new CrossLinesMarker(screenPos, marker.color.ToColor());
                crossLinesMarker.Draw();
            } else if (marker.type == MarkerType.Circle) {
                float radius = Screen.width * marker.radius / 100f;
                var circleMarker = new CircleMarker(
                    screenPos, 
                    radius, 
                    marker.color.ToColor(), 
                    marker.divisions
                );
                circleMarker.Draw();
            }
        }
        
        public void Dispose() {
            if (lineMaterial != null) {
                Object.DestroyImmediate(lineMaterial);
                lineMaterial = null;
            }
        }
    }
} 