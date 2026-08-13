using UnityEngine;

namespace TNT
{
    public class PlayerInteract : MonoBehaviour
    {
        public LayerMask interactLayer;
        public int interactRange = 2;

        private PlayerInputs inputs;
        private PlayerCamera playerCam;

        private void Start()
        {
            inputs = GetComponent<PlayerInputs>();
            playerCam = GetComponentInChildren<PlayerCamera>();
        }

        private void Update()
        {
            Ray interactRay = new(playerCam.mainCamera.transform.position, playerCam.mainCamera.transform.forward);
            RaycastHit interactHit;
            if (Physics.Raycast(interactRay,out interactHit, interactRange, interactLayer))
            {
                if (interactHit.transform.TryGetComponent(out IInteractable interactable))
                {
                    if (inputs.interact.pressed)
                    {
                        inputs.interact.pressed = false;
                        interactable.OnInteract(gameObject);
                    }
                }
            }
        }
    }

    public interface IInteractable
    {
        public string InteractMSG { set; get; }
        public void OnInteract(GameObject interactPoss);
    }
}
