using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
    Dictionary<int, EnemyPointer> pointers = new Dictionary<int, EnemyPointer>(); // int: ID of enemy, EnemyPointer: pointer instance
    public GameObject pfShipPointer;
    public Sprite shipPointerSprite;
    public Sprite targetedShipPointerSprite;
    public Sprite itemPointerSprite;
    private GameObject currentTarget;
    private ShipController playerShipController;
    public string collectableTag = "Collectable";

    private void Awake() {
        playerShipController = GameObject.FindGameObjectWithTag("PlayerTag").transform.parent.GetComponent<ShipController>();
    }
    void Update()
    {
        setShipPointers();

    }

    private void setShipPointers() {
        if (playerShipController.currentTarget != null) currentTarget = playerShipController.currentTarget;
        else currentTarget = null; // In case currentTarget of ship changes during gameplay

        // Loop through enemies and collectables that exist and
        // create pointers for each one of them
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies) {
            setPointer(enemy, shipPointerSprite, pointerIfTargeted: targetedShipPointerSprite, pointerColor: Color.red);
        }
        GameObject[] collectables = GameObject.FindGameObjectsWithTag(collectableTag);
        foreach (GameObject collectable in collectables) {
            setPointer(collectable, itemPointerSprite, Color.yellow);
        }
        // Destroy pointers that no longer exist.
        if (enemies.Length + collectables.Length != pointers.Count) {
            foreach(EnemyPointer pointer in pointers.Values) {
                Destroy(pointer.gameObject);
            }
            pointers.Clear();
        }
    }

    private void setPointer(GameObject objectToPointAt, Sprite basicPointer, Color pointerColor, Sprite pointerIfTargeted=null) {
        int objectToPointAtID = objectToPointAt.GetInstanceID();
        EnemyPointer pointer;

        // If we have already instantiated a pointer for this object, then
        // only modify it. If we don't have one, then instantiate it.
        if (pointers.ContainsKey(objectToPointAtID)) pointer = pointers[objectToPointAtID];
        else {
            // Instantiate the pointer (which will be the one putting itself to the target's position)
            // Then putting it to be this object's child (because it should be in canvas)
            // And add to the dictionary to know we have this pointer (later for not showing dead enemy)
            GameObject pointerObj = Instantiate(pfShipPointer, transform.position, Quaternion.identity);
            pointer = pointerObj.GetComponent<EnemyPointer>();
            pointerObj.transform.SetParent(transform);
            pointers.Add(objectToPointAtID, pointer);
        }

        pointer.SetTarget(objectToPointAt.transform.position);
        Image pointerImage = pointer.GetComponent<Image>();
        pointerImage.color = pointerColor;
        
        // NOTE: as currentTarget, shipController has the gameobject with Ship,
        // but here we save the instanceID of the EnemyTag child
        if (currentTarget != null && currentTarget.transform.Find("EnemyTag").gameObject.GetInstanceID() == objectToPointAtID) {
            pointerImage.sprite = pointerIfTargeted;
        } else {
            pointerImage.sprite = basicPointer;
        }
    }

}
