using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeadBullet : MonoBehaviour
{
    // TODO: Have a Bullet script that will be Rocket and LeadBullet using instead of MonoBehaviour

    public float moveSpeed = 100f;
    private Vector3 shootDir;
    private new Rigidbody rigidbody;
    public GameObject explosionVFX;
    public float explosionScale = 1;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    public void Setup(Vector3 shootDir, Vector3 initialVelocity, int maxRange, int damageAmount)
    {
        this.shootDir = shootDir;
        rigidbody.velocity = initialVelocity;
        Destroy(gameObject, 5f);
        

        RaycastHit hit;
        if (Physics.Raycast(transform.position, shootDir, out hit, maxRange))
        {
            if (hit.transform.GetComponent<HealthData>() != null)
            {
                // TODO: Wait as much so that the damage is done when the bullet is there at the ship.
                // Wait a little bit so that the route of the bullet looks realistic
                WaitSeconds(0.5f);
                HealthData healthData = hit.transform.GetComponent<HealthData>();
                healthData.Damage(damageAmount);
                GameObject explosion = Instantiate(explosionVFX, hit.point, Quaternion.identity);
                explosion.transform.localScale *= explosionScale * hit.transform.localScale.x;
                Destroy(explosion, 2f);
            }
        }
    }

    IEnumerator WaitSeconds(float x)
    {
        yield return new WaitForSeconds(x);
    }

    private void FixedUpdate()
    {
        // Move in given direction
        rigidbody.AddForce(shootDir * Time.deltaTime * moveSpeed * 1000);
    }
}
