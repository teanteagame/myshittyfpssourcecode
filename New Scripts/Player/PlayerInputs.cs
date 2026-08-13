using UnityEngine;

namespace TNT
{
    public class PlayerInputs : MonoBehaviour
    {
        [HideInInspector] public float vertical, horizontal, mouseX, mouseY, mouseRoll;
        [HideInInspector] public InputAction up, down, right, left;
        [HideInInspector] public InputAction crouch, jump, run;
        [HideInInspector] public InputAction fire, aim, reload, interact, drop;
        [HideInInspector] public InputAction inventory, escape;

        public float lerpSpeed = 4;
        public bool instantBrake = true;
        /// <summary>
        /// public InputData data;
        /// </summary>

        public void Update()
        {
            GetButtons();
            MovementInputs();
        }

        public void GetButtons()
        {
            KeyAction(up, KeyCode.W);
            KeyAction(down, KeyCode.S);
            KeyAction(right, KeyCode.D);
            KeyAction(left, KeyCode.A);
            KeyAction(crouch, KeyCode.C);
            KeyAction(jump, KeyCode.Space);
            KeyAction(run, KeyCode.LeftShift);         
            KeyAction(fire, KeyCode.Mouse0);
            KeyAction(aim, KeyCode.Mouse1);
            KeyAction(reload, KeyCode.R);       
            KeyAction(interact,KeyCode.E);            
            KeyAction(drop, KeyCode.G);
            KeyAction(inventory, KeyCode.Tab);
            KeyAction(escape, KeyCode.Escape);
        }

        void MovementInputs()
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            mouseRoll = Input.GetAxis("Mouse ScrollWheel");
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
        }

        public void KeyAction(InputAction input, KeyCode key)
        {
            input.pressed = Input.GetKeyDown(key);
            input.unpressed = Input.GetKeyUp(key);
            input.hold = Input.GetKey(key);
        }
    }

    [System.Serializable]
    public class InputAction
    {
        public bool pressed;
        public bool unpressed;
        public bool hold;
    }
}
