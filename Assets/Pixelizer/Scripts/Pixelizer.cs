using UnityEngine;

namespace AngryKoala.Pixel
{
    public class Pixelizer : MonoBehaviour
    {
        [SerializeField] private int width;
        [SerializeField] private int height;

        [SerializeField] private Pix pixPrefab;

        [SerializeField] private Pix[] pixCollection;

        public void Pixelize()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
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
    }
}