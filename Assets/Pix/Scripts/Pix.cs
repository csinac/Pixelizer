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
        }

        public void ResetColor()
        {
            this.color = OriginalColor;
        }
    }
}