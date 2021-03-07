using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetInfoController : MonoBehaviour
{
    private Text targetName;
    private Text targetHealth;
    private Text targetDistance;
    private GameObject target;
    private ShipController playerShipController;

    private void Awake()
    {
        targetName = transform.Find("targetName").GetComponent<Text>();
        targetHealth = transform.Find("targetHealth").GetComponent<Text>();
        targetDistance = transform.Find("targetDistance").GetComponent<Text>();
        playerShipController = GameObject.FindGameObjectWithTag("PlayerTag").transform.parent.GetComponent<ShipController>();
    }

    void Update()
    {
        target = playerShipController.currentTarget;
        if (target != null)
        {
            // Enable children and set text accordingly
            foreach (Transform child in transform) child.gameObject.SetActive(true);
            targetName.text = target.GetComponent<ShipAI>().nameOfAI;
            HealthData targetHealthData = target.GetComponent<HealthData>();
            targetHealth.text = string.Concat(Mathf.RoundToInt(targetHealthData.health / targetHealthData.healthMax * 100).ToString(), "%");
            targetDistance.text = string.Concat(Mathf.RoundToInt(Vector3.Distance(playerShipController.transform.position, target.transform.position)).ToString(), " m");
        
        } 
        else 
        {
            // Disable children
            foreach (Transform child in transform) child.gameObject.SetActive(false);
        }
    }
}
