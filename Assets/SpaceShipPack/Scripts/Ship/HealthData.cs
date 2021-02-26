using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthData : MonoBehaviour
{

    public float health = 100f;
    public float healthMax = 100f;
    [HideInInspector] public UnityEvent onDamage;
    
    public void Damage(float damageAmount) {
        health -= damageAmount;
        onDamage.Invoke();
    }

    public void Heal(float healAmount) {
        health += healAmount;
        if (health > healthMax) health = healthMax;
    }

}
