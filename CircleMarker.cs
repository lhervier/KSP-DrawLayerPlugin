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
                
                DrawLine(point1, point2, color, 2);
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
                
                DrawDottedLine(center, outerPoint, color, 1);
            }
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
        
        private void DrawDottedLine(Vector2 start, Vector2 end, Color color, int thickness) {
            // Paramètres pour les pointillés
            float dashLength = 4f; // Longueur de chaque tiret
            float gapLength = 4f;  // Espace entre les tirets
            
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
                    
                    for (int i = 0; i < thickness; i++) {
                        GL.Vertex3(segmentStart.x + i, segmentStart.y, 0);
                        GL.Vertex3(segmentEnd.x + i, segmentEnd.y, 0);
                    }
                }
                
                currentLength += segmentLength;
                drawDash = !drawDash;
            }
            
            GL.End();
        }
    }
} 