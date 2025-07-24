using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager instance;
    public List<FactionObject> objectFactions;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        //If we are not the master client then destroy this
        /*
        if (!PhotonNetwork.IsMasterClient)
        {
            Destroy(gameObject);
        }
        */
    }

    public void AssignObjectFaction(FactionObject newObject)
    {
        //if not master client, return

        objectFactions.Add(newObject);
        //Tell all object faction to update its thing

        if (objectFactions.Count <= 0) return;
        foreach (FactionObject faction in objectFactions)
        {
            /*
            if (faction.GetComponent<AIBase>()) //If this was on an AI
            {
                faction.GetComponent<AIBase>().DecideNewTargets();
            }
            */
        }
    }

    public void RemoveObjectFaction(FactionObject removeObject)
    {
        //if not master client, return


        if (objectFactions.Contains(removeObject)) objectFactions.Remove(removeObject);
        //Tell all object faction to update its thing

        if (objectFactions.Count <= 0) return;
        foreach (FactionObject faction in objectFactions)
        {
            /*
            if (faction.GetComponent<AIBase>()) //If this was on an AI
            {
                faction.GetComponent<AIBase>().DecideNewTargets();
            }
            */
        }
    }
}
