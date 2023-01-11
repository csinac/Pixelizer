using UnityEngine;

namespace AngryKoala.Pixel
{
    public class Pixelizer : MonoBehaviour
    {
        [SerializeField] private Texture2D texture;

        [SerializeField] private int width;
        public int Width => width;
        [SerializeField] private int height;
        public int Height => height;

        [SerializeField] private float pixSize;

        [SerializeField] private Pix pixPrefab;

        [SerializeField] private Pix[] pixCollection;
        public Pix[] PixCollection => pixCollection;

        public void Pixelize()
        {
            if(texture == null)
            {
                Debug.LogError("No texture found to pixelize");
                return;
            }

            CreateGrid();

            SetPixColors();
        }

        private void CreateGrid()
        {
            if(pixCollection != null)
            {
                for(int i = 0; i < pixCollection.Length; i++)
                {
                    DestroyImmediate(pixCollection[i].gameObject);
                }
            }

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
                pixCollection[i].SetColor(GetAverageColor(texture.GetPixels(i / height * textureAreaX, i % height * textureAreaY, textureAreaX, textureAreaY)));
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
    }
}