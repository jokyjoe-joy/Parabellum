using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float moveSpeed = 100f;
    public int DamageAmount = 10;
    private Vector3 shootDir;

    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
    }
    Transform GetClosestEnemy(GameObject[] enemies)
    {
		// TODO: rather get Transform[] than GameObject[]
		
		// create bestTarget
        Transform bestTarget = null;
		
		// create closest distance and set it to infinity
        float closestDistanceSqr = Mathf.Infinity;
		
		// get currentPosition
        Vector3 currentPosition = transform.position;
		
		// go through each given target
        foreach (GameObject potentialT in enemies)
        {
			// get target's transform
            Transform potentialTarget = potentialT.transform;
			// calculate vector towards target
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
			// getting magnitude of distance (avoiding square root calculation with sqrMagnitude)
            float dSqrToTarget = directionToTarget.sqrMagnitude;
			// if current target is closer than the current closest one
            if (dSqrToTarget < closestDistanceSqr)
            {
				// save this target's distance as closest
                closestDistanceSqr = dSqrToTarget;
				// save this target as the bestTarget
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }

    private void Update()
    {
        // Check enemies nearby
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        // Find nearest one
        Transform target = GetClosestEnemy(enemies);

        if (target != null)
        {
			// TODO: Rather use rigidbody.addForce?
            // Move towards target
            float step = moveSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        } 
        else 
        {
            Destroy(gameObject);
        }
        

    }

    private void OnTriggerEnter(Collider collider)
    {
        Ship ship = collider.gameObject.GetComponent<Ship>();
        if (ship != null)
        {
            // target hit
            ship.Damage(DamageAmount);
            Destroy(gameObject);
        }
    }
}
