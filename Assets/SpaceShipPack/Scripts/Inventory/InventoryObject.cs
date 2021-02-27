using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{

    public List<InventorySlot> inventoryContainer = new List<InventorySlot>();
    public void AddItem(ItemObject _item, int _amount) {
        bool hasItem = false;

        for (int i = 0 ; i < inventoryContainer.Count; i++) {
            if (inventoryContainer[i].item == _item) {
                inventoryContainer[i].AddAmount(_amount);
                hasItem = true;
                break;
            }
        }

        if (!hasItem) {
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