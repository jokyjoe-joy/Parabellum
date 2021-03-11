using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionMember : MonoBehaviour
{
    public string factionName;
    public FactionDatabase factionDB;

    private void Awake()
    {
        factionDB = FactionManager.Instance.factionDB;
        // check if our faction even exists (in case we have a designing error)
        bool factionExists = false;
        for (int i = 0; i < factionDB.factions.Count; i++)
        {
            if (factionDB.factions[i].name == factionName)
            {
                factionExists = true;
                break;
            }
        }
        if (!factionExists) Debug.LogError("The specified faction doesn't exist!");
    }

    void Update()
    {
        
    }
}
