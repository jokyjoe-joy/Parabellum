using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FactionRelation
{
    public string factionName;
    [Tooltip("Below 0 can attack the faction members, above 0 can help the faction members.")]
    [Range(-100, 100)] public int relation;
}

[System.Serializable]
public class Faction
{
    public string name;
    public string description;
    [Tooltip("In the UI faction members will be shown in this color.")]
    public Color color = Color.yellow;
    [Tooltip("If there is no relationship set for a specific faction, it will default to this value.")]
    [Range(-100, 100)] public int defaultRelationship;
    public FactionRelation[] relationships;
    
}

[CreateAssetMenu(fileName = "New Faction Database", menuName = "Factions System/Faction Database")]
public class FactionDatabase : ScriptableObject
{
    public List<Faction> factions = new List<Faction>();
}
