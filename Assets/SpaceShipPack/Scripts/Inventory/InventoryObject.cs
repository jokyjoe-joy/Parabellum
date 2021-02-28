using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> inventoryContainer = new List<InventorySlot>();
    public void AddItem(ItemObject _item, int _amount) {
        bool hasItem = false;

        // Loop through inventory and check if we have item
        // If we already have the item and it is stackable, add amount to it, then break;
        for (int i = 0 ; i < inventoryContainer.Count; i++) {
            if (inventoryContainer[i].item == _item) {
                hasItem = true;
                if (_item.stackable) {
                    inventoryContainer[i].AddAmount(_amount);
                    break;
                };
            }
        }

        // If we don't have the item or we do and it is not stackable, add it to inventory
        if (!hasItem || hasItem && !_item.stackable) {
            inventoryContainer.Add(new InventorySlot(_item, _amount));
        }
    }

}

[System.Serializable]
public class InventorySlot {
    public ItemObject item;
    public int amount;
    public InventorySlot(ItemObject _item, int _amount) {
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value) {
        amount += value;
    }
}