using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [HideInInspector] public WeaponSystem weaponSystem;
    [HideInInspector] public Vector3 currentVelocity;
    [HideInInspector] public new Rigidbody rigidbody;
    [HideInInspector] public HealthData healthData;
    [HideInInspector] public UnityEvent onTargeted;

    private void Awake()
    {
        healthData = GetComponent<HealthData>();
        rigidbody = GetComponent<Rigidbody>();
        weaponSystem = GetComponent<WeaponSystem>();
    }
    
    private void Update() 
    {
        if (healthData.health <= 0) Explode();
    }

    private void FixedUpdate()
    {
        currentVelocity = rigidbody.velocity;
    }

    public void ThrustForward()
    {
        if (currentVelocity.magnitude < MaxSpeed) rigidbody.AddForce(transform.forward * forwardSpeed * 100 * Time.deltaTime);

    }

    public void ThrustBackwards()
    {
        rigidbody.AddForce(-transform.forward * backwardSpeed * 100 * Time.deltaTime );
    }

    public void LookUpDown(float speed)
    {
        rigidbody.AddRelativeTorque(speed * TurnSpeed * 50 * Time.deltaTime, 0, 0);
    }

    public void LookLeftRight(float speed)
    {
        rigidbody.AddRelativeTorque(0, speed * TurnSpeed * 50 * Time.deltaTime, 0);
    }

    public void RollLeft()
    {
        rigidbody.AddRelativeTorque(0, 0, RotationSpeed *  Time.deltaTime * 100);
    }
    public void RollRight()
    {
        rigidbody.AddRelativeTorque(0, 0, -RotationSpeed *  Time.deltaTime * 100);
    }

    public void Stabilise()
    {
        rigidbody.AddForce(-rigidbody.velocity * forwardSpeed * 10 * Time.deltaTime);
        rigidbody.AddTorque(-rigidbody.angularVelocity.normalized * 200 * RotationSpeed * Time.deltaTime);
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
        explosion.transform.localScale = transform.localScale * explosionScale;
        Destroy(explosion, 2f);

    
        foreach(Transform child in transform)
        {
            if (child.gameObject.CompareTag("MainCamera")) continue;
            
            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            child.gameObject.name = "SCRAP";
            // If there are any tags attached to the ship, destroy them
            if (child.gameObject.CompareTag("Enemy") || child.gameObject.name.Contains("Tag")) 
                Destroy(child.gameObject);
        }

        explosionRadius *= transform.localScale.x;

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null) rb.AddExplosionForce(explosionPower, transform.position, explosionRadius, 0);
        }

        transform.DetachChildren();
        Destroy(gameObject);
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized] public ShipController parent;
    public Attributes type;
    public ModifiableInt value;

    public void SetParent(ShipController _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}