using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerController : MonoBehaviour
{
    Dictionary<int, EnemyPointer> pointers = new Dictionary<int, EnemyPointer>(); // int: ID of enemy, EnemyPointer: pointer instance
    public GameObject pfPointer;

    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies) {
            int enemyID = enemy.GetInstanceID();
            // If we have already instantiated a pointer for this enemy, then
            // only modify it.
            // If we don't have one, then instantiated it.
            if (pointers.ContainsKey(enemyID)) {
                EnemyPointer pointer = pointers[enemyID];
                pointer.SetTarget(enemy.transform.position);
            
            } else {
                // Instantiate the pointer (which will be the one putting itself to the target's position)
                // Then putting it to be this object's child (because it should be in canvas)
                // And add to the dictionary to know we have this pointer (later for not showing dead enemy)
                GameObject pointerObj = Instantiate(pfPointer, transform.position, Quaternion.identity);
                EnemyPointer pointer = pointerObj.GetComponent<EnemyPointer>();
                pointerObj.transform.SetParent(transform);
                pointer.SetTarget(enemy.transform.position);
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
