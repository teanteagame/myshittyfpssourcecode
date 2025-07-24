using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFPSItemManager : MonoBehaviour
{
    public FPSItem currentActiveItem;
    public FistFPSItem fistObject; /// this will be the current if there is no weapon equip

    [Header("Weapon Loadout")]
    public FPSItem primaryWeapon;
    public FPSItem secondaryWeapon;
    public FPSItem handgunWeapon;
    public FPSItem meleeWeapon;
    public FPSItem throwableWeapon;

    public ItemSway itemSway;

    private PlayerInputs inputs;

    private void Start()
    {
        inputs = GetComponent<PlayerInputs>();
    }

    private void Update()
    {
        if (inputs != null)
        {
            if (currentActiveItem == null) return;
            CurrentFPSItemInput();
        }
    }

    private void CurrentFPSItemInput()
    {
        if (inputs.fire.pressed)
        {
            currentActiveItem.OnFirePress();
            Debug.Log("Fire pressed");
            inputs.fire.pressed = false;
        }
        else if (inputs.fire.unpressed)
        {
            currentActiveItem.OnFireRelease();
            inputs.fire.unpressed = false;
        }
        else if (inputs.fire.hold)
        {
            currentActiveItem.OnFireHold();
        }

        if (inputs.reload.pressed)
        {
            currentActiveItem.OnReloadPress();
            inputs.reload.pressed = false;
        }
        else if (inputs.reload.unpressed)
        {
            currentActiveItem.OnReloadRelease();
            inputs.reload.unpressed = false;
        }
        else if (inputs.reload.hold)
        {
            currentActiveItem.OnReloadHold();
        }

        if (inputs.aim.pressed)
        {
            currentActiveItem.OnAimPress();
            inputs.aim.pressed = false;
        }
        else if (inputs.aim.unpressed)
        {
            currentActiveItem.OnAimRelease();
            inputs.aim.unpressed = false;
        }
        else if (inputs.aim.hold)
        {
            currentActiveItem.OnAimHold();
        }

        if (inputs.changeFireMode.pressed) 
        {
            currentActiveItem.OnChangeFireModePress();
            inputs.changeFireMode.pressed = false;
        }
        else if (inputs.changeFireMode.unpressed)
        {
            currentActiveItem.OnChangeFireModeRelease();
            inputs.changeFireMode.unpressed = false;
        }
        else if(inputs.changeFireMode.hold)
        {
            currentActiveItem.OnChangeFireModeHold();
        }
    }
}
