using UnityEngine;

namespace AngryKoala.Pixelization
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        private void OnEnable()
        {
            Pixelizer.OnPixelize += SetCameraSize;
        }

        private void OnDisable()
        {
            Pixelizer.OnPixelize -= SetCameraSize;
        }

        public void SetCameraSize(float width, float height)
        {
            if(width / height >= mainCamera.aspect)
            {
                mainCamera.orthographicSize = width / (2 * mainCamera.aspect);
            }
            else
            {
                mainCamera.orthographicSize = height / 2f;
            }
        }
    }
}