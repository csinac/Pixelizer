using UnityEditor;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CustomEditor(typeof(Colorizer))]
    public class ColorizerEditor : Editor
    {
        private Colorizer colorizer;

        public override void OnInspectorGUI()
        {
            if(colorizer == null)
                colorizer = (Colorizer)target;

            DrawDefaultInspector();

            if(GUILayout.Button("Colorize"))
            {
                colorizer.Colorize();
            }

            if(GUILayout.Button("Clear Color Collection"))
            {
                colorizer.ColorCollection.Colors.Clear();
            }

            if(GUILayout.Button("Complement Colors"))
            {
                colorizer.ComplementColors();
            }

            if(GUILayout.Button("Invert Colors"))
            {
                colorizer.InvertColors();
            }

            if(GUILayout.Button("Reset Colors"))
            {
                colorizer.ResetColors();
            }
        }
    }
}