using Hirame.Pantheon;
using Hirame.Pantheon.Core;
using UnityEngine;

namespace Hirame.Muses
{
    public class CameraSystem : GameSystem<CameraSystem>, ICameraUpdate
    {
        private MusesCamera mainCamera;
        private readonly PriorityQueue<VirtualCamera> virtualCameras = new PriorityQueue<VirtualCamera> ();

        public static MusesCamera MainCamera => GetOrCreate ().mainCamera;

        internal static void SetMusesCamera (MusesCamera mCam)
        {
            GetOrCreate ().mainCamera = mCam;
        }

        internal static void TrackVirtualCamera (VirtualCamera vCam)
        {
            GetOrCreate ().virtualCameras.Enqueue (vCam);
        }

        internal static void UnTrackVirtualCamera (VirtualCamera vCam)
        {
            GetOrCreate ().virtualCameras.Remove (vCam);
        }

        void ICameraUpdate.OnCameraUpdate ()
        {
            if (mainCamera == false || virtualCameras.Count == 0)
                return;

            var mainCamTransform = mainCamera.transform;

            var mCamPosition = mainCamTransform.position;
            var mCamRotation = mainCamTransform.forward;

            var vCam = virtualCameras.Peek ();

            var (vCamPosition, vCamRotation) = vCam.GetDesiredPositionAndRotation ();
            var smooth = vCam.SmoothValue;

            var framePosition = LerpPosition (
                in mCamPosition, in vCamPosition, smooth, MainCamera.PositionBlendSpeed);
            
            var frameRotation = LerpRotation (
                in mCamRotation, in vCamRotation, smooth, MainCamera.RotationBlendSpeed);

            vCam.transform.SetPositionAndRotation (framePosition, frameRotation);
            #if UNITY_EDITOR
            if (Application.isPlaying == false)
                return;
            #endif
            mainCamTransform.SetPositionAndRotation (framePosition, frameRotation);
        }

        private Vector3 blendVelocity;
        private Vector3 blendVelocityAngular;

        private Vector3 LerpPosition (in Vector3 from, in Vector3 to, float smooth, float blend)
        {
            return Vector3.SmoothDamp (
                from, to, ref blendVelocity, smooth, blend, Time.smoothDeltaTime);
        }

        private Quaternion LerpRotation (in Vector3 from, in Vector3 to, float smooth, float blend)
        {
            var lookDirection = Vector3.SmoothDamp (
                from, to, ref blendVelocityAngular, smooth, blend, Time.smoothDeltaTime);
            //lookDirection.z = 0;

            var xz = Vector3.ProjectOnPlane (lookDirection, Vector3.up);
            xz.Normalize ();
            
            var xRot = Vector3.SignedAngle (xz, lookDirection, Vector3.Cross (Vector3.up, xz));
            var yRot = Vector3.SignedAngle (Vector3.forward, xz, Vector3.up);

            return Quaternion.Euler (xRot, yRot, 0);
        }
    }
}