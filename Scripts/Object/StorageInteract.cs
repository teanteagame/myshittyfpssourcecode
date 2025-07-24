
public class StorageInteract : InteractableObject
{
    public InventoryObject parentInventory;

    public override void Interact(PlayerInteract player)
    {
        base.Interact(player);
    }

    public override void LimitedInteract(PlayerInteract player)
    {
        base.LimitedInteract(player);
    }
}
