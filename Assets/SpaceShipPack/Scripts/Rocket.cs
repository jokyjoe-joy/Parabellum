using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float moveSpeed = 100f;
    public int DamageAmount = 10;
    public GameObject explosionVFX;
    public float explosionScale = 2;
    private Vector3 shootDir;
    private new Rigidbody rigidbody;


    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        // TODO: Rocket should start with the same velocity as the ship
        // because the other way it hits the shooter ship if it is moving
        //rigidbody.AddForce(transform.forward * Time.deltaTime * 100);
        
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

    private void FixedUpdate()
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
            rigidbody.AddForce(transform.forward * Time.deltaTime * moveSpeed * 200);
            Destroy(gameObject, 3f);
        }
        

    }

    private void OnTriggerEnter(Collider collider)
    {
        // Find Ship component on collider.
        // If there is no Ship component, then try to get it from parent.
        Ship ship = collider.gameObject.GetComponent<Ship>();
        if (ship == null && collider.gameObject.transform.parent != null) {
            ship = collider.gameObject.transform.parent.GetComponent<Ship>();
        }

        if (ship != null)
        {
            // target hit
            // FIXME: For some reason, sometimes it doubles the damage (hitting multiple colliders as well?)
            ship.Damage(DamageAmount);
            // create VFX
            GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            explosion.transform.localScale *= explosionScale * collider.gameObject.transform.localScale.x;
            Destroy(explosion, 2f);
            Destroy(gameObject);
        }
    }
}
