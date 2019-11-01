using UnityEngine;

namespace Hirame.Muses
{
    [RequireComponent (typeof (Camera))]
    [ExecuteInEditMode]
    public class MusesCamera : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        [Tooltip ("Snap the Camera to the position of first virtual camera")]
        [SerializeField] private bool inheritPosition = true;
        
        public Camera UnityCamera => mainCamera;

        private void OnEnable ()
        {
            CameraSystem.SetMusesCamera (this);
        }

        private void Reset ()
        {
            if (mainCamera == false)
                mainCamera = GetComponent<Camera> ();
        }
    }

}