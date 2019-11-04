using UnityEngine;

namespace Hirame.Muses
{
    [System.Serializable]
    public class Follow : MusesBehavior
    {
        [SerializeField] private Transform target;
    
        [SerializeField] private Vector3 offset;
        [SerializeField] private CoordinateSpace space;

        public Vector3 GetPosition (VirtualCamera virtualCamera)
        {
            if (target == false)
                return virtualCamera.AttachedTransform.position;

            var adjustedOffset = space == CoordinateSpace.Local ? target.TransformVector (offset) : offset;
            
            return target.position + adjustedOffset;
        }

        public override void OnDrawGizmos (VirtualCamera virtualCamera)
        {
            if (target == false)
                return;
            
            Gizmos.color = Color.gray;
            Gizmos.DrawLine (virtualCamera.transform.position, target.position);
        }
    }

}