using UnityEngine;

namespace AngryKoala.Pixelization
{
    public class Pixelizer : MonoBehaviour
    {
        [SerializeField] private Texture2D texture;
        public Texture2D Texture => texture;

        [SerializeField] private int width;
        public int Width => width;
        [SerializeField] private int height;
        public int Height => height;

        [SerializeField] private bool preserveRatio;
        public bool PreserveRatio => preserveRatio;

        private int previousWidth;
        private int previousHeight;

        [SerializeField] private float pixSize;

        [SerializeField] private Pix pixPrefab;

        [SerializeField] private Pix[] pixCollection;
        public Pix[] PixCollection => pixCollection;

        private void OnValidate()
        {
            width = Mathf.Max(width, 1);
            height = Mathf.Max(height, 1);

            pixSize = Mathf.Max(pixSize, Mathf.Epsilon);

            if(preserveRatio)
            {
                AdjustGridSize();
            }
        }

        public void Pixelize()
        {
            if(preserveRatio)
            {
                AdjustGridSize();
            }

            CreateGrid();

            SetPixColors();
        }

        public void AdjustGridSize()
        {
            if(texture == null)
            {
                return;
            }

            float ratio = (float)texture.width / texture.height;

            if(previousWidth != width)
            {
                height = Mathf.FloorToInt(width * (1f / ratio));

                previousWidth = width;
                previousHeight = height;
            }
            else if(previousHeight != height)
            {
                width = Mathf.FloorToInt(height * ratio);

                previousWidth = width;
                previousHeight = height;
            }
        }

        private void CreateGrid()
        {
            Clear();

            pixCollection = new Pix[width * height];
            int pixIndex = 0;

            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    Pix pix = Instantiate(pixPrefab, transform);

                    pix.gameObject.name = $"Pix[{i},{j}]";
                    pix.transform.localPosition = new Vector3(-width * pixSize / 2f + pixSize / 2f + i * pixSize, 0f, -height * pixSize / 2f + pixSize / 2f + j * pixSize);
                    pix.transform.localScale = new Vector3(pixSize, 1f, pixSize);

                    pixCollection[pixIndex] = pix;
                    pixIndex++;
                }
            }
        }

        private void SetPixColors()
        {
            int textureAreaX = Mathf.FloorToInt((float)texture.width / width - 1);
            int textureAreaY = Mathf.FloorToInt((float)texture.height / height);

            for(int i = 0; i < width * height; i++)
            {
                Color color = GetAverageColor(texture.GetPixels(i / height * textureAreaX, i % height * textureAreaY, textureAreaX, textureAreaY));

                pixCollection[i].OriginalColor = color;
                pixCollection[i].SetColor(color);
            }
        }

        private Color GetAverageColor(Color[] colors)
        {
            float r = 0f;
            float g = 0f;
            float b = 0f;

            for(int i = 0; i < colors.Length; i++)
            {
                r += colors[i].r;
                g += colors[i].g;
                b += colors[i].b;
            }

            r /= colors.Length;
            g /= colors.Length;
            b /= colors.Length;

            return new Color(r, g, b);
        }

        public void Clear()
        {
            if(pixCollection != null)
            {
                for(int i = 0; i < pixCollection.Length; i++)
                {
                    DestroyImmediate(pixCollection[i].gameObject);
                }
            }

            pixCollection = null;
        }
    }
}