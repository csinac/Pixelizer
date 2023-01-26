using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CustomEditor(typeof(Texturizer))]
    public class TexturizerEditor : NaughtyInspector
    {
        private Texturizer texturizer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(texturizer == null)
                texturizer = (Texturizer)target;

            if(GUILayout.Button("Texturize"))
            {
                texturizer.Texturize(true);
            }
        }
    }
}

