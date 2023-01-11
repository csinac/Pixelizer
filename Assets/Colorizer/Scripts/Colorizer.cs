using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    public class Colorizer : MonoBehaviour
    {
        [SerializeField] private Pixelizer pixelizer;

        [SerializeField] private List<Color> colors;

        public void Colorize()
        {
            for(int i = 0; i < pixelizer.PixCollection.Length; i++)
            {
                pixelizer.PixCollection[i].SetColor(GetClosestColorizerColor(pixelizer.PixCollection[i].Color));
            }
        }

        public void ColorizeWithBrightness()
        {
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

            foreach(var colorizerColor in colors)
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
    }
}