using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace AngryKoala.Pixelization
{
    public class Pixelizer : MonoBehaviour
    {
        [SerializeField][OnValueChanged("PreserveRatio")] private Texture2D texture;
        public Texture2D Texture => texture;

        [HideInInspector] public Texture2D TexturizedTexture;

        private bool showOriginalDimensions => texture != null;

        [SerializeField][ReadOnly][ShowIf("showOriginalDimensions")] private int originalWidth;
        [SerializeField][ReadOnly][ShowIf("showOriginalDimensions")] private int originalHeight;

        [SerializeField][OnValueChanged("OnWidthChanged")] private int width;
        [SerializeField][HideInInspector] private int currentWidth;
        public int Width => width;
        [SerializeField][OnValueChanged("OnHeightChanged")] private int height;
        [SerializeField][HideInInspector] private int currentHeight;
        public int Height => height;

        [Tooltip("Try to match the width/height ratio of the grid to the texture.")]
        [SerializeField][OnValueChanged("PreserveRatio")] private bool preserveRatio;

        [SerializeField] private float pixSize;

        [SerializeField] private Pix pixPrefab;

        [Tooltip("Use mesh UVs instead of material instances to display texturized image. Greatly reduces draw calls.")]
        [SerializeField][EnableIf("usePerformanceModeEnabled")] private bool usePerformanceMode;
        public bool UsePerformanceMode => usePerformanceMode;

#if UNITY_EDITOR
        private bool usePerformanceModeEnabled => !EditorApplication.isPlaying;
#endif
        [SerializeField] private Pix[] pixCollection;
        public Pix[] PixCollection => pixCollection;

        public static UnityAction<float, float> OnGridSizeUpdated;

        private void Start()
        {
            if(pixCollection.Length > 0)
            {
                OnGridSizeUpdated?.Invoke(currentWidth * pixSize, currentHeight * pixSize);
            }

            if(usePerformanceMode)
            {
                SetPixTextures(TexturizedTexture);
            }
        }

        private void Update()
        {
            OnGridSizeUpdated?.Invoke(currentWidth * pixSize, currentHeight * pixSize);
        }

        private void OnApplicationQuit()
        {
            SetPixTextures(null);
        }

        public void Pixelize()
        {
            CreateGrid();

            SetPixColors();

            OnGridSizeUpdated?.Invoke(currentWidth * pixSize, currentHeight * pixSize);
        }

        private void CreateGrid()
        {
            Clear();

            currentWidth = width;
            currentHeight = height;

            pixCollection = new Pix[width * height];
            int pixIndex = 0;

            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    Pix pix = Instantiate(pixPrefab, transform);

                    pix.Pixelizer = this;
                    pix.Position = new Vector2Int(i, j);

                    pix.gameObject.name = $"Pix[{i},{j}]";
                    pix.transform.localPosition = new Vector3(-width * pixSize / 2f + pixSize / 2f + i * pixSize, 0f, -height * pixSize / 2f + pixSize / 2f + j * pixSize);
                    pix.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    pix.transform.localScale = new Vector3(pixSize, 1f, pixSize);

                    pixCollection[pixIndex] = pix;
                    pixIndex++;
                }
            }
        }

        private void SetPixColors()
        {
            float textureAreaX = (float)texture.width / width;
            float textureAreaY = (float)texture.height / height;

            for(int i = 0; i < width * height; i++)
            {
                Color color = GetAverageColor(texture.GetPixels(Mathf.FloorToInt((i / height) * textureAreaX), Mathf.FloorToInt(i % height * textureAreaY), Mathf.FloorToInt(textureAreaX), Mathf.FloorToInt(textureAreaY)));

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

        private void SetPixTextures(Texture2D texture)
        {
            for(int i = 0; i < pixCollection.Length; i++)
            {
                pixCollection[i].MeshRenderer.sharedMaterial.SetTexture("_MainTex", texture);
            }
        }

        #region Validation

        private void OnValidate()
        {
            pixSize = Mathf.Max(pixSize, Mathf.Epsilon);

            originalWidth = texture != null ? texture.width : 0;
            originalHeight = texture != null ? texture.height : 0;
        }

        private void OnWidthChanged()
        {
            width = Mathf.Max(width, 1);

            if(texture == null)
            {
                return;
            }

            if(preserveRatio)
            {
                float ratio = (float)texture.width / texture.height;

                height = Mathf.FloorToInt(width * (1f / ratio));
                height = Mathf.Max(height, 1);
            }
        }

        private void OnHeightChanged()
        {
            height = Mathf.Max(height, 1);

            if(texture == null)
            {
                return;
            }

            if(preserveRatio)
            {
                float ratio = (float)texture.width / texture.height;

                width = Mathf.FloorToInt(height * ratio);
                width = Mathf.Max(width, 1);
            }
        }

        private void PreserveRatio()
        {
            if(width >= height)
            {
                OnWidthChanged();
            }
            else
            {
                OnHeightChanged();
            }
        }

        #endregion
    }
}