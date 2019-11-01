using UnityEngine;

namespace Hirame.Muses
{
    public class CameraCutTrigger : MonoBehaviour
    {
        [SerializeField] private VirtualCamera virtualCamera;
        [SerializeField] private int activateWithPriority = 500;
        
        private void OnTriggerEnter (Collider other)
        {
            Debug.Log ("CAM TRIGGER ENTER");
            virtualCamera.Priority = activateWithPriority;
        }

        private void OnTriggerExit (Collider other)
        {
            Debug.Log ("CAM TRIGGER EXIT");
            virtualCamera.Priority = 0;
        }
    }

}
