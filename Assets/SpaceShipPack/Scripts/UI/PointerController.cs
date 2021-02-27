using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
    Dictionary<int, EnemyPointer> pointers = new Dictionary<int, EnemyPointer>(); // int: ID of enemy, EnemyPointer: pointer instance
    public GameObject pfPointer;
    public Sprite basicPointerSprite;
    public Sprite targetedPointerSprite;
    private GameObject currentTarget;
    private ShipController playerShipController;

    private void Awake() {
        playerShipController = GameObject.FindGameObjectWithTag("PlayerTag").transform.parent.GetComponent<ShipController>();
    }
    void Update()
    {
        if (playerShipController.currentTarget != null) currentTarget = playerShipController.currentTarget;
        else currentTarget = null; // In case currentTarget of ship changes during gameplay

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies) {
            int enemyID = enemy.GetInstanceID();
            // If we have already instantiated a pointer for this enemy, then
            // only modify it.
            // If we don't have one, then instantiated it.
            if (pointers.ContainsKey(enemyID)) {
                EnemyPointer pointer = pointers[enemyID];
                pointer.SetTarget(enemy.transform.position);
                // Set its sprite.
                Image pointerImage = pointer.GetComponent<Image>();
                // NOTE: as currentTarget, shipController has the gameobject with Ship,
                // but here we save the instanceID of the EnemyTag child
                if (currentTarget != null && currentTarget.transform.Find("EnemyTag").gameObject.GetInstanceID() == enemyID) {
                    pointerImage.sprite = targetedPointerSprite;
                } else {
                    pointerImage.sprite = basicPointerSprite;
                }
            
            } else {
                // Instantiate the pointer (which will be the one putting itself to the target's position)
                // Then putting it to be this object's child (because it should be in canvas)
                // And add to the dictionary to know we have this pointer (later for not showing dead enemy)
                GameObject pointerObj = Instantiate(pfPointer, transform.position, Quaternion.identity);
                EnemyPointer pointer = pointerObj.GetComponent<EnemyPointer>();
                pointerObj.transform.SetParent(transform);
                pointer.SetTarget(enemy.transform.position);
                // Set its sprite
                Image pointerImage = pointer.GetComponent<Image>();
                // NOTE: as currentTarget, shipController has the gameobject with Ship,
                // but here we save the instanceID of the EnemyTag child
                if (currentTarget != null && currentTarget.transform.Find("EnemyTag").gameObject.GetInstanceID() == enemyID) {
                    pointerImage.sprite = targetedPointerSprite;
                } else {
                    pointerImage.sprite = basicPointerSprite;
                }
                pointers.Add(enemyID, pointer);
            }
        }

        // If there are different amounts of pointers than enemies, then reset
        // dictionary and destroy each pointer, so that later we will recreate them
        if (enemies.Length != pointers.Count) {
            foreach(EnemyPointer pointer in pointers.Values) {
                Destroy(pointer.gameObject);
            }
            pointers.Clear();
        }
    }
}
