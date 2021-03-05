using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public List<Gun> Guns = new List<Gun>();
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
        //Debug.DrawLine(Gun1.transform.parent.transform.position, Gun1.transform.parent.transform.forward * 200 + Gun1.transform.parent.transform.position);
        //Debug.DrawLine(Gun2.transform.parent.transform.position, Gun2.transform.parent.transform.forward * 200 + Gun2.transform.parent.transform.position);
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
