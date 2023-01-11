using UnityEditor;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CustomEditor(typeof(Colorizer))]
    public class ColorizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Colorizer colorizer = (Colorizer)target;

            DrawDefaultInspector();

            if(GUILayout.Button("Colorize"))
            {
                colorizer.Colorize();
            }

            if(GUILayout.Button("Colorize With Brigtness"))
            {
                colorizer.ColorizeWithBrightness();
            }
        }
    }
}