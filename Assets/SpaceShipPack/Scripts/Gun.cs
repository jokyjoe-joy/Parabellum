using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Gun : MonoBehaviour
{

    [SerializeField] private Transform pfBullet;

    void Start()
    {
        // adding onShoot to shipController's onShoot event
        ShipController shipcontroller = transform.parent.parent.GetComponent<ShipController>();
        shipcontroller.Shoot += OnShoot;
    }

    private void OnShoot(object sender, ShipController.ShootArgs e)
    {
        // Get position from where we want to shoot
        var bulletOutPosition = transform.Find("bulletOut").transform.position;
        // Shoot
        Transform bulletTransform = Instantiate(pfBullet, bulletOutPosition, Quaternion.identity);

        // Init Bullet's script
        //bulletTransform.GetComponent<Bullet>().Setup(e.shootDir);
        bulletTransform.GetComponent<Bullet>().Setup(transform.forward);

    }
}
