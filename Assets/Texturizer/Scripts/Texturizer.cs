using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AngryKoala.Pixelization
{
    public class Texturizer : MonoBehaviour
    {
        [SerializeField] private Pixelizer pixelizer;

        [SerializeField] private int pixSize;
        public int PixSize => pixSize;

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

        public void SaveTexture(int pixSize)
        {
            Texture2D newTexture = new Texture2D(pixelizer.Width * pixSize, pixelizer.Height * pixSize, TextureFormat.RGBA32, false);

            int pixIndex = 0;

            for(int i = 0; i < pixelizer.Width; i++)
            {
                for(int j = 0; j < pixelizer.Height; j++)
                {
                    Color[] pixColor = new Color[pixSize * pixSize];

                    for(int k = 0; k < pixColor.Length; k++)
                    {
                        pixColor[k] = pixelizer.PixCollection[pixIndex].Color;
                    }

                    newTexture.SetPixels(i * pixSize, j * pixSize, pixSize, pixSize, pixColor);
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