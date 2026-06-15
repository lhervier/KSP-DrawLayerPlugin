using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class CrossLinesMarker {
        private readonly Vector2 center;
        private readonly Color color;
        
        public CrossLinesMarker(Vector2 center, Color color) {
            this.center = center;
            this.color = color;
        }
        
        public void Draw() {
            // Get the screen dimensions
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            // Horizontal line (from left to right of the screen)
            DrawLine(
                new Vector2(0, center.y),
                new Vector2(screenWidth, center.y),
                color
            );
            
            // Vertical line (from top to bottom of the screen)
            DrawLine(
                new Vector2(center.x, 0),
                new Vector2(center.x, screenHeight),
                color
            );
        }
        
        private void DrawLine(Vector2 start, Vector2 end, Color color) {
            GL.Begin(GL.LINES);
            GL.Color(color);
            
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            
            GL.End();
        }
    }
} 