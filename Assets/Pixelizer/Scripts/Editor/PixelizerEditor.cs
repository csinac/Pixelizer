using UnityEditor;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CustomEditor(typeof(Pixelizer))]
    public class PixelizerEditor : Editor
    {
        private Pixelizer pixelizer;

        public override void OnInspectorGUI()
        {
            if(pixelizer == null)
                pixelizer = (Pixelizer)target;

            DrawDefaultInspector();

            if(GUILayout.Button("Pixelize"))
            {
                if(pixelizer.Texture == null)
                {
                    Debug.LogWarning("No texture found to pixelize");
                    return;
                }

                if(!pixelizer.Texture.isReadable)
                {
                    SetTextureReadability(pixelizer.Texture);
                }

                pixelizer.Pixelize();
            }

            if(GUILayout.Button("Clear"))
            {
                pixelizer.Clear();
            }
        }

        private void SetTextureReadability(Texture2D texture)
        {
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));

            importer.textureType = TextureImporterType.Default;
            importer.isReadable = true;

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
    }
}