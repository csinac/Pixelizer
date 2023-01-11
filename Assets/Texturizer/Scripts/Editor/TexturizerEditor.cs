using UnityEditor;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CustomEditor(typeof(Texturizer))]
    public class TexturizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Texturizer texturizer = (Texturizer)target;

            DrawDefaultInspector();

            if(GUILayout.Button("Save Texture"))
            {
                texturizer.SaveTexture(texturizer.PixSize);
            }
        }
    }
}

