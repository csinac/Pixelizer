using NaughtyAttributes;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    public class Pix : MonoBehaviour
    {
        [SerializeField] private MeshRenderer pixMeshRenderer;
        public MeshRenderer MeshRenderer => pixMeshRenderer;

        [HideInInspector] public Color OriginalColor;

        [SerializeField] private Color color;
        public Color Color => color;

        private Color currentColor;

        [SerializeField][OnValueChanged("OnHSVChanged")][Range(0f, 1f)] private float hue;

        [SerializeField][OnValueChanged("OnHSVChanged")][Range(0f, 1f)] private float saturation;

        [SerializeField][OnValueChanged("OnHSVChanged")][Range(0f, 1f)] private float brightness;

        private void Start()
        {
            color = OriginalColor;

            pixMeshRenderer.material.color = color;
            pixMeshRenderer.material.SetFloat("_Glossiness", 0f);

            currentColor = color;
        }

        private void Update()
        {
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

        #region Validation

        private void OnHSVChanged()
        {
            color = Color.HSVToRGB(hue, saturation, brightness);
        }

        #endregion
    }
}