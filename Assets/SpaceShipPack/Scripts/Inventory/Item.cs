using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemObject item;

    public void OnTriggerEnter(Collider other) {
        InventoryObject inventory = null;
        // TODO: also try to get the inventory of AI
        ShipController shipController = other.GetComponent<ShipController>();
        if (shipController != null) {
            inventory = shipController.inventory;
        } else {
            shipController = other.transform.parent.gameObject.GetComponent<ShipController>();
        }
        if (shipController != null) {
            inventory = shipController.inventory;
        } else {
            return; // if not even parent has a shipcontroller
        }
        if (inventory == null) return;

        inventory.AddItem(item, 1);
        Destroy(gameObject);
    }
}
