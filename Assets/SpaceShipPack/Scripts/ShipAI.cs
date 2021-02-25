using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAI : MonoBehaviour
{
    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private Ship ship;

    // Returns random normalized vector
    private Vector3 getRandomDirection() {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private void Awake() {
        ship = GetComponent<Ship>();
    }
    void Start()
    {
        startingPosition = transform.position;
        roamPosition = startingPosition + getRandomDirection() * Random.Range(10f, 70f);
    }

    private void Update() {
        ship.ThrustForward(1);
    }
}
