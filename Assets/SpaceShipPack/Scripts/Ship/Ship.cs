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
    [System.Serializable]
    public struct Guns {
        public Transform positionTransf;
        public Transform gunObj;
    }
    public Guns[] shipGuns;
    
    [HideInInspector] public WeaponSystem weaponSystem;
    [HideInInspector] public Vector3 currentVelocity;
    [HideInInspector] public new Rigidbody rigidbody; // FIXME: shouldn't be public later on?
    [HideInInspector] public HealthData healthData;
    [HideInInspector] public UnityEvent onTargeted;

    private void Awake()
    {
        healthData = GetComponent<HealthData>();
        rigidbody = GetComponent<Rigidbody>();
        weaponSystem = GetComponent<WeaponSystem>();
        // TODO: Put all the inventory stuff to ShipController to avoid complicating everything.
        if (inventory == null) inventory = ScriptableObject.CreateInstance<InventoryObject>();
        if (equipment == null) equipment = ScriptableObject.CreateInstance<InventoryObject>();
        
    }

    private void Start() 
    {

        // TODO: Get a better method for doing this stuff below
        // Load only after Start() because in inventorySlot, in UpdateSlot, onAfterUpdate and onBeforeUpdate
        // don't work right after start (they are null).
        StartCoroutine(LoadInventories());

        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }    
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveEquipment;
            equipment.GetSlots[i].OnAfterUpdate += OnAddEquipment;
        }
    }

    IEnumerator LoadInventories()
    {
        yield return new WaitForSeconds(0.3f);
        inventory.Load();
        equipment.Load();
    }

    public void OnRemoveEquipment(InventorySlot _slot)
    {
        if (_slot.ItemObject == null) return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                // Removing attributes that this item was giving the ship.
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                    }
                }

                if (_slot.ItemObject.prefabToEquip != null)
                {
                    switch (_slot.AllowedItems[0])
                    {
                        case ItemType.Weapon:
                            // TODO: This is not removing the item that is intended to remove.
                            // TODO: should remove from weaponsystem as well?!
                            for (int i = 0; i < shipGuns.Length; i++)
                            {
                                if (shipGuns[i].gunObj != null)
                                {
                                    Destroy(shipGuns[i].gunObj.gameObject);
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }
    public void OnAddEquipment(InventorySlot _slot)
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

                if (_slot.ItemObject.prefabToEquip != null)
                {
                    switch (_slot.AllowedItems[0])
                    {
                        case ItemType.Weapon:
                            for (int i = 0; i < shipGuns.Length; i++)
                            {
                                if (shipGuns[i].gunObj == null)
                                {
                                    shipGuns[i].gunObj = AddEquipment(_slot.ItemObject.prefabToEquip, shipGuns[i].positionTransf);
                                    weaponSystem.Guns.Add(shipGuns[i].gunObj.gameObject.GetComponent<Gun>());
                                }
                            }
                            break;
                        default:
                            break;
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
        // TODO: alternative save mode/time?
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
        // TODO: this?
    }

    public Transform AddEquipment(GameObject _equipmentToAdd, Transform _equipmentPosition)
    {
        GameObject _equippedEquipment = Instantiate(_equipmentToAdd, _equipmentPosition.position, _equipmentPosition.rotation);
        _equippedEquipment.transform.SetParent(transform);
        return _equippedEquipment.transform;
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