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

#if UNITY_EDITOR
        public static VirtualCamera SoloCamera;
        private static Camera previewCamera;

        public static Camera GerOrCreatePreviewCamera ()
        {
            if (!previewCamera)
            {
                var previewGo = GameObject.Find ("Preview Camera");
                if (previewGo)
                    previewCamera = previewGo.GetComponent<Camera> ();
                
                if (!previewCamera)
                    previewCamera = new GameObject ("Preview Camera").AddComponent<Camera> ();
                
                previewCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            
            UnityEditorInternal.ComponentUtility.CopyComponent (Camera.main);
            UnityEditorInternal.ComponentUtility.PasteComponentValues (previewCamera);

            previewCamera.depth = -100;
            return previewCamera;
        }
#endif
        
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
#if UNITY_EDITOR
            if (Editor_UpdatePreview ())
                return;
#endif
            
            if (mainCamera == false || virtualCameras.Count == 0)
                return;

            var mainCamTransform = mainCamera.transform;
            var virtualCamera = virtualCameras.Peek ();
            var smooth = virtualCamera.SmoothValue;

            var (vCamPosition, vCamLookDirection) = virtualCamera.UpdatePositionAndRotation ();

            var framePosition = LerpPosition (
                mainCamTransform.position, in vCamPosition, smooth, MainCamera.PositionBlendSpeed);
            
            var frameRotation = LerpRotation (
                mainCamTransform.forward, in vCamLookDirection, smooth, MainCamera.RotationBlendSpeed);

            mainCamTransform.SetPositionAndRotation (framePosition, frameRotation);
            virtualCamera.PushState (framePosition, frameRotation);
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
            
            return MusesUtility.GetCameraLookRotation (lookDirection);
        }

        private static bool Editor_UpdatePreview ()
        {
            if (Application.isPlaying)
            {
                if (previewCamera && previewCamera.depth != -100)
                {
                    previewCamera.depth = -100;
                }

                return false;
            }
            
            if (SoloCamera)
            {
                GerOrCreatePreviewCamera ();

                if (previewCamera.depth != 100)
                {
                    previewCamera.depth = 100;
                    Debug.Log ("ENABLE PREVIEW");
                }

                var (pos, lookDirection) = SoloCamera.UpdatePositionAndRotation ();
                previewCamera.transform.SetPositionAndRotation (pos, MusesUtility.GetCameraLookRotation (lookDirection));
                return true;
            }

            if (previewCamera && previewCamera.depth != -100)
            {
                Debug.Log ("DISABLE PREVIEW");
                previewCamera.depth = -100;
            }

            return false;
        }
    }
}