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

        //Destroy(gameObject, 15f);
    }
    Transform GetClosestEnemy(GameObject[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialT in enemies)
        {
            Transform potentialTarget = potentialT.transform;
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
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

        // If there is a target move towards it, else self-destruct with delay
        if (target != null)
        {
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
