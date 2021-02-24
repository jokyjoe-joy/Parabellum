using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 100f;
    public int DamageAmount = 10;
    private Vector3 shootDir;

    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        //transform.right = shootDir; // ???
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        // Move in given direction
        transform.position += shootDir * Time.deltaTime * moveSpeed * 150;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Ship ship = collider.gameObject.GetComponent<Ship>();
        if ( ship != null )
        {
            // target hit
            ship.Damage(DamageAmount);
            Destroy(gameObject);
        }
    }
}
