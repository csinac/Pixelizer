using UnityEngine;

namespace AngryKoala.Pixel
{
    public class Pixelizer : MonoBehaviour
    {
        [SerializeField] private Texture2D texture;

        [SerializeField] private int width;
        [SerializeField] private int height;

        [SerializeField] private Pix pixPrefab;

        [SerializeField] private Pix[] pixCollection;

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
                    pix.transform.localPosition = new Vector3(-width / 2f + .5f + i, 0f, -height / 2f + .5f + j);

                    pixCollection[pixIndex] = pix;
                    pixIndex++;
                }
            }
        }

        private void SetPixColors()
        {
            int textureAreaX = texture.width / width;
            int textureAreaY = texture.height / height;

            for(int i = 0; i < width * height; i++)
            {
                pixCollection[i].SetColor(GetAverageColor(texture.GetPixels(i / width * textureAreaX, i % width * textureAreaY, textureAreaX, textureAreaY)));
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