using System.Collections.Generic;
using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class MarkerRenderer {
        private Material lineMaterial;
        
        public MarkerRenderer() {
            CreateLineMaterial();
        }
        
        private void CreateLineMaterial() {
            if (!lineMaterial) {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }
        
        public void DrawMarkers(List<VisualMarker> markers, VisualMarker previewMarker = null) {
            if (lineMaterial == null || markers == null) return;
            
            lineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            
            foreach (var marker in markers) {
                if (!marker.visible) continue;
                
                DrawMarker(marker);
            }
            
            // Dessiner le marqueur d'aperçu s'il existe
            if (previewMarker != null && previewMarker.visible) {
                DrawMarker(previewMarker);
            }
            
            GL.PopMatrix();
        }
        
        private void DrawMarker(VisualMarker marker) {
            Vector2 screenPos = new Vector2(
                Screen.width * marker.positionX / 100f,
                Screen.height * marker.positionY / 100f
            );
            
            if (marker.type == MarkerType.CrossLines) {
                var crossLinesMarker = new CrossLinesMarker(screenPos, marker.color);
                crossLinesMarker.Draw();
            } else if (marker.type == MarkerType.Circle) {
                float radius = Screen.width * marker.radius / 100f;
                var circleMarker = new CircleMarker(
                    screenPos, 
                    radius, 
                    marker.color, 
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