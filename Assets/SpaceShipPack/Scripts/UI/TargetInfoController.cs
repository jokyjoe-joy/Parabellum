using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetInfoController : MonoBehaviour
{
    private Text targetName;
    private Text targetHealth;
    private GameObject target;
    private ShipController playerShipController;

    private void Awake() {
        targetName = transform.Find("targetName").GetComponent<Text>();
        targetHealth = transform.Find("targetHealth").GetComponent<Text>();
        playerShipController = GameObject.FindGameObjectWithTag("PlayerTag").transform.parent.GetComponent<ShipController>();
    }

    void Update()
    {
        target = playerShipController.currentTarget;
        if (target != null) {
            // Enable children and set text accordingly
            foreach (Transform child in transform) {
                child.gameObject.SetActive(true);
            }
            targetName.text = target.GetComponent<ShipAI>().nameOfAI;
            targetHealth.text = target.GetComponent<HealthData>().health.ToString();
        } else {
            // Disable children
            foreach (Transform child in transform) {
                child.gameObject.SetActive(false);
            }
        }
    }
}
