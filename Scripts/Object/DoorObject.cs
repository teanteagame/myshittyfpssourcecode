using UnityEngine;

public class DoorObject : MonoBehaviour
{
    public Animator doorAnim;

    public bool isOpen;
    public bool isLocked;
    public bool isJammed;

    public void DoorAction(int side)
    {
        if (isJammed) return;
        if (isLocked) return;

        if (isOpen)
        {         
            isOpen = false;
        }
        else
        {
            isOpen = true;
        }

        doorAnim.SetInteger("Side", side);
        doorAnim.SetBool("Open", isOpen);
    }

    public void DoorLock()
    {
        if (!isLocked)
        {
            isLocked = true;
        }
        else
        {
            isLocked = false;
        }
    }
}