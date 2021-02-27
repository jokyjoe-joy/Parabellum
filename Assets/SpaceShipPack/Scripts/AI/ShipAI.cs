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
    public float DampingOnTurning = 3f;
    public float DistanceFromMoveTarget = 50f;
    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private Ship ship;
    private WeaponSystem weaponSystem;

    // Returns random normalized vector
    private Vector3 GetRandomDirection() {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private Vector3 GetRoamingPosition() {
        return startingPosition + GetRandomDirection() * Random.Range(100f, 500f);
    }

    private void MoveTo(Vector3 target) {
        /* 
        // TODO: Finish this pseudo-code :)
        float neededDistanceFromTarget;
        bool targetReached;

        when looking at target { // Quaternion.Angle(transform.rotation, rotation) < 1
            move towards it;
            if (velocity.magnitude >= distanceFromTarget) { // ????
                slow down;
            }

        } else {
            look at target;
        }
        
         */
        // Look at target
        var rotation = Quaternion.LookRotation (target - transform.position);
        transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * DampingOnTurning);
        // If looking at target, do this:
        if (Quaternion.Angle(transform.rotation, rotation) < 5) {
            // Then move towards target
            Vector3 vectorTowardsTarget = (target - transform.position);
            if (vectorTowardsTarget.magnitude >= 1) {
                ship.ThrustForward();
            } else {
                // Stopping.
                ship.Stabilise();
            }
        } else {
            if (ship.rigidbody.velocity.magnitude > 20) {
                ship.Stabilise();
            }
        }
    }

    void Awake() {
        ship = GetComponent<Ship>();
        weaponSystem = GetComponent<WeaponSystem>();
        ship.onTargeted.AddListener(OnTargeted);
    }
    void Start()
    {
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
    }

    void OnTargeted() {
        Debug.Log("AI HAS BEEN TARGETED");
    }

    void Update() {
        switch (state) {
            default:
            case State.Roaming:
                MoveTo(roamPosition);
                Debug.DrawLine(transform.position, roamPosition, Color.green);
                float reachedPositionDistance = 10f;
                if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance) {
                    // Reached roam position, thus getting new roam position
                    roamPosition = GetRoamingPosition();
                }
                
                GameObject player = GameObject.FindGameObjectWithTag("PlayerTag");
                FindTarget(player);
                break;
            case State.ChaseTarget:
                // Look at player
                // If within shootingRange of player, stop and shoot at it
                // Otherwise move towards player
                player = GameObject.FindGameObjectWithTag("PlayerTag");
                var rotation = Quaternion.LookRotation (player.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * DampingOnTurning);
                
                float shootingRange = 200f;
                if (Vector3.Distance(player.transform.position, transform.position) < shootingRange) {
                    ship.Stabilise();
                    weaponSystem.ShootGuns(ship.currentVelocity);
                } else {
                    Debug.DrawLine(transform.position, player.transform.position, Color.green);
                    MoveTo(player.transform.position);
                }
                break;
        }
    }

    void FindTarget(GameObject target) {
        float targetRange = 500f;
        if (Vector3.Distance(transform.position, target.transform.position) < targetRange) {
            state = State.ChaseTarget;
            // Invoke ship's onTargeted event (if it is a ship).
            Ship targetShip = target.GetComponent<Ship>();
            if (targetShip != null) {
                targetShip.onTargeted.Invoke();
            }
        }
    }
}
