using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AngryKoala.Pixelization
{
    public class Colorizer : MonoBehaviour
    {
        [SerializeField] private Pixelizer pixelizer;
        [SerializeField] private Texturizer texturizer;

        [SerializeField] private ColorPalette colorCollection;
        public ColorPalette ColorCollection => colorCollection;

        private enum ColorizationStyle { Replace, ReplaceWithOriginalBrightness }
        [SerializeField] private ColorizationStyle colorizationStyle;

        private enum ReplacementStyle { ReplaceUsingColor, ReplaceUsingBrightness }
        [SerializeField] private ReplacementStyle replacementStyle;

        [SerializeField] private int extractColorPaletteColorCount;

        public static UnityAction OnColorize;

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

                    float colorBrightness = originalColor.maxColorComponent;

                    float hue;
                    float saturation;
                    float brightness;

                    Color.RGBToHSV(adjustedColor, out hue, out saturation, out brightness);
                    adjustedColor = Color.HSVToRGB(hue, saturation, colorBrightness);

                    pixelizer.PixCollection[i].SetColor(adjustedColor);
                }
            }

            texturizer.Texturize();
            OnColorize?.Invoke();
        }

        private Color GetClosestColorizerColor(Color color)
        {
            float colorDifference = Mathf.Infinity;

            Color closestColor = Color.white;

            if(replacementStyle == ReplacementStyle.ReplaceUsingColor)
            {
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
            }
            if(replacementStyle == ReplacementStyle.ReplaceUsingBrightness)
            {
                foreach(var colorizerColor in colorCollection.Colors)
                {
                    float colorBrightness = color.maxColorComponent;
                    float colorizerColorBrightness = colorizerColor.maxColorComponent;

                    float difference = Mathf.Abs(colorBrightness - colorizerColorBrightness);

                    if(difference < colorDifference)
                    {
                        closestColor = colorizerColor;
                        colorDifference = difference;
                    }
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

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(colorCollection);
#endif
        }

        public void ComplementColors()
        {
            foreach(var pix in pixelizer.PixCollection)
            {
                pix.ComplementColor();
            }

            texturizer.Texturize();
            OnColorize?.Invoke();
        }

        public void InvertColors()
        {
            foreach(var pix in pixelizer.PixCollection)
            {
                pix.InvertColor();
            }

            texturizer.Texturize();
            OnColorize?.Invoke();
        }

        public void ResetColors()
        {
            foreach(var pix in pixelizer.PixCollection)
            {
                pix.ResetColor();
            }

            texturizer.Texturize();
            OnColorize?.Invoke();
        }
    }
}