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
    public Attribute[] attributes;
    public InventoryObject inventory; 
    public InventoryObject equipment;

    [HideInInspector] public Vector3 currentVelocity;
    [HideInInspector] public new Rigidbody rigidbody; // FIXME: shouldn't be public later on?
    [HideInInspector] public HealthData healthData;
    [HideInInspector] public UnityEvent onTargeted;

    private void Awake()
    {
        healthData = GetComponent<HealthData>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start() 
    {
        // TODO: this is for debug
        inventory.Load();
        equipment.Load();
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }    
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnBeforeEquipmentSlotUpdate;
            equipment.GetSlots[i].OnAfterUpdate += OnAfterEquipmentSlotUpdate;
        }
    }

    public void OnBeforeEquipmentSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null) return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                    }
                }
                break;
            default:
                break;
        }
    }
    public void OnAfterEquipmentSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null) return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                            attributes[j].value.AddModifier(_slot.item.buffs[i]);
                    }
                }
                break;
            default:
                break;
        }
    }

    private void Update() 
    {
        if (healthData.health <= 0) Explode();
        // TODO: this is for debug
        if (Input.GetKeyDown(KeyCode.F9))
        {
            inventory.Save();
            equipment.Save();
        }
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

    public void AttributeModified(Attribute attribute)
    {
        
    }


    private void Explode()
    {
        /* 
        Checking nearby colliders and adding explosion force to their rigidbodies (if they have)
        Also adding rigidbodies to children and then explosion force to them
        Renaming children to SCRAPE, then detaching them, then destroying gameObject
        */
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
        explosion.transform.localScale = transform.localScale * explosionScale;
        Destroy(explosion, 2f);

        foreach(Transform child in transform)
        {
            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            child.gameObject.name = "SCRAP";
            // Destroy attached tags if there are any.
            // TODO: Check if "Tag" is in name, and destroy it
            // or if Tag is not set to default, destroy
            if (child.gameObject.CompareTag("Enemy")) Destroy(child.gameObject);
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

    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized] public Ship parent;
    public Attributes type;
    public ModifiableInt value;

    public void SetParent(Ship _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}