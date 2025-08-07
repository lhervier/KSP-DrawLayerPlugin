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
        public float positionX = 50f; // % de la largeur
        public float positionY = 50f; // % de la hauteur
        public float radius = 10f; // % de la largeur (pour les cercles)
        public bool showGraduations = false;
        public int mainGraduationDivisions = 12; // nombre de divisions principales (2, 4, 8, 12, 36)
        public int subGraduationDivisions = 3; // nombre de divisions pour les sous-graduations
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
            this.showGraduations = other.showGraduations;
            this.mainGraduationDivisions = other.mainGraduationDivisions;
            this.subGraduationDivisions = other.subGraduationDivisions;
            this.color = other.color;
            this.visible = other.visible;
        }
    }
}