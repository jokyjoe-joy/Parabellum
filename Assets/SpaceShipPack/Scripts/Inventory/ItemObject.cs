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

    public GameObject prefab;
    public ItemType type;

}