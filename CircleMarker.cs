using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public class CircleMarker {
        private readonly Vector2 center;
        private readonly float radius;
        private readonly Color color;
        private readonly int mainGraduationDivisions;
        private readonly int segments;
        
        public CircleMarker(
            Vector2 center, 
            float radius, 
            Color color, 
            int mainGraduationDivisions
        ) {
            this.center = center;
            this.radius = radius;
            this.color = color;
            this.mainGraduationDivisions = mainGraduationDivisions;
            this.segments = 64;
        }
        
        public void Draw() {
            DrawCircle();
            DrawCenter();
            
            DrawGraduations();
            DrawRadialLines();
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
        
        private void DrawCenter() {
            // Taille de la croix du centre (en pixels)
            float crossSize = 8f;
            
            // Ligne horizontale de la croix
            Vector2 leftPoint = center + new Vector2(-crossSize, 0);
            Vector2 rightPoint = center + new Vector2(crossSize, 0);
            DrawLine(leftPoint, rightPoint, color, 2);
            
            // Ligne verticale de la croix
            Vector2 topPoint = center + new Vector2(0, crossSize);
            Vector2 bottomPoint = center + new Vector2(0, -crossSize);
            DrawLine(topPoint, bottomPoint, color, 2);
        }
        
        private void DrawRadialLines() {
            // Ne dessiner les rayons que s'il y a des divisions
            if (mainGraduationDivisions <= 1) return;
            
            float mainGraduationSize = 360f / mainGraduationDivisions;
            
            // Dessiner les rayons en pointillés pour chaque graduation principale
            for (int i = 0; i < mainGraduationDivisions; i++) {
                float angle = i * mainGraduationSize * Mathf.Deg2Rad;
                
                Vector2 outerPoint = center + new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );
                
                DrawDottedLine(center, outerPoint, color, 1);
            }
        }
        
        private void DrawGraduations() {
            // Ne dessiner les graduations que s'il y a des divisions
            if (mainGraduationDivisions <= 1) return;
            
            float mainGraduationSize = 360f / mainGraduationDivisions;
            
            // Dessiner toutes les graduations principales
            for (int i = 0; i < mainGraduationDivisions; i++) {
                float angle = i * mainGraduationSize * Mathf.Deg2Rad;
                
                // Taille de la graduation (toutes les graduations principales ont la même taille)
                float graduationLength = 20f;
                int thickness = 2;
                
                Vector2 outerPoint = center + new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );
                Vector2 innerPoint = center + new Vector2(
                    Mathf.Cos(angle) * (radius - graduationLength),
                    Mathf.Sin(angle) * (radius - graduationLength)
                );
                
                DrawLine(outerPoint, innerPoint, color, thickness);
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