using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthData : MonoBehaviour
{
    public float health = 100f;
    public float healthMax = 100f;
    [HideInInspector] public UnityEvent onDamage;
    
    public void Damage(float damageAmount)
    {
        health -= damageAmount;
        if (health < 0) health = 0;
        onDamage.Invoke();
    }

    public void Heal(float healAmount)
    {
        health += healAmount;
        if (health > healthMax) health = healthMax;
    }

}
