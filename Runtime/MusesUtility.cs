using UnityEngine;

namespace Hirame.Muses
{
    public static class MusesUtility
    {
        public static Quaternion GetCameraLookRotation (in Vector3 lookDirection)
        {
            var xz = Vector3.ProjectOnPlane (lookDirection, Vector3.up);
            xz.Normalize ();
            
            var xRot = Vector3.SignedAngle (xz, lookDirection, Vector3.Cross (Vector3.up, xz));
            var yRot = Vector3.SignedAngle (Vector3.forward, xz, Vector3.up);
            
            return Quaternion.Euler (xRot, yRot, 0);
        }
    
    }

}