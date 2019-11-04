using System;
using Hirame.Pantheon;
using UnityEngine;

namespace Hirame.Muses
{
    [ExecuteInEditMode]
    public class VirtualCamera : MonoBehaviour, System.IComparable<VirtualCamera>
    {
        [SerializeField] private int priority;
        [Range (0.1f, 10)]
        [SerializeField] private float smoothValue = 1f;

        [SerializeField] private Follow follow;
        [SerializeField] private LookAt lookAt;
        
        [ShowIfNull]
        [SerializeField] private Transform attachedTransform;

        public Transform AttachedTransform => attachedTransform;
        
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

        public (Vector3 position, Vector3 lookDirection) GetDesiredPositionAndRotation ()
        {
            var position = follow.GetPosition (this);
            var lookDirection = lookAt.GetForwardVector (this);
            return (position, lookDirection);
        }

        public void UpdateCamera ()
        {
            if (!enabled)
                return;
            
            CameraSystem.UnTrackVirtualCamera (this);
            CameraSystem.TrackVirtualCamera (this);
        }

        public void PushState (Vector3 position, Quaternion rotation)
        {
            AttachedTransform.SetPositionAndRotation (position, rotation);
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

        private void OnDrawGizmosSelected ()
        {
            lookAt.OnDrawGizmos (this);
            follow.OnDrawGizmos (this);
        }
    }

}
