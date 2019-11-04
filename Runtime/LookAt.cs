using UnityEngine;

namespace Hirame.Muses
{
    [System.Serializable]
    public class LookAt : MusesBehavior
    {
        [SerializeField] private Transform target;

        [SerializeField] private Vector3 offset;
        [SerializeField] private CoordinateSpace space;

        public Vector3 GetForwardVector (VirtualCamera virtualCamera)
        {
            if (target == false)
                return virtualCamera.AttachedTransform.forward;

            var adjustedOffset = space == CoordinateSpace.Local ? target.TransformVector (offset) : offset;

            var direction = target.position - virtualCamera.AttachedTransform.position + adjustedOffset;
            return direction.normalized;
        }

        public override void OnDrawGizmos (VirtualCamera virtualCamera)
        {
            if (target == false)
                return;
            
            var adjustedOffset = space == CoordinateSpace.Local ? target.TransformVector (offset) : offset;

            Gizmos.color = Color.gray;
            Gizmos.DrawLine (target.position, target.position + adjustedOffset);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine (virtualCamera.transform.position, target.position + adjustedOffset);
            Gizmos.DrawSphere (target.position + adjustedOffset, 0.2f);
        }
    }

}