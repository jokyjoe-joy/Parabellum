using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public Gun Gun1;
    public Gun Gun2;
    public Transform Bullet1;
    public Transform Bullet2;
    public bool isLookingForEnemy = true;

    void Start()
    {
       
    }


    public void ShootGuns(Vector3 initialVelocity)
    {   
        // Shooting, then reloading
        if (Gun1 != null) {Gun1.Shoot(Bullet1, initialVelocity, isLookingForEnemy);}
        if (Gun2 != null) {Gun2.Shoot(Bullet2, initialVelocity, isLookingForEnemy);}
        if (Gun1 != null) {StartCoroutine(Gun1.Reload());}
        if (Gun2 != null) {StartCoroutine(Gun2.Reload());}
    }
}
