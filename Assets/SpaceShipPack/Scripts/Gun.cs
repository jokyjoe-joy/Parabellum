using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int reloadTime = 5;
    public bool isLoaded = true;
    public bool isLoading = false;

    public void Shoot(Transform pfBullet)
    {
        // Only shoot if gun is loaded
        if (isLoaded) {
            // Get position from where we want to shoot
            var bulletOutPosition = transform.Find("bulletOut").transform.position;
            // Shoot
            Transform bulletTransform = Instantiate(pfBullet, bulletOutPosition, Quaternion.identity);
            isLoaded = false;

            // Init Bullet's script
            Bullet bulletController = bulletTransform.GetComponent<Bullet>();
            if (bulletController != null)
            {
                bulletController.Setup(transform.forward);
            }

            Rocket rocketController = bulletTransform.GetComponent<Rocket>();
            if (rocketController != null)
            {
                rocketController.Setup(transform.forward);
            }
        // Only load gun if not loading already
        } else if (!isLoading) {
            StartCoroutine(Reload());
        }


    }

    IEnumerator Reload() {
        isLoading = true;
        yield return new WaitForSeconds(reloadTime);
        isLoaded = true;
        isLoading = false;
    }
}
