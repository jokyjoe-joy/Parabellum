using System;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    private float defaultFOV;
    private float currentFOV;
    private Ship ship;
    private WeaponSystem weaponSystem;

    private void Awake() {
        weaponSystem = GetComponent<WeaponSystem>();
    }

    void Start()
    {
        // Locking cursor
        Cursor.lockState = CursorLockMode.Locked;
        ship = GetComponent<Ship>();
        defaultFOV = Camera.main.fieldOfView;
    }

    void Update()
    {
        // Create Shoot event on Left Click
        if (Input.GetMouseButtonDown(0))
        {
            weaponSystem.ShootGuns(ship.currentVelocity);
        }
    }

    private void FixedUpdate() {
        CheckMovementControls();
        AdjustFOVOnSpeed();
    }

    private void CheckMovementControls()
    {
        ship.LookUpDown(Input.GetAxis("Mouse Y"));
        ship.LookLeftRight(Input.GetAxis("Mouse X"));

        if (Input.GetKey("w"))
        {
            ship.ThrustForward();
        }
        if (Input.GetKey("s"))
        {
            ship.ThrustBackwards();
        }
        if (Input.GetKey("a"))
        {
            ship.RollLeft();
        }
        if (Input.GetKey("d"))
        {
            ship.RollRight();
        }
        if (Input.GetKey("space"))
        {
            ship.Stabilise();
        }
    }

    private void AdjustFOVOnSpeed()
    {
        // Dynamic field of view based on speed
        if (ship.currentVelocity.magnitude > 10)
        {
            currentFOV = defaultFOV + ship.currentVelocity.magnitude / 10;
        }
        else
        {
            currentFOV = 60;
        }

        // If right-clicking, zoom
        if (Input.GetMouseButton(1))
        {
            currentFOV /= 2;
        }

        // setting fov
        Camera.main.fieldOfView = currentFOV;
    }
}
