using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    public class Colorizer : MonoBehaviour
    {
        [SerializeField] private Pixelizer pixelizer;

        [SerializeField] private ColorPalette colorCollection;
        public ColorPalette ColorCollection => colorCollection;

        private enum ColorizationStyle { Replace, ReplaceWithOriginalBrightness }
        [SerializeField] private ColorizationStyle colorizationStyle;

        [SerializeField] private int extractColorPaletteColorCount;

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
                if(colorizationStyle == ColorizationStyle.Replace)
                {
                    pixelizer.PixCollection[i].SetColor(GetClosestColorizerColor(pixelizer.PixCollection[i].Color));
                }
                if(colorizationStyle == ColorizationStyle.ReplaceWithOriginalBrightness)
                {
                    Color originalColor = pixelizer.PixCollection[i].Color;
                    Color adjustedColor = GetClosestColorizerColor(originalColor);

                    float colorBrightness = (originalColor.r + originalColor.g + originalColor.b) / 3f;

                    pixelizer.PixCollection[i].SetColor(adjustedColor * colorBrightness);
                }
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

        public void ExtractColorPalette()
        {
            int iterationCount = 10;

            List<Color> pixels = new List<Color>();
            foreach(var pix in pixelizer.PixCollection)
            {
                pixels.Add(pix.Color);
            }
            int pixelCount = pixels.Count;

            Color[] centroids = new Color[extractColorPaletteColorCount];
            for(int i = 0; i < extractColorPaletteColorCount; i++)
            {
                centroids[i] = pixels[Random.Range(0, pixelCount)];
            }

            for(int i = 0; i < iterationCount; i++)
            {
                int[] nearestCentroidIndices = new int[pixelCount];

                for(int j = 0; j < pixelCount; j++)
                {
                    float nearestDistance = float.MaxValue;
                    int nearestCentroidIndex = 0;
                    for(int k = 0; k < extractColorPaletteColorCount; k++)
                    {
                        Vector3 colorA = new Vector3(pixels[j].r, pixels[j].g, pixels[j].b);
                        Vector3 colorB = new Vector3(centroids[k].r, centroids[k].g, centroids[k].b);

                        float distance = Vector3.Distance(colorA, colorB);
                        if(distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestCentroidIndex = k;
                        }
                    }
                    nearestCentroidIndices[j] = nearestCentroidIndex;
                }

                for(int j = 0; j < extractColorPaletteColorCount; j++)
                {
                    var pixelsInCluster =
                        from pixelIndex in Enumerable.Range(0, pixelCount)
                        where nearestCentroidIndices[pixelIndex] == j
                        select pixels[pixelIndex];

                    if(pixelsInCluster.Any())
                    {
                        centroids[j] = new Color(pixelsInCluster.Average(c => c.r), pixelsInCluster.Average(c => c.g), pixelsInCluster.Average(c => c.b));
                    }
                }
            }

            colorCollection.Colors.Clear();

            foreach(var centroid in centroids)
            {
                colorCollection.Colors.Add(centroid);
            }
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