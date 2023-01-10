using UnityEngine;

namespace AngryKoala.Pixel
{
    public class Pix : MonoBehaviour
    {
        [SerializeField] private MeshRenderer pixMeshRenderer;
        public MeshRenderer MeshRenderer => pixMeshRenderer;

        [SerializeField] private Color color;
        public Color Color => color;
    }
}