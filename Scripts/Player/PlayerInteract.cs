using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 2;
    public LayerMask interactLayers;

    internal PlayerInventory playerInventory;
    internal PlayerCamera playerCamera;
    internal PlayerInputs playerInput;
    internal PlayerUI playerUI;

    private float holdTimer = 0;
    private float hoveredTimer = 0;
    private InteractableObject currentInteractable;
    private InteractableObject previousInteractable;

    private void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerCamera = GetComponentInChildren<PlayerCamera>();
        playerInput = GetComponent<PlayerInputs>();
        playerUI = GetComponent<PlayerUI>();
    }

    private void Update()
    {
        //if (Cursor.lockState != CursorLockMode.Locked) return;
        Ray interactRay = playerCamera.mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(interactRay, out RaycastHit hit, interactRange, interactLayers))
        {
            var interact = hit.transform.GetComponent<InteractableObject>();

            if (interact != null)
            {
                if (interact != previousInteractable)
                {
                    currentInteractable = interact;
                }
                else
                {
                    hoveredTimer += Time.deltaTime;
                    if(hoveredTimer > 1f)
                    {
                        hoveredTimer = 0;
                        previousInteractable = null;
                    }
                }
            }
            else
            {
                if (currentInteractable) previousInteractable = currentInteractable;
                currentInteractable = null;
                hoveredTimer = 0;
            }
        }
        else
        {
            currentInteractable = null;
            previousInteractable = null;
            hoveredTimer = 0;
        }

        if (currentInteractable != null)
        {
            if (playerInput.interact.hold)
            {
                playerUI.UpdateInteractUI(currentInteractable.interactMsg, holdTimer, currentInteractable.interactTime, true, true);
                holdTimer += Time.deltaTime;
                if (holdTimer >= currentInteractable.interactTime)
                {
                    currentInteractable.Interact(this);
                    holdTimer = 0;
                    if (currentInteractable) previousInteractable = currentInteractable;
                    currentInteractable = null;
                }
            }
            else if (playerInput.limitedInteract.hold)
            {
                playerUI.UpdateInteractUI(currentInteractable.interactMsg, holdTimer, currentInteractable.limitedInteractTime, true, true);
                holdTimer += Time.deltaTime;
                if (holdTimer >= currentInteractable.limitedInteractTime)
                {                    
                    currentInteractable.LimitedInteract(this);
                    holdTimer = 0;
                    if (currentInteractable) previousInteractable = currentInteractable;
                    currentInteractable = null;
                }
            }
            else
            {
                playerUI.UpdateInteractUI(string.Empty, 0, 1, false, false);
                holdTimer = 0;
            }
        }
        else
        {
            holdTimer = 0;
            playerInput.interact.hold = false;
            playerInput.limitedInteract.hold = false;
            playerUI.UpdateInteractUI(string.Empty, 0, 1, false, false);
        }
    }
}
