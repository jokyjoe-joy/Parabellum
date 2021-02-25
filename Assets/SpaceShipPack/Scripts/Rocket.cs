using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float moveSpeed = 100f;
    public int DamageAmount = 10;
    private Vector3 shootDir;
    private new Rigidbody rigidbody;
    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        rigidbody = GetComponent<Rigidbody>();
    }
    GameObject GetClosestEnemy(GameObject[] enemies)
    {
		// create bestTarget
        GameObject bestTarget = null;
		
		// create closest distance and set it to infinity
        float closestDistanceSqr = Mathf.Infinity;
		
		// get currentPosition
        Vector3 currentPosition = transform.position;
		
		// go through each given target
        foreach (GameObject potentialTarget in enemies)
        {
			// calculate vector towards target
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
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
        GameObject target = GetClosestEnemy(enemies);

        if (target != null)
        {
            Vector3 vector = target.transform.position - transform.position;
            rigidbody.AddForce(vector * Time.deltaTime * moveSpeed);
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
