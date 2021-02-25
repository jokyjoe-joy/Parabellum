using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAI : MonoBehaviour
{
    public float DampingOnTurning = 3f;
    public float DistanceFromMoveTarget = 50f;
    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private Ship ship;
    private WeaponSystem weaponSystem;
    private bool moveResult = false;

    // Returns random normalized vector
    private Vector3 getRandomDirection() {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private bool MoveTo(Vector3 target) {
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
        // TODO: the function below works, however "AI" is cheating greatly
        // we don't need to solve it yet, but documentation would help ;)
        // Look at target
        var rotation = Quaternion.LookRotation (target - transform.position);
        transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * DampingOnTurning);
        // If looking at target, do this:
        if (Quaternion.Angle(transform.rotation, rotation) < 5) {
            // Then move towards target
            Vector3 vectorTowardsTarget = (target - transform.position);
            if (vectorTowardsTarget.magnitude >= DistanceFromMoveTarget) {
                ship.ThrustForward();
            } else {
                // Stopping.
                ship.rigidbody.velocity = new Vector3(0,0,0);
                return true;
            }
        } else {
            if (ship.rigidbody.velocity.magnitude > 20) {
                ship.rigidbody.velocity = new Vector3(0,0,0);
            }
        }
        return false;
    }

    void Awake() {
        ship = GetComponent<Ship>();
        weaponSystem = GetComponent<WeaponSystem>();
    }
    void Start()
    {
        startingPosition = transform.position;
        roamPosition = startingPosition + getRandomDirection() * Random.Range(10f, 70f);
    }

    void FixedUpdate() {
        GameObject player = GameObject.FindGameObjectWithTag("PlayerTag");
        var distanceFromPlayer = player.transform.position - transform.position;
        if (distanceFromPlayer.magnitude > DistanceFromMoveTarget) {
            moveResult = false;
        }
        
        // look at opponent
        var rotation = Quaternion.LookRotation (player.transform.position - transform.position);
        if (Quaternion.Angle(transform.rotation, rotation) < 2) {
            transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * DampingOnTurning);
        }

        // Move if haven't arrived
        if (!moveResult) {moveResult = MoveTo(player.transform.position);}
        else {
            if (ship.rigidbody.velocity.magnitude < 10) {
                weaponSystem.ShootGuns();
            } else {
                ship.rigidbody.velocity = new Vector3(0,0,0);
            }
        }
    }
}
