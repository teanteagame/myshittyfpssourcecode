using UnityEngine;

public class FactionObject : MonoBehaviour
{
    [Header("Faction Info")]
    public int factionNumber = 1;
    public Vector3 detectOffset;

    public virtual void Start()
    {
        FactionManager.instance.AssignObjectFaction(this);
        Init();
    }

    public virtual void Init()
    {

    }

    public virtual void TakeDamage(float amount, Vector3 fromPos) { }

    public void Die()
    {
        FactionManager.instance.RemoveObjectFaction(this);
    }

    public void OnDestroy()
    {
        FactionManager.instance.RemoveObjectFaction(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + detectOffset, 0.3f);
    }
}
