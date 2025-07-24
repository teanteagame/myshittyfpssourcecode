using UnityEngine;

public class DoorInteract : InteractableObject
{
    public DoorObject doorParent;
    public int doorSideID;

    void Update()
    {
        interactMsg = doorParent.isOpen ? "Close" : "Open";
    }

    public override void Interact(PlayerInteract player)
    {
        doorParent.DoorAction(doorSideID);
    }

    public override void LimitedInteract(PlayerInteract player)
    {
        doorParent.DoorLock();
    }
}
