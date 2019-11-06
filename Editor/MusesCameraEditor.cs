using UnityEditor;

namespace Hirame.Muses.Editor
{
    public class MusesCameraEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();
            
            using (var scope = new EditorGUI.ChangeCheckScope ())
            {
                DrawPropertiesExcluding (serializedObject, "m_Script");

                if (scope.changed)
                    serializedObject.ApplyModifiedProperties ();
            }
        }
    }

}