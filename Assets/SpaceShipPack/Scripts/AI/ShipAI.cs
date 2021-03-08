using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAI : MonoBehaviour
{
    public string nameOfAI = "Joe";
    private enum State {
        Roaming,
        ChaseTarget
    }
    private State state;
    public float dampingOnTurning = 3f;
    public float DistanceFromMoveTarget = 50f;
    public float shootingAccuracy = 0.6f;
    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private GameObject currentTarget;
    private Ship ship;
    private WeaponSystem weaponSystem;
    public Transform dropItemTemplatePf;
    public int itemDropSpawnRadius = 15;
    [System.Serializable]
    public struct ItemsToDropOnDeath
    {
        public ItemObject item;
        public float chance;
    }

    [Tooltip("Chance is a float between 0.0 and 1.0. The higher the number the bigger the chance to drop.")]
    public ItemsToDropOnDeath[] itemsToDrop;
    private int shootingRange;

    void Awake()
    {
        ship = GetComponent<Ship>();
        weaponSystem = GetComponent<WeaponSystem>();
        ship.onTargeted.AddListener(OnTargeted);
        ship.onDeath.AddListener(OnDeath);

        // Set shooting range to the range of the gun with the smallest range.
        shootingRange = 9999;
        for (int i = 0; i < weaponSystem.Guns.Count; i++)
        {
            if (weaponSystem.Guns[i].maxRange < shootingRange) shootingRange = weaponSystem.Guns[i].maxRange;
        }
    }
    void Start()
    {
        roamPosition = AIMovement.GetRoamingPosition(transform.position);
    }

    void OnTargeted() 
    {
        // TODO: set current target to the one that has targeted this ship
        currentTarget = GameObject.FindGameObjectWithTag("PlayerTag").transform.parent.gameObject;
        state = State.ChaseTarget;
    }

    void Update()
    {
        switch (state) 
        {
            default:
            case State.Roaming:
                AIMovement.MoveTo(roamPosition, ship, dampingOnTurning);
                Debug.DrawLine(transform.position, roamPosition, Color.green);
                float reachedPositionDistance = 10f;
                if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance) 
                {
                    // Reached roam position, thus getting new roam position
                    roamPosition = AIMovement.GetRoamingPosition(transform.position);
                }
                
                GameObject player = GameObject.FindGameObjectWithTag("PlayerTag");
                FindTarget(player);
                break;
            case State.ChaseTarget:
                // Look at player
                // If within shootingRange of player, stop and shoot at it
                // Otherwise move towards player
                
                // TODO: Think of a better way to calculate accuracy, also use player's speed in calculation.
                // Choose a random position around the player where AI will shoot.
                Vector3 targetPosition = Random.insideUnitSphere * (35 * (1 / shootingAccuracy)) + currentTarget.transform.position;
                
                var rotation = Quaternion.LookRotation (targetPosition - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampingOnTurning);
                
                if (Vector3.Distance(targetPosition, transform.position) < shootingRange) 
                {
                    ship.Stabilise();
                    weaponSystem.ShootGuns(ship.currentVelocity);
                } 
                else 
                {
                    Debug.DrawLine(transform.position, targetPosition, Color.green);
                    AIMovement.MoveTo(targetPosition, ship, dampingOnTurning);
                }
                break;
        }
    }

    void FindTarget(GameObject target) 
    {
        float targetRange = 500f;
        if (Vector3.Distance(transform.position, target.transform.position) < targetRange) 
        {
            state = State.ChaseTarget;
            // Invoke ship's onTargeted event (if it is a ship).
            Ship targetShip = target.GetComponent<Ship>();
            // TODO: When invoking, pass (this ship? as) argument
            if (targetShip != null) targetShip.onTargeted.Invoke();
            currentTarget = target.gameObject;
        }
    }

    void OnDeath()
    {
        for (int i = 0; i < itemsToDrop.Length; i++)
        {
            if (Random.Range(0.0f,1.0f) < itemsToDrop[i].chance)
            {
                Vector3 placeToSpawn = (Random.insideUnitSphere * itemDropSpawnRadius) + transform.position;
                Transform droppedItem = Instantiate(dropItemTemplatePf, placeToSpawn, Quaternion.identity);
                droppedItem.GetComponent<CollectableToInventory>().item = itemsToDrop[i].item;
            }
        }
    }
}
