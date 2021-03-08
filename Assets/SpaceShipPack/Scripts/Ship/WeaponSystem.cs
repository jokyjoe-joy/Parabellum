using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public List<Gun> Guns = new List<Gun>();
    public List<string> raycastWhitelist = new List<string>();
    public int raycastSphereRadius = 12;
    private bool isLookingForEnemy = false;
    [HideInInspector] public RaycastHit hit;

    private void Awake()
    {
        // In case this is a player ship, attack AI
        ShipAI shipAI = GetComponent<ShipAI>();
        if (shipAI == null) isLookingForEnemy = true;

        // add default blacklists
        raycastWhitelist.Add("enemy");
        raycastWhitelist.Add("Enemy");
        raycastWhitelist.Add("Player");
    }

    private void FixedUpdate() 
    {
        OrientateGunsTowardsCrosshair();
    }

    private void OrientateGunsTowardsCrosshair()
    {
        Vector3 p1 = transform.position;
        Vector3 p2 = transform.TransformDirection(Vector3.forward);
        
        // Check if there is any object in front of the ship and gun.LookAt() at that position.
        // If there is no object there, reset back to initialRotation.
        if (Physics.SphereCast(p1, raycastSphereRadius, p2, out hit, Mathf.Infinity))
        {
            // Only look at object if it is in whitelist.
            foreach (string whitename in raycastWhitelist)
            {
                if (hit.transform.name.Contains(whitename))
                {
                    // NOTE: On bullets use Ignore Raycast layer!
                    Debug.DrawRay(p1, p2 * hit.distance, Color.yellow);
                    foreach (Gun gun in Guns)
                    {
                        if (gun != null) gun.transform.LookAt(hit.point);
                    }
                }
            }
        }
        else
        {
            //Debug.DrawRay(p1, p2 * 1000, Color.white);
            foreach (Gun gun in Guns)
            {
                if (gun != null) gun.transform.localEulerAngles = gun.initialRotation;
            }
        }
    }

    public void ShootGuns(Vector3 initialVelocity)
    {   
        // Shooting, then reloading
        foreach (Gun gun in Guns)
        {
            if (gun == null) continue;
            gun.Shoot(initialVelocity, isLookingForEnemy);
            StartCoroutine(gun.Reload());
        }
    }
}
