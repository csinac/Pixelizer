using UnityEngine;

namespace AngryKoala.Pixelization
{
    [SelectionBase]
    public class Pix : MonoBehaviour
    {
        [SerializeField] private MeshRenderer pixMeshRenderer;
        public MeshRenderer MeshRenderer => pixMeshRenderer;

        [HideInInspector] public Color OriginalColor;

        [SerializeField] private Color color;
        public Color Color => color;

        private Color currentColor;

        [SerializeField][Range(0f, 1f)] private float hue;

        [SerializeField][Range(0f, 1f)] private float saturation;

        [SerializeField][Range(0f, 1f)] private float brightness;

        private void OnValidate()
        {
            color = Color.HSVToRGB(hue, saturation, brightness);
        }

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
    }
}