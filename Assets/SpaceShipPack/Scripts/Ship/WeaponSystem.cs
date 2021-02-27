using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public Gun Gun1;
    public Gun Gun2;
    public Transform Bullet1;
    public Transform Bullet2;
    private bool isLookingForEnemy = false;

    private void Awake() {
        // In case this is a player ship, attack AI
        ShipAI shipAI = GetComponent<ShipAI>();
        if (shipAI == null) isLookingForEnemy = true;
    }

    private void Update() {
        //GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        //Gun1.transform.parent.transform.LookAt(enemy.transform.position);
        //Gun2.transform.parent.transform.LookAt(enemy.transform.position);
        Debug.DrawLine(Gun1.transform.parent.transform.position, Gun1.transform.parent.transform.forward * 200 + Gun1.transform.parent.transform.position);
        Debug.DrawLine(Gun2.transform.parent.transform.position, Gun2.transform.parent.transform.forward * 200 + Gun2.transform.parent.transform.position);
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
