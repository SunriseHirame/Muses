using UnityEditor;
using UnityEngine;

namespace Hirame.Muses.Editor
{
    [CustomEditor (typeof (VirtualCamera))]
    public class VirtualCameraEditor : UnityEditor.Editor
    {
        private Transform vCamTransform;
        private VirtualCamera vCam;
        
        private void OnEnable ()
        {
            vCam = target as VirtualCamera;
            vCamTransform = vCam.transform;

            CameraSystem.SoloCamera = vCam;
        }

        private void OnDisable ()
        {
            CameraSystem.SoloCamera = null;
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();
            
            using (var scope = new EditorGUI.ChangeCheckScope ())
            {
                DrawPropertiesExcluding (serializedObject, "m_Script");
                
                if (scope.changed)
                {
                    serializedObject.ApplyModifiedProperties ();
                    vCam.SetDirty ();
                }
            }
        }

        [DrawGizmo (GizmoType.Selected)]
        private static void OnDrawGizmos (VirtualCamera vCam, GizmoType gizmoType)
        {
            var cam = vCam.GetComponent<Camera> ();
            if (cam == false)
                return;
            
            var vCamTransform = vCam.transform;
            
            Gizmos.matrix = vCamTransform.localToWorldMatrix;
            Gizmos.DrawFrustum (
                Vector3.zero, cam.fieldOfView, 
                cam.farClipPlane, cam.nearClipPlane, cam.aspect);
        }
    }

}
