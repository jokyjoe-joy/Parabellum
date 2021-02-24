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

    
    void Update()
    {
        
    }

    private void OnShoot(object sender, ShipController.ShootArgs e)
    {
        Gun1.Shoot(Bullet1);
        Gun2.Shoot(Bullet2);
    }
}
