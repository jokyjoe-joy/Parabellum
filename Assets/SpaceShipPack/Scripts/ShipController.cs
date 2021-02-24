using System;
using UnityEngine;

public class ShipController : MonoBehaviour
{

    public float forwardSpeed = 10f;
    public float backwardSpeed = 10f;
    public float RotationSpeed = 5f;
    public float MouseSpeed = 10f;
    public float MaxSpeed = 100f;
    private new Rigidbody rigidbody;

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
        rigidbody = GetComponent<Rigidbody>();
        ship = GetComponent<Ship>();
        defaultFOV = Camera.main.fieldOfView;
    }

    void Update()
    {
        ship.TurnLeft(Input.GetAxis("Mouse Y"));
        ship.TurnRight(Input.GetAxis("Mouse X"));

        var vel = ship.currentVelocity;
       

        // Dynamic field of view based on speed
        if (vel.magnitude > 10)
        {
            currentFOV = defaultFOV + vel.magnitude/10;
        } else
        {
            currentFOV = 60;
        }

        // If right-clicking, zoom
        if (Input.GetMouseButton(1))
        {
            currentFOV = currentFOV / 2;
        }

        // setting fov
        Camera.main.fieldOfView = currentFOV;


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


            if (Input.GetMouseButtonDown(0))
        {
            Vector3 shootDir = transform.forward;
            Shoot?.Invoke(this, new ShootArgs { shootDir = shootDir });
        }
    }
}
