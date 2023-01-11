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
                if(pixelizer.Texture == null)
                {
                    Debug.LogError("No texture found to pixelize");
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