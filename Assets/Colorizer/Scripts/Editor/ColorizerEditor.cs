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

            if(GUILayout.Button("Colorize With Brigtness"))
            {
                colorizer.ColorizeWithBrightness();
            }

            if(GUILayout.Button("Reset Colors"))
            {
                colorizer.ResetColors();
            }

            if(GUILayout.Button("Clear"))
            {
                colorizer.Colors.Clear();
            }
        }
    }
}