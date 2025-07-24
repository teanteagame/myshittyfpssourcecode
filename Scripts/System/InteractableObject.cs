using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    public string interactMsg = "Do this";
    public float interactTime = 0.5f;
    public float limitedInteractTime = 1;

    public UnityEvent onInteract;
    public UnityEvent onLimitedInteract;

    public virtual void Interact(PlayerInteract player)
    {
        onInteract.Invoke();
    }

    public virtual void LimitedInteract(PlayerInteract player)
    {
        onLimitedInteract.Invoke();
    }

    private void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");      
    }
}
