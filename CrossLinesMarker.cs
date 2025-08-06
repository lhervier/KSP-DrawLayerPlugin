using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class CrossLinesMarker {
        private readonly Vector2 center;
        private readonly Color color;
        private readonly int lineLength;
        private readonly int thickness;
        
        public CrossLinesMarker(Vector2 center, Color color) {
            this.center = center;
            this.color = color;
            this.lineLength = 50;
            this.thickness = 2;
        }
        
        public void Draw() {
            // Ligne horizontale
            DrawLine(
                new Vector2(center.x - lineLength, center.y),
                new Vector2(center.x + lineLength, center.y),
                color, thickness
            );
            
            // Ligne verticale
            DrawLine(
                new Vector2(center.x, center.y - lineLength),
                new Vector2(center.x, center.y + lineLength),
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