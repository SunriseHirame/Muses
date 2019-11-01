using UnityEngine;

namespace Hirame.Muses
{
    [ExecuteInEditMode]
    public class VirtualCamera : MonoBehaviour, System.IComparable<VirtualCamera>
    {
        [SerializeField] private int priority;
        [Range (0.1f, 10)]
        [SerializeField] private float smoothValue = 1f;
        
        [SerializeField] private Transform attachedTransform;

        public Vector3 Position => attachedTransform.position;
        public Quaternion Rotation => attachedTransform.rotation;
        public Vector3 RotationEuler => attachedTransform.eulerAngles;

        public int Priority
        {
            get => priority;
            set
            {
                if (priority == value)
                    return;
                
                priority = value;
                UpdateCamera ();
            }
        }

        public float SmoothValue
        {
            get => smoothValue;
            set => smoothValue = Mathf.Clamp (value, 0, 10);
        }

        public void UpdateCamera ()
        {
            if (!enabled)
                return;
            
            CameraSystem.UnTrackVirtualCamera (this);
            CameraSystem.TrackVirtualCamera (this);
        }

        private void OnEnable ()
        {
            CameraSystem.TrackVirtualCamera (this);
        }

        private void OnDisable ()
        {
            CameraSystem.UnTrackVirtualCamera (this);
        }

        private void OnValidate ()
        {
            if (attachedTransform == false)
                attachedTransform = transform;
        }

        public int CompareTo (VirtualCamera other)
        {
            return -(priority - other.priority);
        }
    }

}
