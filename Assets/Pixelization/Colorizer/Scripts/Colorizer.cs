using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AngryKoala.Pixelization
{
    public class Colorizer : MonoBehaviour
    {
        [SerializeField] private Pixelizer pixelizer;

        [SerializeField] private ColorPalette colorPalette;
        public ColorPalette ColorPalette => colorPalette;

        private enum ColorizationStyle { Replace, ReplaceWithOriginalSaturation, ReplaceWithOriginalValue }
        [SerializeField] private ColorizationStyle colorizationStyle;

        private enum ReplacementStyle { ReplaceUsingHue, ReplaceUsingSaturation, ReplaceUsingValue }
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

            if(colorPalette.Colors.Count == 0)
            {
                Debug.LogWarning("No colors selected");
                return;
            }

            for(int i = 0; i < pixelizer.PixCollection.Length; i++)
            {
                switch(colorizationStyle)
                {
                    case ColorizationStyle.Replace:
                        pixelizer.PixCollection[i].SetColor(GetClosestColorizerColor(pixelizer.PixCollection[i].Color));
                        break;

                    case ColorizationStyle.ReplaceWithOriginalSaturation:
                        {
                            Color originalColor = pixelizer.PixCollection[i].Color;
                            Color adjustedColor = GetClosestColorizerColor(originalColor);

                            float originalHue, originalSaturation, originalValue;
                            float hue, saturation, value;

                            Color.RGBToHSV(originalColor, out originalHue, out originalSaturation, out originalValue);
                            Color.RGBToHSV(adjustedColor, out hue, out saturation, out value);

                            adjustedColor = Color.HSVToRGB(hue, originalSaturation, value);

                            pixelizer.PixCollection[i].SetColor(adjustedColor);
                        }
                        break;

                    case ColorizationStyle.ReplaceWithOriginalValue:
                        {
                            Color originalColor = pixelizer.PixCollection[i].Color;
                            Color adjustedColor = GetClosestColorizerColor(originalColor);

                            float originalHue, originalSaturation, originalValue;
                            float hue, saturation, value;

                            Color.RGBToHSV(originalColor, out originalHue, out originalSaturation, out originalValue);
                            Color.RGBToHSV(adjustedColor, out hue, out saturation, out value);

                            adjustedColor = Color.HSVToRGB(hue, saturation, originalValue);

                            pixelizer.PixCollection[i].SetColor(adjustedColor);
                        }
                        break;
                }
            }

            pixelizer.Texturizer.Texturize();
            OnColorize?.Invoke();
        }

        private Color GetClosestColorizerColor(Color color)
        {
            float hue, saturation, value;
            Color.RGBToHSV(color, out hue, out saturation, out value);

            float colorDifference = Mathf.Infinity;

            Color closestColor = Color.white;

            switch(replacementStyle)
            {
                case ReplacementStyle.ReplaceUsingHue:
                    foreach(var colorizerColor in colorPalette.Colors)
                    {
                        Vector3 colorHue = new Vector3(color.r, color.g, color.b);
                        Vector3 colorizerColorHue = new Vector3(colorizerColor.r, colorizerColor.g, colorizerColor.b);

                        float difference = Vector3.Distance(colorHue, colorizerColorHue);

                        if(difference < colorDifference)
                        {
                            closestColor = colorizerColor;
                            colorDifference = difference;
                        }
                    }
                    break;

                case ReplacementStyle.ReplaceUsingSaturation:
                    foreach(var colorizerColor in colorPalette.Colors)
                    {
                        float colorizerHue, colorizerSaturation, colorizerValue;
                        Color.RGBToHSV(colorizerColor, out colorizerHue, out colorizerSaturation, out colorizerValue);

                        float difference = Mathf.Abs(saturation - colorizerSaturation);

                        if(difference < colorDifference)
                        {
                            closestColor = colorizerColor;
                            colorDifference = difference;
                        }
                    }
                    break;
                case ReplacementStyle.ReplaceUsingValue:
                    foreach(var colorizerColor in colorPalette.Colors)
                    {
                        float colorizerHue, colorizerSaturation, colorizerValue;
                        Color.RGBToHSV(colorizerColor, out colorizerHue, out colorizerSaturation, out colorizerValue);

                        float difference = Mathf.Abs(value - colorizerValue);

                        if(difference < colorDifference)
                        {
                            closestColor = colorizerColor;
                            colorDifference = difference;
                        }
                    }
                    break;
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

#if UNITY_EDITOR

            ColorPalette newColorPalette = ScriptableObject.CreateInstance<ColorPalette>();

            foreach(var centroid in centroids)
            {
                newColorPalette.Colors.Add(centroid);
            }

            string path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Pixelization/Colorizer/ScriptableObjects/ColorPalette_.asset");

            UnityEditor.AssetDatabase.CreateAsset(newColorPalette, path);

            colorPalette = newColorPalette;
#endif
        }

        public void ComplementColors()
        {
            foreach(var pix in pixelizer.PixCollection)
            {
                pix.ComplementColor();
            }

            pixelizer.Texturizer.Texturize();
            OnColorize?.Invoke();
        }

        public void InvertColors()
        {
            foreach(var pix in pixelizer.PixCollection)
            {
                pix.InvertColor();
            }

            pixelizer.Texturizer.Texturize();
            OnColorize?.Invoke();
        }

        public void ResetColors()
        {
            foreach(var pix in pixelizer.PixCollection)
            {
                pix.ResetColor();
            }

            pixelizer.Texturizer.Texturize();
            OnColorize?.Invoke();
        }
    }
}