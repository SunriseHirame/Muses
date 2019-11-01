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
        
        public void OnCameraUpdate ()
        {
            if (mainCamera == false || virtualCameras.Count == 0)
                return;
            
            var mainCamTransform = mainCamera.transform;

            var mCamPosition = mainCamTransform.position;
            var mCamRotation = mainCamTransform.eulerAngles;

            var vCam = virtualCameras.Peek ();
            
            var vCamPosition = vCam.Position;
            var vCamRotation = vCam.transform.eulerAngles;
            var smooth = vCam.SmoothValue;
            
            var framePosition = LerpPosition (in mCamPosition, in vCamPosition, smooth);
            var frameRotation = LerpRotation (mCamRotation, vCamRotation, smooth);

            mainCamTransform.SetPositionAndRotation (framePosition, frameRotation);
        }

        private Vector3 blendVelocity;
        private Vector3 blendVelocityAngular;

        private Vector3 LerpPosition (in Vector3 from, in Vector3 to, float smooth)
        {
            return Vector3.SmoothDamp (from, to, ref blendVelocity, smooth, 20f, Time.smoothDeltaTime);
        }

        private Quaternion LerpRotation (Vector3 from, Vector3 to, float smooth)
        {
            if (from.x > 180)
                from.x -= 360;
            
            if (to.x > 180)
                to.x -= 360;
            
            var frame = Vector3.SmoothDamp (from, to, ref blendVelocityAngular, smooth, 120f, Time.smoothDeltaTime);
            frame.z = 0;
            
            return Quaternion.Euler (frame);
        }
    }
}