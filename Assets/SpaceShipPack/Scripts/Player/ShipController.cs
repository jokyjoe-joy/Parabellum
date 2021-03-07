using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class ShipController : MonoBehaviour
{
    private float defaultFOV;
    private float currentFOV;
    private Ship ship;
    private WeaponSystem weaponSystem;
    private PostProcessVolume postProcessVolume;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private Vignette vignette;
    private float vignetteDefaultValue;
    private CameraShake cameraShake;
    private HealthData healthData;
    [HideInInspector] public GameObject currentTarget = null;
    [HideInInspector] public bool shouldCheckControls = true;
    public GameObject firstPersonCamera;
    public GameObject thirdPersonCamera;
    private SmoothMouseLook smoothMouse;
    private Vector3 initialCameraRotation;
    public InventoryObject inventory; 
    public InventoryObject equipment;
    public Attribute[] attributes;
    [System.Serializable]
    public struct Guns {
        public Transform positionTransf;
        public Transform gunObj;
    }
    public Guns[] shipGuns;

    private void Awake()
    {
        weaponSystem = GetComponent<WeaponSystem>();
        healthData = GetComponent<HealthData>();
        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out chromaticAberration);
        postProcessVolume.profile.TryGetSettings(out lensDistortion);
        postProcessVolume.profile.TryGetSettings(out vignette);
        vignetteDefaultValue = vignette.intensity.value;
        cameraShake = Camera.main.GetComponent<CameraShake>();
        healthData.onDamage.AddListener(ShakeCamera);
        smoothMouse  = firstPersonCamera.GetComponent<SmoothMouseLook>();
        ship = GetComponent<Ship>();

        if (inventory == null) Debug.LogWarning("Player's inventory is null!");
        if (equipment == null) Debug.LogWarning("Player's equipment is null!");
    
    }

    void Start()
    {
        // TODO: Get a better method for doing this stuff below
        // Load only after Start() because in inventorySlot, in UpdateSlot, onAfterUpdate and onBeforeUpdate
        // don't work right after start (they are null).
        StartCoroutine(LoadInventories());

        // Locking cursor
        Cursor.lockState = CursorLockMode.Locked;
        defaultFOV = Camera.main.fieldOfView;
        initialCameraRotation = Camera.main.transform.localEulerAngles;

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

    void Update()
    {
        if (Input.GetKeyDown("v"))
        {
            // Switch between third and first person camera
            if (Camera.main.name == firstPersonCamera.name)
            {
                Camera.main.gameObject.SetActive(false);
                thirdPersonCamera.SetActive(true);
            }
            else
            {
                Camera.main.gameObject.SetActive(false);
                firstPersonCamera.SetActive(true);
            }
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Camera.main.name == firstPersonCamera.name)
        {
            smoothMouse.enabled = true;
            shouldCheckControls = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt) && Camera.main.name == firstPersonCamera.name)
        {
            smoothMouse.enabled = false;
            shouldCheckControls = true;
            Camera.main.transform.localEulerAngles = initialCameraRotation;
        }

        // TODO: alternative save mode/time?
        if (Input.GetKeyDown(KeyCode.F9))
        {
            inventory.Save();
            equipment.Save();
        }

    }

    IEnumerator LoadInventories()
    {
        yield return new WaitForSeconds(0.3f);
        inventory.Load();
        equipment.Load();
    }

    private void CheckShootingControls()
    {
        // Create Shoot event on Left Click
        if (Input.GetMouseButton(0))
        {
            weaponSystem.ShootGuns(ship.currentVelocity);
        }
        if (Input.GetKeyDown("t"))
        {

            // Try to target enemy
            // Check enemies nearby
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject target = jokyUtilities.GetClosestGameObject(enemies, transform);
            
            if (target != null)
            {
                // FIXME: isOffScreen doesn't work properly! Sometimes when object is offScreen
                // it still returns false
                bool isOffScreen = jokyUtilities.checkIfObjectIsOffScreen(target.transform.position);
                // In case Player doesn't have a target, try to target the closest enemy
                // which is on-screen.
                if (currentTarget == null && !isOffScreen)
                {
                    // Invoke onTargeted event on the targeted ship
                    // Below parent is needed, as enemyTag is its child gameobject
                    currentTarget = target.transform.parent.gameObject;
                    Ship enemyShip = target.transform.parent.GetComponent<Ship>();
                    if (enemyShip != null) enemyShip.onTargeted.Invoke();
                }
                else
                {
                    // If there is only one enemy nearby, then turn off targeting
                    if (enemies.Length == 1)
                    {
                        currentTarget = null;
                    }
                    else
                    {
                        // TODO: Does this work below properly?
                        currentTarget = jokyUtilities.GetClosestGameObject(enemies, transform).transform.parent.gameObject;
                        //currentTarget = enemies[Random.Range(0,enemies.Length)].transform.parent.gameObject;
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (shouldCheckControls) CheckMovementControls();
        if (shouldCheckControls) CheckShootingControls();
        // Note: AdjustFOVOnSpeed check Right Click for zooming
        if (shouldCheckControls) AdjustFOVOnSpeed();
        AdjustPostProcessing();
    }

    private void CheckMovementControls()
    {
        ship.LookUpDown(Input.GetAxis("Mouse Y"));
        ship.LookLeftRight(Input.GetAxis("Mouse X"));

        if (Input.GetKey("w"))
        {
            ship.ThrustForward();
        }
        if (Input.GetKey("s"))
        {
            ship.ThrustBackwards();
        }
        if (Input.GetKey("a"))
        {
            ship.RollLeft();
        }
        if (Input.GetKey("d"))
        {
            ship.RollRight();
        }
        if (Input.GetKey("space"))
        {
            ship.Stabilise();
        }
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
                            for (int i = 0; i < shipGuns.Length; i++)
                            {
                                if (shipGuns[i].gunObj != null)
                                {
                                    weaponSystem.Guns.Remove(shipGuns[i].gunObj.GetComponent<Gun>());
                                    Destroy(shipGuns[i].gunObj.gameObject);
                                    return;
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
                                    return;
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

    public Transform AddEquipment(GameObject _equipmentToAdd, Transform _equipmentPosition)
    {
        GameObject _equippedEquipment = Instantiate(_equipmentToAdd, _equipmentPosition.position, _equipmentPosition.rotation);
        _equippedEquipment.transform.SetParent(transform);
        return _equippedEquipment.transform;
    }
    
    public void AttributeModified(Attribute attribute)
    {
        // TODO: this?
    }

    private void AdjustFOVOnSpeed()
    {
        // Dynamic field of view based on speed
        if (ship.currentVelocity.magnitude > 10)
        {
            currentFOV = defaultFOV + ship.currentVelocity.magnitude / 10;
        }
        else
        {
            currentFOV = 60;
        }

        // If right-clicking, zoom
        if (Input.GetMouseButton(1))
        {
            currentFOV /= 2;
        }

        // setting fov
        Camera.main.fieldOfView = currentFOV;
    }

    private void AdjustPostProcessing() 
    {
        float aberrationIntensity = ship.currentVelocity.magnitude / 150f;
        if (chromaticAberration != null) chromaticAberration.intensity.Override(Mathf.Clamp(aberrationIntensity, 0f, 1f));

        float lensDistortionIntensity = -(ship.currentVelocity.magnitude / 10);
        if (lensDistortion != null) lensDistortion.intensity.Override(Mathf.Clamp(lensDistortionIntensity, -20f, 0f));

        if (healthData.health < healthData.healthMax && vignette != null && chromaticAberration != null)
        {
            vignette.color.Override(Color.red);
            float vignetteIntensityValue = (healthData.healthMax - healthData.health) / healthData.healthMax;
            vignette.intensity.Override(Mathf.Clamp(vignetteIntensityValue,vignetteDefaultValue,0.65f));
            
            chromaticAberration.intensity.Override(1f);
        }
        else
        {
            if (vignette != null) vignette.intensity.Override(vignetteDefaultValue);
        }
    }

    private void ShakeCamera()
    {
        cameraShake.shakeDuration = 0.4f;
    }

    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
}
