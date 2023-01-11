using UnityEditor;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CustomEditor(typeof(Pix))]
    public class PixEditor : Editor
    {
        private Pix pix;

        public override void OnInspectorGUI()
        {
            if(pix == null)
                pix = (Pix)target;

            DrawDefaultInspector();

            if(GUILayout.Button("Reset Color"))
            {
                pix.ResetColor();
            }
        }
    }
}