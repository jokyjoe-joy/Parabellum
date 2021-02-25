using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public Gun Gun1;
    public Gun Gun2;
    public Transform Bullet1;
    public Transform Bullet2;

    void Start()
    {
        // adding onShoot to shipController's onShoot event
        ShipController shipcontroller = transform.GetComponent<ShipController>();
        shipcontroller.Shoot += OnShoot;
    }


    private void OnShoot(object sender, ShipController.ShootArgs e)
    {   
        // Shooting, then reloading
        if (Gun1 != null) {Gun1.Shoot(Bullet1);}
        if (Gun2 != null) {Gun2.Shoot(Bullet2);}
        if (Gun1 != null) {StartCoroutine(Gun1.Reload());}
        if (Gun2 != null) {StartCoroutine(Gun2.Reload());}
    }
}
