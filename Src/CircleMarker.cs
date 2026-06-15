using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class CircleMarker {
        private static readonly int SEGMENTS = 64;
        private readonly Vector2 center;
        private readonly float radius;
        private readonly Color color;
        private readonly int divisions;
        
        public CircleMarker(
            Vector2 center, 
            float radius, 
            Color color, 
            int divisions
        ) {
            this.center = center;
            this.radius = radius;
            this.color = color;
            this.divisions = divisions;
        }
        
        public void Draw() {
            DrawCircle();
            DrawRadialLines();
        }
        
        private void DrawCircle() {
            float angleStep = 360f / SEGMENTS;
            
            // Draw the circle
            for (int i = 0; i < SEGMENTS; i++) {
                float angle1 = i * angleStep * Mathf.Deg2Rad;
                float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
                
                Vector2 point1 = center + new Vector2(
                    Mathf.Cos(angle1) * radius,
                    Mathf.Sin(angle1) * radius
                );
                Vector2 point2 = center + new Vector2(
                    Mathf.Cos(angle2) * radius,
                    Mathf.Sin(angle2) * radius
                );
                
                DrawLine(point1, point2, color);
            }
        }
        
        private void DrawRadialLines() {
            float mainGraduationSize = 360f / divisions;
            
            // Draw the radial lines in dotted for each main graduation
            for (int i = 0; i < divisions; i++) {
                float angle = i * mainGraduationSize * Mathf.Deg2Rad;
                
                Vector2 outerPoint = center + new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );
                
                DrawDottedLine(center, outerPoint, color);
            }
        }
        
        private void DrawLine(Vector2 start, Vector2 end, Color color) {
            GL.Begin(GL.LINES);
            GL.Color(color);
            
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            
            GL.End();
        }
        
        private void DrawDottedLine(Vector2 start, Vector2 end, Color color) {
            // Parameters for the dotted lines
            float dashLength = 4f; // Length of each dash
            float gapLength = 4f;  // Space between the dashes
            
            Vector2 direction = (end - start).normalized;
            float totalLength = Vector2.Distance(start, end);
            float currentLength = 0f;
            bool drawDash = true;
            
            GL.Begin(GL.LINES);
            GL.Color(color);
            
            while (currentLength < totalLength) {
                float segmentLength = drawDash ? dashLength : gapLength;
                float remainingLength = totalLength - currentLength;
                
                if (segmentLength > remainingLength) {
                    segmentLength = remainingLength;
                }
                
                if (drawDash) {
                    Vector2 segmentStart = start + direction * currentLength;
                    Vector2 segmentEnd = start + direction * (currentLength + segmentLength);
                    
                    GL.Vertex3(segmentStart.x, segmentStart.y, 0);
                    GL.Vertex3(segmentEnd.x, segmentEnd.y, 0);
                }
                
                currentLength += segmentLength;
                drawDash = !drawDash;
            }
            
            GL.End();
        }
    }
} 