using System;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public event EventHandler<ShootArgs> Shoot;
    public class ShootArgs : EventArgs
    {
        public Vector3 shootDir;
    }

    private float defaultFOV;
    private float currentFOV;
    private Ship ship;

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
            Vector3 shootDir = transform.forward;
            Shoot?.Invoke(this, new ShootArgs { shootDir = shootDir });
        }
    }

    private void FixedUpdate() {
        CheckMovementControls();
        AdjustFOVOnSpeed();
    }

    private void CheckMovementControls()
    {
        ship.TurnLeft(Input.GetAxis("Mouse Y"));
        ship.TurnRight(Input.GetAxis("Mouse X"));

        if (Input.GetKey("w"))
        {
            ship.ThrustForward(1);
        }
        if (Input.GetKey("s"))
        {
            ship.ThrustBackwards(1);
        }
        if (Input.GetKey("a"))
        {
            ship.RollLeft(1);
        }
        if (Input.GetKey("d"))
        {
            ship.RollRight(1);
        }
        if (Input.GetKey("space"))
        {
            ship.Stabilise(1);
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
