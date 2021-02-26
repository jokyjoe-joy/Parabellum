using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracture : MonoBehaviour
{
    [Tooltip("\"Fractured\" is the object that this will break into")]
    public GameObject fractured;
    private HealthData healthData;

    private void Awake() {
        healthData = transform.GetComponent<HealthData>();
    }

    private void Update() {
        if (healthData.health <= 0) {
            FractureObject();
        }
    }

    public void FractureObject()
    {
        GameObject fracturedInstance = Instantiate(fractured, transform.position, transform.rotation); //Spawn in the broken version
        fracturedInstance.transform.localScale = transform.localScale;
        fracturedInstance.name = "SCRAPE"; // select it for garbage collection
        Destroy(fracturedInstance, 20.0f);
        foreach(Transform asteroidPart in fracturedInstance.transform) {
            Rigidbody rb = asteroidPart.GetComponent<Rigidbody>();
            rb.useGravity = false;
           
            rb.AddExplosionForce(Random.Range(10,100), transform.position, 50);
            // TODO: This is added for performance, as there are tons of instantiated
            // objects here, so maybe we should think about something better?
            // Get random boolean, then destroy it if true.
            if (Random.value > 0.2f) {
                Destroy(asteroidPart.gameObject);
            }
        }
        Destroy(gameObject); //Destroy the object to stop it getting in the way
    }
}
