using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Pix))]
    public class PixEditor : NaughtyInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(GUILayout.Button("Complement Color"))
            {
                foreach(var item in Selection.gameObjects)
                {
                    if(item.TryGetComponent(out Pix pix))
                    {
                        pix.ComplementColor();
                    }
                }
            }

            if(GUILayout.Button("Invert Color"))
            {
                foreach(var item in Selection.gameObjects)
                {
                    if(item.TryGetComponent(out Pix pix))
                    {
                        pix.InvertColor();
                    }
                }
            }

            if(GUILayout.Button("Reset Color"))
            {
                foreach(var item in Selection.gameObjects)
                {
                    if(item.TryGetComponent(out Pix pix))
                    {
                        pix.ResetColor();
                    }
                }
            }
        }
    }
}