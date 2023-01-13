using UnityEngine;

namespace AngryKoala.Pixelization
{
    public class Colorizer : MonoBehaviour
    {
        [SerializeField] private Pixelizer pixelizer;

        [SerializeField] private ColorCollection colorCollection;
        public ColorCollection ColorCollection => colorCollection;

        public void Colorize()
        {
            if(pixelizer.PixCollection.Length == 0)
            {
                Debug.LogWarning("Pixelize a texture first");
                return;
            }

            if(colorCollection.Colors.Count == 0)
            {
                Debug.LogWarning("No colors selected");
                return;
            }

            for(int i = 0; i < pixelizer.PixCollection.Length; i++)
            {
                pixelizer.PixCollection[i].SetColor(GetClosestColorizerColor(pixelizer.PixCollection[i].Color));
            }
        }

        public void ColorizeWithBrightness()
        {
            if(pixelizer.PixCollection.Length == 0)
            {
                Debug.LogWarning("Pixelize a texture first");
                return;
            }

            if(colorCollection.Colors.Count == 0)
            {
                Debug.LogWarning("No colors selected");
                return;
            }

            for(int i = 0; i < pixelizer.PixCollection.Length; i++)
            {
                Color originalColor = pixelizer.PixCollection[i].Color;
                Color adjustedColor = GetClosestColorizerColor(originalColor);

                float colorBrightness = (originalColor.r + originalColor.g + originalColor.b) / 3f;

                pixelizer.PixCollection[i].SetColor(adjustedColor * colorBrightness);
            }
        }

        private Color GetClosestColorizerColor(Color color)
        {
            float colorDifference = Mathf.Infinity;

            Color closestColor = Color.white;

            foreach(var colorizerColor in colorCollection.Colors)
            {
                Vector3 colorValues = new Vector3(color.r, color.g, color.b);
                Vector3 colorizerColorValues = new Vector3(colorizerColor.r, colorizerColor.g, colorizerColor.b);

                float difference = Vector3.Distance(colorValues, colorizerColorValues);

                if(difference < colorDifference)
                {
                    closestColor = colorizerColor;
                    colorDifference = difference;
                }
            }

            return closestColor;
        }

        public void ComplementColors()
        {
            foreach(var pix in pixelizer.PixCollection)
            {
                pix.ComplementColor();
            }
        }

        public void InvertColors()
        {
            foreach(var pix in pixelizer.PixCollection)
            {
                pix.InvertColor();
            }
        }

        public void ResetColors()
        {
            foreach(var pix in pixelizer.PixCollection)
            {
                pix.ResetColor();
            }
        }
    }
}