using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class CrossLinesMarker {
        private readonly Vector2 center;
        private readonly Color color;
        private readonly int thickness;
        
        public CrossLinesMarker(Vector2 center, Color color) {
            this.center = center;
            this.color = color;
            this.thickness = 2;
        }
        
        public void Draw() {
            // Obtenir les dimensions de l'écran
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            // Ligne horizontale (de gauche à droite de l'écran)
            DrawLine(
                new Vector2(0, center.y),
                new Vector2(screenWidth, center.y),
                color, thickness
            );
            
            // Ligne verticale (de haut en bas de l'écran)
            DrawLine(
                new Vector2(center.x, 0),
                new Vector2(center.x, screenHeight),
                color, thickness
            );
        }
        
        private void DrawLine(Vector2 start, Vector2 end, Color color, int thickness) {
            GL.Begin(GL.LINES);
            GL.Color(color);
            
            for (int i = 0; i < thickness; i++) {
                GL.Vertex3(start.x + i, start.y, 0);
                GL.Vertex3(end.x + i, end.y, 0);
            }
            
            GL.End();
        }
    }
} 