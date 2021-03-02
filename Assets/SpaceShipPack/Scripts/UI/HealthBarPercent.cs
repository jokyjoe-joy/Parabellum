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
        GameObject player = GameObject.FindGameObjectWithTag("PlayerTag").transform.parent.gameObject;
        playerHealthData = player.GetComponent<HealthData>();
        healthPercentText = GetComponent<Text>();
    }

    void Update()
    {
        healthPercentText.text = (playerHealthData.health / playerHealthData.healthMax * 100).ToString() + "%";
    }
}
