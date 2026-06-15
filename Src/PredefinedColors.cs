using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace com.github.lhervier.ksp {

    public class ColorAttribute : Attribute {
        public ColorAttribute(float r, float g, float b) {
            R = r;
            G = g;
            B = b;
        }

        public float R { get; }
        public float G { get; }
        public float B { get; }
    }

    public enum PredefinedColors {

        [Color(1f, 0f, 0f)]
        Red,
        [Color(0f, 1f, 0f)]
        Green,
        [Color(0f, 0f, 1f)]
        Blue,
        [Color(1f, 1f, 0f)]
        Yellow,
        [Color(0f, 1f, 1f)]
        Cyan,
        [Color(1f, 0f, 1f)]
        Magenta,
        [Color(1f, 1f, 1f)]
        White,
        [Color(0f, 0f, 0f)]
        Black,
        [Color(1f, 0.5f, 0f)]
        Orange,
        [Color(0.5f, 0f, 1f)]
        Purple,
        [Color(0f, 1f, 0.5f)]
        Teal,
        [Color(1f, 0f, 0.5f)]
        Pink,
        [Color(0.5f, 1f, 0f)]
        Lime,
        [Color(0f, 0.5f, 1f)]
        LightBlue,
        [Color(1f, 0.5f, 0.5f)]
        LightPink,
        [Color(0.5f, 0.5f, 0.5f)]
        Gray
    }

    public static class PredefinedColorsExtensions {

        public static Color ToColor(this PredefinedColors color) {
            var field = color.GetType().GetField(color.ToString());
            var attribute = field.GetCustomAttribute<ColorAttribute>();
            return new Color(attribute.R, attribute.G, attribute.B);
        }

        public static string GetName(this PredefinedColors color) {
            var field = color.GetType().GetField(color.ToString());
            return field.Name;
        }

        public static string GetDisplayName(this PredefinedColors color) {
            // Strart from the name of the property, and add a space before each uppercase character
            string name = color.GetName();
            string displayName = "";
            for( int i = 0; i < name.Length; i++ ) {
                if( char.IsUpper(name[i]) ) {
                    displayName += " " + name[i];
                } else {
                    displayName += name[i];
                }
            }
            return displayName;
        }
    }
}