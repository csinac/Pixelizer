using UnityEditor;
using UnityEngine;

namespace AngryKoala.Pixel
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
        }
    }
}