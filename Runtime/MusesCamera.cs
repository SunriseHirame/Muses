using UnityEngine;

namespace Hirame.Muses
{
    [RequireComponent (typeof (Camera))]
    [ExecuteInEditMode]
    public class MusesCamera : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        [SerializeField] private float globalPositionBlendSpeed = 40f;
        [SerializeField] private float globalRotationBlendSpeed = 200f;
        
        [Tooltip ("Snap the Camera to the position of first virtual camera")]
        [SerializeField] private bool inheritPosition = true;

        public float PositionBlendSpeed => globalPositionBlendSpeed;
        public float RotationBlendSpeed => globalRotationBlendSpeed;
        
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