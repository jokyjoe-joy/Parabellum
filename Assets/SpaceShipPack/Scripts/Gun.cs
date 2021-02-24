using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public void Shoot(Transform pfBullet)
    {
        // Get position from where we want to shoot
        var bulletOutPosition = transform.Find("bulletOut").transform.position;
        // Shoot
        Transform bulletTransform = Instantiate(pfBullet, bulletOutPosition, Quaternion.identity);

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

    }
}
