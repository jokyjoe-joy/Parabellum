using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float moveSpeed = 100f;
    public int DamageAmount = 10;
    public float distanceToExplode = 15;
    public float explosionRadius = 100;
    public GameObject explosionVFX;
    public float explosionScale = 2;
    // TODO: Rocket having a range, or maybe the gun?
    private Vector3 shootDir;
    private new Rigidbody rigidbody;
    private bool isLookingForEnemy = true;


    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(Vector3 shootDir, bool isLookingForEnemy = true)
    {
        this.shootDir = shootDir;
        this.isLookingForEnemy = isLookingForEnemy;
        // TODO: Rocket should start with the same velocity as the ship
        // because the other way it hits the shooter ship if it is moving
        //rigidbody.AddForce(transform.forward * Time.deltaTime * 100);
        Destroy(gameObject, 10f);
        
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
        GameObject[] enemies;
        // Check enemies nearby
        if (this.isLookingForEnemy) {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        } else {
            enemies = GameObject.FindGameObjectsWithTag("PlayerTag");
        }
        // Find nearest one
        GameObject target = GetClosestEnemy(enemies);

        if (target != null)
        {
            Vector3 vector = target.transform.position - transform.position;
            rigidbody.AddForce(vector * Time.deltaTime * moveSpeed);
            if (vector.magnitude < distanceToExplode) {
                Explode();
            }
        } 
        else 
        {
            rigidbody.AddForce(transform.forward * Time.deltaTime * moveSpeed * 200);
            Destroy(gameObject, 3f);
        }
        

    }

    private void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders) {
            Ship ship = collider.gameObject.GetComponent<Ship>();
            if (ship == null && collider.gameObject.transform.parent != null) {
                ship = collider.gameObject.transform.parent.GetComponent<Ship>();
            }
            if (ship != null)
            {
                // target hit
                // FIXME: For some reason, sometimes it doubles the damage?? (hitting multiple colliders as well?)
                ship.Damage(DamageAmount);
                // create VFX
                GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
                explosion.transform.localScale *= explosionScale * collider.gameObject.transform.localScale.x;
                Destroy(explosion, 2f);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Explode();
    }
}
