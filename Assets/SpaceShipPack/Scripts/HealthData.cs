using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthData : MonoBehaviour
{

    public float health = 100f;
    public float healthMax = 100f;
    
    public void Damage(float damageAmount) {
        health -= damageAmount;
    }

    public void Heal(float healAmount) {
        health += healAmount;
        if (health > healthMax) health = healthMax;
    }

}
