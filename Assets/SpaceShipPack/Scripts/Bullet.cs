using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 100f;
    public int DamageAmount = 10;
    private Vector3 shootDir;
    private new Rigidbody rigidbody;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }
    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        //transform.right = shootDir; // ???
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        // Move in given direction
        rigidbody.AddForce(shootDir * Time.deltaTime * moveSpeed * 150);
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Find Ship component on collider.
        // If there is no Ship component, then try to get it from parent.
        // TODO: Create a new Bullet script that will be inherited by this and Rocket
        // so I don't have to repeat OnTriggerEnter
        HealthData healthData = collider.gameObject.GetComponent<HealthData>();
        if (healthData == null && collider.gameObject.transform.parent != null) {
            healthData = collider.gameObject.transform.parent.GetComponent<HealthData>();
        }
        if ( healthData != null )
        {
            // target hit
            healthData.Damage(DamageAmount);
            Destroy(gameObject);
        }
    }
}
