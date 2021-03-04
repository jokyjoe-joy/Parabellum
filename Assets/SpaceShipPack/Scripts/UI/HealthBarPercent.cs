using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarPercent : MonoBehaviour
{
    private HealthData playerHealthData;
    private Text healthPercentText;
    
    private void Awake()
    {
        playerHealthData = transform.parent.GetComponent<HealthBar>().healthData;
        healthPercentText = GetComponent<Text>();
    }

    void Update()
    {
        healthPercentText.text = (playerHealthData.health / playerHealthData.healthMax * 100).ToString() + "%";
    }
}
