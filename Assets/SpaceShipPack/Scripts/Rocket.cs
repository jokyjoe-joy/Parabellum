using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private enum State {
        Startup,
        GoingForward,
        ChaseTarget
    }
    private State state;
    public float moveSpeed = 100f;
    public int DamageAmount = 10;
    public float distanceToExplode = 1;
    public float explosionRadius = 1;
    public GameObject explosionVFX;
    public float explosionScale = 2;
    // TODO: Rocket having a range, or maybe the gun?
    private Vector3 shootDir;
    private Vector3 initialVelocity;
    private new Rigidbody rigidbody;
    private bool isLookingForEnemy = true;
    public float timeBeforeChasing = 1.0f;


    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(Vector3 shootDir, Vector3 initialVelocity, bool isLookingForEnemy = true)
    {
        this.shootDir = shootDir;
        transform.localRotation = Quaternion.Euler(90,180,90);
        this.isLookingForEnemy = isLookingForEnemy;
        rigidbody.velocity = initialVelocity;
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
        switch (state) {
            default:
            case State.Startup:
                // Wait for timeBeforeChasing time, until then, move forward
                StartCoroutine(ChangeToChasingInSeconds(timeBeforeChasing));
                state = State.GoingForward;
                break;
            case State.GoingForward:
                rigidbody.AddForce(shootDir * Time.deltaTime * moveSpeed * 200);
                break;
            case State.ChaseTarget:
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
                break;
        }
    }

    IEnumerator ChangeToChasingInSeconds(float delay) {
        yield return new WaitForSeconds(delay);
        state = State.ChaseTarget;
    }

    private void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders) {
            HealthData healthData = collider.gameObject.GetComponent<HealthData>();
            if (healthData == null && collider.gameObject.transform.parent != null) {
                healthData = collider.gameObject.transform.parent.GetComponent<HealthData>();
            }
            if (healthData != null)
            {
                // target hit
                // FIXME: For some reason, sometimes it doubles the damage?? (hitting multiple colliders as well?)
                healthData.Damage(DamageAmount);
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
