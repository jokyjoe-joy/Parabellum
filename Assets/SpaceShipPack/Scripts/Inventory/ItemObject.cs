using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Food,
    Equipment,
    Default
}

public abstract class ItemObject : ScriptableObject {
    
    public string itemName;
    [TextArea(15,20)]
    public string itemDescription;
    public Sprite itemSprite;
    public bool stackable = true;
    [Tooltip("Prefab that will be instantiated when item is dropped or equipped.")]
    public GameObject prefab;
    public ItemType type;

}