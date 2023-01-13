using UnityEditor;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CustomEditor(typeof(Texturizer))]
    public class TexturizerEditor : Editor
    {
        private Texturizer texturizer;

        public override void OnInspectorGUI()
        {
            if(texturizer == null)
                texturizer = (Texturizer)target;

            DrawDefaultInspector();

            if(GUILayout.Button("Texturize"))
            {
                texturizer.Texturize(texturizer.PixSize);
            }
        }
    }
}

