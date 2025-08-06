using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class CircleMarker {
        private readonly Vector2 center;
        private readonly float radius;
        private readonly Color color;
        private readonly bool showGraduations;
        private readonly float mainGraduationAngle;
        private readonly int segments;
        
        public CircleMarker(Vector2 center, float radius, Color color, bool showGraduations, float mainGraduationAngle) {
            this.center = center;
            this.radius = radius;
            this.color = color;
            this.showGraduations = showGraduations;
            this.mainGraduationAngle = mainGraduationAngle;
            this.segments = 64;
        }
        
        public void Draw() {
            DrawCircle();
            
            if (showGraduations) {
                DrawGraduations();
            }
        }
        
        private void DrawCircle() {
            float angleStep = 360f / segments;
            
            // Dessiner le cercle
            for (int i = 0; i < segments; i++) {
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
        
        private void DrawGraduations() {
            // Graduation principale
            float mainAngleRad = mainGraduationAngle * Mathf.Deg2Rad;
            Vector2 mainPoint = center + new Vector2(
                Mathf.Cos(mainAngleRad) * radius,
                Mathf.Sin(mainAngleRad) * radius
            );
            Vector2 mainInnerPoint = center + new Vector2(
                Mathf.Cos(mainAngleRad) * (radius - 20),
                Mathf.Sin(mainAngleRad) * (radius - 20)
            );
            
            DrawLine(mainPoint, mainInnerPoint, color, 3);
            
            // Sous-graduations (tous les 30 degrés)
            for (int i = 0; i < 12; i++) {
                float angle = i * 30f * Mathf.Deg2Rad;
                if (Mathf.Abs(angle - mainAngleRad) < 0.1f) continue; // Éviter la graduation principale
                
                Vector2 point = center + new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );
                Vector2 innerPoint = center + new Vector2(
                    Mathf.Cos(angle) * (radius - 10),
                    Mathf.Sin(angle) * (radius - 10)
                );
                
                DrawLine(point, innerPoint, color, 1);
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
    }
} 