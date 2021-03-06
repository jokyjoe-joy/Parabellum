using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform pfBullet;
    public float reloadTime = 5;
    public int maxRange = 1000;
    public int damageAmount = 1;
    public bool isGunLoaded = true;
    public bool isGunLoading = false;
    public GameObject shootingVFX;
    public float shootingVFXScale = 1;
    public Vector3 initialRotation;
    public AudioClip audioOnShoot;
    private void Start()
    {
        initialRotation = transform.localEulerAngles;
    }

    /// <summary>
    /// Shoots its own bullet.
    /// </summary>
    /// <returns>True if bullet was shot, false if it was not.</returns>
    public bool Shoot(Vector3 initialVelocity, bool isLookingForEnemy=true)
    {
        if (initialVelocity == null) initialVelocity = new Vector3(0,0,0);
        // Only shoot if gun is loaded
        if (isGunLoaded) {
            // Shooting out bullet, then "deloading" Gun
            var bulletOutPosition = transform.Find("bulletOut").transform.position;
            Transform bulletTransform = Instantiate(pfBullet, bulletOutPosition, Quaternion.identity);
            isGunLoaded = false;

            // Creating VFX
            if (shootingVFX != null) {
                GameObject explosion = Instantiate(shootingVFX, bulletOutPosition, Quaternion.identity);
                explosion.transform.parent = transform;
                explosion.transform.localScale *= shootingVFXScale;
                Destroy(explosion, 2f);
            }

            // Init Bullet's script
            LeadBullet bulletController = bulletTransform.GetComponent<LeadBullet>();
            if (bulletController != null)
            {
                Vector3 aimDir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
                bulletController.Setup(transform.forward, aimDir * initialVelocity.sqrMagnitude, maxRange, damageAmount);
            }

            Rocket rocketController = bulletTransform.GetComponent<Rocket>();
            if (rocketController != null)
            {
                rocketController.Setup(transform.forward, initialVelocity, damageAmount, isLookingForEnemy);
            }

            return true;
        }
        else return false;
    }

    public IEnumerator Reload()
    {
        if (!isGunLoading)
        {
            isGunLoading = true;
            yield return new WaitForSeconds(reloadTime);
            isGunLoaded = true;
            isGunLoading = false;
        }
    }
}
