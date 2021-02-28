using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableToInventory : MonoBehaviour
{
    public ItemObject item;
    private Transform closeObjectTransform = null;
    public float fromDistanceToAddToInventory = 10f;
    public float maxSpeed = 0.8f;
    private InventoryObject inventory = null;
    
    private void Update() {
        if (closeObjectTransform != null) {
            // If collector is close enough then add the item to its inventory
            // Otherwise move closer to the object
            float distanceFromObject = Vector3.Distance(transform.position, closeObjectTransform.position);
            if (distanceFromObject < fromDistanceToAddToInventory) {
                inventory.AddItem(item, 1);
                Destroy(gameObject);
            } else {
                float step = Mathf.Clamp(30 * Time.deltaTime - distanceFromObject / 50, 0, maxSpeed);
                if (step > 0) transform.position = Vector3.MoveTowards(transform.position, closeObjectTransform.position, step);
            }
        }
    }

    public void OnTriggerEnter(Collider other) {
        // Check if this object has an inventory, if it doesn't: return.
        // TODO: also try to get the inventory of AI
        ShipController shipController = other.transform.GetComponent<ShipController>();
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

        closeObjectTransform = other.transform;
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform == closeObjectTransform) {
            closeObjectTransform = null;
        }
    }
}
