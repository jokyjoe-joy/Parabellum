using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public static Vector3 GetRandomDirection()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    public static Vector3 GetRoamingPosition(Vector3 startingPosition)
    {
        return startingPosition + GetRandomDirection() * Random.Range(100f, 500f);
    }

    public static void MoveTo(Vector3 target, Ship ship, float dampingOnTurning)
    {
        /* 
        // TODO: Finish this pseudo-code :)
        float neededDistanceFromTarget;
        bool targetReached;

        when looking at target { // Quaternion.Angle(ship.gameObject.transform.rotation, rotation) < 1
            move towards it;
            if (velocity.magnitude >= distanceFromTarget) { // ????
                slow down;
            }

        } else {
            look at target;
        }
        
         */
        // Look at target
        var rotation = Quaternion.LookRotation (target - ship.gameObject.transform.position);
        ship.gameObject.transform.rotation = Quaternion.Slerp (ship.gameObject.transform.rotation, rotation, Time.deltaTime * dampingOnTurning);
        // If looking at target, do this:
        if (Quaternion.Angle(ship.gameObject.transform.rotation, rotation) < 5)
        {
            // Then move towards target
            Vector3 vectorTowardsTarget = (target - ship.gameObject.transform.position);
            if (vectorTowardsTarget.magnitude >= 1)
            {
                ship.ThrustForward();
            } 
            else
            {
                // Stopping.
                ship.rigidbody.velocity = new Vector3(0,0,0);
            }
        }
        else
        {
            if (ship.rigidbody.velocity.magnitude > 20)
            {
                // Stopping.
                ship.rigidbody.velocity = new Vector3(0,0,0);
            }
        }
    }
}
