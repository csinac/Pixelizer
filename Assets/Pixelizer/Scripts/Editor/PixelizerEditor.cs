using UnityEditor;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CustomEditor(typeof(Pixelizer))]
    public class PixelizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Pixelizer pixelizer = (Pixelizer)target;

            DrawDefaultInspector();

            if(GUILayout.Button("Pixelize"))
            {
                pixelizer.Pixelize();
            }

            if(GUILayout.Button("Clear"))
            {
                pixelizer.Clear();
            }
        }
    }
}