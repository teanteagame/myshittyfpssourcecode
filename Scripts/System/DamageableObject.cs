using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    public virtual void TakeDamage(float damage, Vector3 fromPos)
    {
        Debug.Log("Take " + damage + " from " + fromPos);
    }
}
