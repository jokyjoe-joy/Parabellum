using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float backwardSpeed = 10f;
    public float RotationSpeed = 5f;
    public float TurnSpeed = 10f;
    public float MaxSpeed = 100f;
    public GameObject explosionVFX;
    public float explosionPower = 100;
    public float explosionRadius = 20;
    public float explosionScale = 1;

    [HideInInspector] public Vector3 currentVelocity;
    [HideInInspector] public int HEALTH = 100;
    private new Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        currentVelocity = rigidbody.velocity;
    }

    public void ThrustForward(float speed)
    {
        rigidbody.AddForce(transform.forward * forwardSpeed * 100 * Time.deltaTime * speed);
    }

    public void ThrustBackwards(float speed)
    {
        rigidbody.AddForce(-transform.forward * backwardSpeed * 100 * Time.deltaTime * speed);
    }

    public void TurnLeft(float speed)
    {
        rigidbody.AddRelativeTorque(speed * TurnSpeed * 50 * Time.deltaTime, 0, 0);
    }

    public void TurnRight(float speed)
    {
        rigidbody.AddRelativeTorque(0, speed * TurnSpeed * 50 * Time.deltaTime, 0);
    }

    public void RollLeft(float speed)
    {
        rigidbody.AddRelativeTorque(0, 0, RotationSpeed * speed * Time.deltaTime * 100);
    }
    public void RollRight(float speed)
    {
        rigidbody.AddRelativeTorque(0, 0, -RotationSpeed * speed * Time.deltaTime * 100);
    }

    public void Stabilise(float speed)
    {
        rigidbody.AddForce(-rigidbody.velocity * forwardSpeed * 10 * Time.deltaTime);
        rigidbody.AddTorque(-rigidbody.angularVelocity.normalized * 200 * RotationSpeed * Time.deltaTime);
    }

    public void Damage(int amount)
    {
        HEALTH -= amount;
        Debug.Log(HEALTH);
        if (HEALTH <= 0)
        {
            Explode();
        }
    }

    private void Explode() {
        /* 
        Checking nearby colliders and adding explosion force to their rigidbodies (if they have)
        Also adding rigidbodies to children and then explosion force to them
        Renaming children to SCRAPE, then detaching them, then destroying gameObject
        */
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
        explosion.transform.localScale = transform.localScale * explosionScale;
        Destroy(explosion, 2f);

        foreach(Transform child in transform) {
            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            child.gameObject.name = "SCRAP";
        }

        explosionRadius *= transform.localScale.x;

        foreach (Collider hit in colliders) {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null) {
                rb.AddExplosionForce(explosionPower, transform.position, explosionRadius, 0);
            }
        }

        // Destroy attached tags if there are any.
        foreach(Transform child in transform) {
            if (child.gameObject.CompareTag("Enemy")) {
                Destroy(child.gameObject);
            }
        }

        transform.DetachChildren();
        Destroy(gameObject);
    }

}
