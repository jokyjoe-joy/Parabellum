using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float reloadTime = 5;
    public bool isGunLoaded = true;
    public bool isGunLoading = false;
    public GameObject shootingVFX;
    public float shootingVFXScale = 1;

    public void Shoot(Transform pfBullet, Vector3 initialVelocity, bool isLookingForEnemy=true)
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
                bulletController.Setup(transform.forward, initialVelocity);
            }

            Rocket rocketController = bulletTransform.GetComponent<Rocket>();
            if (rocketController != null)
            {
                rocketController.Setup(transform.forward, initialVelocity, isLookingForEnemy);
            }
        }
    }

    public IEnumerator Reload() {
        if (!isGunLoading) {
            isGunLoading = true;
            yield return new WaitForSeconds(reloadTime);
            isGunLoaded = true;
            isGunLoading = false;
        }
    }
}
