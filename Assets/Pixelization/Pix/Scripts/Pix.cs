using NaughtyAttributes;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    public class Pix : MonoBehaviour
    {
        [HideInInspector] public Pixelizer Pixelizer;

        [SerializeField] private MeshFilter pixMeshFilter;
        public MeshFilter MeshFilter => pixMeshFilter;

        [SerializeField] private MeshRenderer pixMeshRenderer;
        public MeshRenderer MeshRenderer => pixMeshRenderer;

        public Vector2Int Position;

        [HideInInspector] public Color OriginalColor;

        [SerializeField][OnValueChanged("OnColorChanged")] private Color color;
        public Color Color => color;

        private Color currentColor;

        [SerializeField][OnValueChanged("OnHSVChanged")][Range(0f, 1f)] private float hue;

        [SerializeField][OnValueChanged("OnHSVChanged")][Range(0f, 1f)] private float saturation;

        [SerializeField][OnValueChanged("OnHSVChanged")][Range(0f, 1f)] private float brightness;

        public Vector2[] uvs = new Vector2[4];
        private void Start()
        {
            if(Pixelizer.UsePerformanceMode)
            {
                pixMeshRenderer.sharedMaterial.shader = Shader.Find("Unlit/Texture");

                Mesh mesh = pixMeshFilter.mesh;


                uvs[0] = ConvertPixelsToUV(Position.x + .1f, Position.y + .9f, Pixelizer.Width, Pixelizer.Height);
                uvs[1] = ConvertPixelsToUV(Position.x + .9f, Position.y + .9f, Pixelizer.Width, Pixelizer.Height);
                uvs[2] = ConvertPixelsToUV(Position.x + .1f, Position.y + .1f, Pixelizer.Width, Pixelizer.Height);
                uvs[3] = ConvertPixelsToUV(Position.x + .9f, Position.y + .1f, Pixelizer.Width, Pixelizer.Height);

                mesh.uv = uvs;

                return;
            }

            pixMeshRenderer.material.shader = Shader.Find("Unlit/Color");
            pixMeshRenderer.material.color = color;

            currentColor = color;
        }

        private void Update()
        {
            if(Pixelizer.UsePerformanceMode)
                return;

            if(currentColor != color)
            {
                pixMeshRenderer.material.color = color;
                currentColor = color;
            }
        }

        public void SetColor(Color color)
        {
            this.color = color;

            Color.RGBToHSV(color, out hue, out saturation, out brightness);
        }

        public void ResetColor()
        {
            color = OriginalColor;

            Color.RGBToHSV(color, out hue, out saturation, out brightness);
        }

        public void ComplementColor()
        {
            float maxValue = 0f;
            float minValue = 1f;

            for(int i = 0; i < 3; i++)
            {
                if(color[i] >= maxValue)
                {
                    maxValue = color[i];
                }
                if(color[i] <= minValue)
                {
                    minValue = color[i];
                }
            }

            color = new Color(maxValue + minValue - color.r, maxValue + minValue - color.g, maxValue + minValue - color.b);
        }

        public void InvertColor()
        {
            color = new Color(1 - color.r, 1 - color.g, 1 - color.b);
        }

        private Vector2 ConvertPixelsToUV(float x, float y, int textureWidth, int textureHeight)
        {
            return new Vector2(x / textureWidth, y / textureHeight);
        }

        #region Validation

        private void OnColorChanged()
        {
            Color.RGBToHSV(color, out hue, out saturation, out brightness);
        }

        private void OnHSVChanged()
        {
            color = Color.HSVToRGB(hue, saturation, brightness);
        }

        #endregion
    }
}