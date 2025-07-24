using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    [HideInInspector] public float vertical, horizontal, mouseX, mouseY, leanAxis, mouseRoll;
    [HideInInspector] public InputAction up, down, right, left;
    [HideInInspector] public InputAction crouch, jump, run, leanL, leanR;
    [HideInInspector] public InputAction fire, aim, reload, changeFireMode, interact, limitedInteract, drop;
    [HideInInspector] public InputAction inventory, escape;

    public float lerpSpeed = 4;
    public bool instantBrake = true;
    public InputData data;

    public void Update()
    {
        GetButtons();
        MovementInputs();
    }

    public void GetButtons()
    {
        KeyAction(up, "Up");
        KeyAction(down, "Down");
        KeyAction(right, "Right");
        KeyAction(left, "Left");
        KeyAction(crouch, "Crouch");
        KeyAction(jump, "Jump");
        KeyAction(run, "Run");
        KeyAction(leanR, "Lean R");
        KeyAction(leanL, "Lean L");
        KeyAction(fire, "Fire");
        KeyAction(aim, "Aim");
        KeyAction(reload, "Reload");
        KeyAction(changeFireMode, "Change Fire Mode");
        KeyAction(interact, "Interact");
        KeyAction(limitedInteract, "Limited Interact");
        KeyAction(drop, "Drop");
        KeyAction(inventory, "Inventory");
        KeyAction(escape, "Escape");        
    }

    void MovementInputs()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        if (up.hold)
        {
            vertical = Mathf.Lerp(vertical, 1, lerpSpeed * Time.deltaTime);
        }
        else if (down.hold)
        {
            vertical = Mathf.Lerp(vertical, -1, lerpSpeed * Time.deltaTime);
        }
        else
        {
            vertical = instantBrake ? 0 : Mathf.Lerp(vertical, 0, lerpSpeed * Time.deltaTime * 10);
        }

        if (right.hold)
        {
            horizontal = Mathf.Lerp(horizontal, 1, lerpSpeed * Time.deltaTime);
        }
        else if (left.hold)
        {
            horizontal = Mathf.Lerp(horizontal, -1, lerpSpeed * Time.deltaTime);
        }
        else
        {
            horizontal = instantBrake ? 0 : Mathf.Lerp(horizontal, 0, lerpSpeed * Time.deltaTime * 10);
        }        

        if (leanR.hold)
        {
            leanAxis = Mathf.Lerp(leanAxis, run.hold ? 0 : 1, Time.deltaTime * lerpSpeed);
        }
        else if (leanL.hold)
        {
            leanAxis = Mathf.Lerp(leanAxis, run.hold ? 0 : -1, Time.deltaTime * lerpSpeed);
        }
        else
        {
            leanAxis = Mathf.Lerp(leanAxis, 0, Time.deltaTime * lerpSpeed * 1.5f);
        }
    }

    public void KeyAction(InputAction input, string key)
    {
        if (!input.pressed) input.pressed = data.GetKeyDown(key);
        if (!input.pressed) input.unpressed = data.GetKeyUp(key);
        input.hold = data.GetKey(key);
    }
}

[System.Serializable]
public class InputAction
{
    public bool pressed;
    public bool unpressed;
    public bool hold;    
}