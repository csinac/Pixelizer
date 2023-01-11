using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AngryKoala.Pixel
{
    public class Texturizer : MonoBehaviour
    {
        [SerializeField] private Pixelizer pixelizer;

        public void SaveTexture()
        {
            Texture2D newTexture = new Texture2D(pixelizer.Width, pixelizer.Height, TextureFormat.RGBA32, false);

            int pixIndex = 0;

            for(int i = 0; i < pixelizer.Width; i++)
            {
                for(int j = 0; j < pixelizer.Height; j++)
                {
                    newTexture.SetPixel(i, j, pixelizer.PixCollection[pixIndex].Color);
                    pixIndex++;
                }
            }

#if UNITY_EDITOR
            if(!AssetDatabase.IsValidFolder("Assets/Texturizer/Textures"))
            {
                AssetDatabase.CreateFolder("Assets/Texturizer", "Textures");
            }

            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Texturizer/Textures/Texture.png");

            byte[] bytes = newTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);


            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
#endif
        }
    }
}