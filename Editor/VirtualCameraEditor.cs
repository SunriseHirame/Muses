using UnityEditor;
using UnityEngine;

namespace Hirame.Muses.Editor
{
    [CustomEditor (typeof (VirtualCamera))]
    public class VirtualCameraEditor : UnityEditor.Editor
    {
        private Transform vCamTransform;
        private VirtualCamera vCam;

        private Camera previewCam;
        
        private void OnEnable ()
        {
            vCam = target as VirtualCamera;
            vCamTransform = vCam.transform;

            CreatePreviewCamera ();
        }

        private void OnDisable ()
        {
            if (previewCam)
                previewCam.enabled = false;
        }

        private void CreatePreviewCamera ()
        {
            UnityEditorInternal.ComponentUtility.CopyComponent (Camera.main);
            previewCam = vCam.GetComponent<Camera> ();
            
            if (previewCam == null)
            {
                previewCam = vCam.gameObject.AddComponent<Camera> ();
            }
          
            UnityEditorInternal.ComponentUtility.PasteComponentValues (previewCam);
            previewCam.depth = -90;
            previewCam.enabled = true;
            previewCam.hideFlags = HideFlags.HideAndDontSave;
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
                    vCam.UpdateCamera ();
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
