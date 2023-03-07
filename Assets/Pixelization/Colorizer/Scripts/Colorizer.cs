using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace AngryKoala.Pixelization
{
    public class Colorizer : MonoBehaviour
    {
        [SerializeField] private Pixelizer pixelizer;

        [SerializeField] private ColorPalette colorPalette;
        public ColorPalette ColorPalette => colorPalette;

        [SerializeField][OnValueChanged("OnColorPaletteColorCountChanged")][Range(1, 20)] private int colorPaletteColorCount;

        [SerializeField] private bool createNewColorPalette;

        private enum ColorizationStyle { Replace, ReplaceWithOriginalSaturation, ReplaceWithOriginalValue }
        [SerializeField] private ColorizationStyle colorizationStyle;

        private bool showUseRamp => (colorizationStyle == ColorizationStyle.ReplaceWithOriginalSaturation || colorizationStyle == ColorizationStyle.ReplaceWithOriginalValue);
        [SerializeField][ShowIf("showUseRamp")] private bool useRamp;
        private bool showRampCount => (colorizationStyle == ColorizationStyle.ReplaceWithOriginalSaturation || colorizationStyle == ColorizationStyle.ReplaceWithOriginalValue) && useRamp;
        [SerializeField][ShowIf("showRampCount")][Range(1, 100)] private int rampCount = 1;

        private enum ReplacementStyle { ReplaceUsingHue, ReplaceUsingSaturation, ReplaceUsingValue }
        [SerializeField] private ReplacementStyle replacementStyle;

        [SerializeField] private bool useColorGroups;

        private List<Color> colorGroupsColors = new List<Color>();
        private Color[] sortedColorPaletteColors;

        public static UnityAction OnColorize;

        public void Colorize()
        {
            if(pixelizer.PixCollection.Length == 0)
            {
                Debug.LogWarning("Pixelize a texture first");
                return;
            }

            if(createNewColorPalette)
            {
                ColorPalette newColorPalette = ScriptableObject.CreateInstance<ColorPalette>();

                foreach(var color in GetColorPalette(colorPaletteColorCount))
                {
                    newColorPalette.Colors.Add(color);
                }

                colorPalette = newColorPalette;
            }

            if(colorPalette == null)
            {
                Debug.LogWarning("Color palette is not assigned");
                return;
            }

            if(colorPalette.Colors.Count == 0)
            {
                Debug.LogWarning("No colors selected");
                return;
            }

            if(useColorGroups)
            {
                colorGroupsColors = GetColorPalette(colorPalette.Colors.Count);
                MapColorPaletteColorsToColorGroupsColors();
            }

            for(int i = 0; i < pixelizer.PixCollection.Length; i++)
            {
                switch(colorizationStyle)
                {
                    case ColorizationStyle.Replace:
                        if(useColorGroups)
                        {
                            Color closestColor = GetClosestColor(pixelizer.PixCollection[i].Color, colorGroupsColors);
                            pixelizer.PixCollection[i].ColorIndex = colorGroupsColors.IndexOf(closestColor);
                            pixelizer.PixCollection[i].SetColor(sortedColorPaletteColors[pixelizer.PixCollection[i].ColorIndex]);
                        }
                        else
                        {
                            pixelizer.PixCollection[i].SetColor(GetClosestColor(pixelizer.PixCollection[i].Color, colorPalette.Colors));
                        }
                        break;

                    case ColorizationStyle.ReplaceWithOriginalSaturation:
                        {
                            Color originalColor = pixelizer.PixCollection[i].Color;
                            Color adjustedColor;

                            if(useColorGroups)
                            {
                                adjustedColor = GetClosestColor(originalColor, colorGroupsColors);
                                pixelizer.PixCollection[i].ColorIndex = colorGroupsColors.IndexOf(adjustedColor);
                                adjustedColor = sortedColorPaletteColors[pixelizer.PixCollection[i].ColorIndex];
                            }
                            else
                            {
                                adjustedColor = GetClosestColor(originalColor, colorPalette.Colors);
                            }

                            float hue, saturation, value;
                            Color.RGBToHSV(adjustedColor, out hue, out saturation, out value);

                            float originalSaturation = originalColor.Saturation();

                            if(useRamp)
                            {
                                adjustedColor = Color.HSVToRGB(hue, (100f / rampCount) * (Mathf.RoundToInt((originalSaturation * 100f) / (100f / rampCount))) / 100f, value);
                            }
                            else
                            {
                                adjustedColor = Color.HSVToRGB(hue, originalSaturation, value);
                            }

                            pixelizer.PixCollection[i].SetColor(adjustedColor);
                        }
                        break;

                    case ColorizationStyle.ReplaceWithOriginalValue:
                        {
                            Color originalColor = pixelizer.PixCollection[i].Color;
                            Color adjustedColor;

                            if(useColorGroups)
                            {
                                adjustedColor = GetClosestColor(originalColor, colorGroupsColors);
                                pixelizer.PixCollection[i].ColorIndex = colorGroupsColors.IndexOf(adjustedColor);
                                adjustedColor = sortedColorPaletteColors[pixelizer.PixCollection[i].ColorIndex];
                            }
                            else
                            {
                                adjustedColor = GetClosestColor(originalColor, colorPalette.Colors);
                            }

                            float hue, saturation, value;
                            Color.RGBToHSV(adjustedColor, out hue, out saturation, out value);

                            float originalValue = originalColor.Value();

                            if(useRamp)
                            {
                                adjustedColor = Color.HSVToRGB(hue, saturation, (100f / rampCount) * (Mathf.RoundToInt((originalValue * 100f) / (100f / rampCount))) / 100f);
                            }
                            else
                            {
                                adjustedColor = Color.HSVToRGB(hue, saturation, originalValue);
                            }

                            pixelizer.PixCollection[i].SetColor(adjustedColor);
                        }
                        break;
                }
            }

            pixelizer.Texturizer.Texturize();
            OnColorize?.Invoke();
        }

        private Color GetClosestColor(Color color, List<Color> colorizerColors)
        {
            float hue, saturation, value;
            Color.RGBToHSV(color, out hue, out saturation, out value);

            float colorDifference = Mathf.Infinity;

            Color closestColor = Color.white;

            switch(replacementStyle)
            {
                case ReplacementStyle.ReplaceUsingHue:
                    foreach(var colorizerColor in colorizerColors)
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
                    foreach(var colorizerColor in colorizerColors)
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
                    foreach(var colorizerColor in colorizerColors)
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

        #region Color Palette

        private List<Color> GetColorPalette(int colorCount)
        {
            int iterationCount = 10;

            List<Color> pixels = new List<Color>();
            foreach(var pix in pixelizer.PixCollection)
            {
                pixels.Add(pix.OriginalColor);
            }
            int pixelCount = pixels.Count;

            Color[] centroids = new Color[colorCount];
            for(int i = 0; i < colorCount; i++)
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
                    for(int k = 0; k < colorCount; k++)
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

                for(int j = 0; j < colorCount; j++)
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

            return centroids.ToList();
        }

        public void ExtractColorPalette()
        {
#if UNITY_EDITOR
            if(colorPaletteColorCount <= 0)
            {
                Debug.LogWarning("Color palette color count must be greater than 0");
                return;
            }

            List<Color> centroids = GetColorPalette(colorPaletteColorCount);

            ColorPalette newColorPalette = ScriptableObject.CreateInstance<ColorPalette>();

            foreach(var centroid in centroids)
            {
                newColorPalette.Colors.Add(centroid);
            }
            colorPalette = newColorPalette;
#endif
        }

        public void SaveColorPalette()
        {
#if UNITY_EDITOR
            if(colorPalette == null)
            {
                Debug.LogWarning("Color palette is not assigned");
                return;
            }

            string path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Pixelization/Colorizer/ScriptableObjects/ColorPalette_.asset");

            UnityEditor.AssetDatabase.CreateAsset(colorPalette, path);
#endif
        }

        private void MapColorPaletteColorsToColorGroupsColors()
        {
            List<Color> colorPaletteColorsToSort = new List<Color>();
            List<Color> colorGroupsColorsToSort = new List<Color>();

            foreach(var color in colorPalette.Colors)
            {
                colorPaletteColorsToSort.Add(color);
            }

            foreach(var color in colorGroupsColors)
            {
                colorGroupsColorsToSort.Add(color);
            }

            sortedColorPaletteColors = new Color[colorPaletteColorsToSort.Count];

            switch(replacementStyle)
            {
                case ReplacementStyle.ReplaceUsingHue:
                    {
                        int i = colorPalette.Colors.Count - 1;
                        while(i >= 0)
                        {
                            Vector3 colorGroupsColorHue = new Vector3(colorGroupsColorsToSort[i].r, colorGroupsColorsToSort[i].g, colorGroupsColorsToSort[i].b);

                            float colorDifference = Mathf.Infinity;

                            Color closestColor = Color.white;

                            for(int index = i; index >= 0; index--)
                            {
                                Vector3 colorPaletteColorHue = new Vector3(colorPaletteColorsToSort[index].r, colorPaletteColorsToSort[index].g, colorPaletteColorsToSort[index].b);

                                float difference = Vector3.Distance(colorPaletteColorHue, colorGroupsColorHue);

                                if(difference < colorDifference)
                                {
                                    closestColor = colorPaletteColorsToSort[index];
                                    colorDifference = difference;
                                }
                            }

                            sortedColorPaletteColors[i] = closestColor;
                            colorPaletteColorsToSort.Remove(closestColor);
                            colorGroupsColorsToSort.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                case ReplacementStyle.ReplaceUsingSaturation:
                    {
                        int i = colorPalette.Colors.Count - 1;
                        while(i >= 0)
                        {
                            float colorGroupsColorSaturation = colorGroupsColorsToSort[i].Saturation();

                            float colorDifference = Mathf.Infinity;

                            Color closestColor = Color.white;

                            for(int index = i; index >= 0; index--)
                            {
                                float colorPaletteColorSaturation = colorPaletteColorsToSort[index].Saturation();

                                float difference = Mathf.Abs(colorPaletteColorSaturation - colorGroupsColorSaturation);

                                if(difference < colorDifference)
                                {
                                    closestColor = colorPaletteColorsToSort[index];
                                    colorDifference = difference;
                                }
                            }

                            sortedColorPaletteColors[i] = closestColor;
                            colorPaletteColorsToSort.Remove(closestColor);
                            colorGroupsColorsToSort.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                case ReplacementStyle.ReplaceUsingValue:
                    {
                        int i = colorPalette.Colors.Count - 1;
                        while(i >= 0)
                        {
                            float colorGroupsColorValue = colorGroupsColorsToSort[i].Value();

                            float colorDifference = Mathf.Infinity;

                            Color closestColor = Color.white;

                            for(int index = i; index >= 0; index--)
                            {
                                float colorPaletteColorValue = colorPaletteColorsToSort[index].Value();

                                float difference = Mathf.Abs(colorPaletteColorValue - colorGroupsColorValue);

                                if(difference < colorDifference)
                                {
                                    closestColor = colorPaletteColorsToSort[index];
                                    colorDifference = difference;
                                }
                            }

                            sortedColorPaletteColors[i] = closestColor;
                            colorPaletteColorsToSort.Remove(closestColor);
                            colorGroupsColorsToSort.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Color Operations

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

        #endregion

        #region Validation

        private void OnColorPaletteColorCountChanged()
        {
            colorPaletteColorCount = Mathf.Max(colorPaletteColorCount, 1);
        }

        #endregion
    }
}