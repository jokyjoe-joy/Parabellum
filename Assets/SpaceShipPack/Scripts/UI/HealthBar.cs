using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public HealthData healthData;
    private Transform bar;
    void Start()
    {
        bar = transform.Find("Bar");
    }

    private void Update()
    {
        setSize(healthData.health / healthData.healthMax);
    }

    public void setSize(float sizeNormalized)
    {
        bar.localScale = new Vector3(sizeNormalized, 1f);
    }
}
