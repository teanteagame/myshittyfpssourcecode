using UnityEngine;

public class PlayerManager : FactionObject
{
    public bool mouseLocked = true;
    public bool inventoryOpen = false;

    private PlayerUI ui;
    private PlayerInputs inputs;

    public override void Start()
    {
        ui = GetComponent<PlayerUI>();
        inputs = GetComponent<PlayerInputs>();
    }

    private void Update()
    {
        Cursor.lockState = mouseLocked ? CursorLockMode.Locked : CursorLockMode.None;

        if (inputs.inventory.pressed)
        {
            if (inventoryOpen)
            {
                inventoryOpen = false;
                mouseLocked = true;        
                ui.CloseInventory();
            }
            else
            {
                inventoryOpen= true;
                mouseLocked = false;
                ui.OpenInventory();
            }        
            inputs.inventory.pressed = false;
        }      
    }
}
