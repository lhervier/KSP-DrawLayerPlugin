using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.github.lhervier.ksp {
    
    [Serializable]
    public class VisualMarker {
        public string name = "Marker";
        public MarkerType type = MarkerType.CrossLines;
        public float positionX = 50f; // % of the width
        public float positionY = 50f; // % of the height
        public float radius = 10f; // % of the width (for circles)
        public int mainGraduationDivisions = 12; // main graduation divisions (2, 4, 8, 12, 36)
        public Color color = Color.white;
        public bool visible = true;
        
        public VisualMarker() {
        }
        
        public VisualMarker(VisualMarker other) {
            this.name = other.name;
            this.type = other.type;
            this.positionX = other.positionX;
            this.positionY = other.positionY;
            this.radius = other.radius;
            this.mainGraduationDivisions = other.mainGraduationDivisions;
            this.color = other.color;
            this.visible = other.visible;
        }
    }
}