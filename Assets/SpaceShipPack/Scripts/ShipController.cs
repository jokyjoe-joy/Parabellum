using System;
using UnityEngine;

public class ShipController : MonoBehaviour
{

    public float forwardSpeed = 10f;
    public float backwardSpeed = 5f;
    public float RotationSpeed = 1;
    public float MouseSpeed = 5;
    public float MaxSpeed = 100f;
    private new Rigidbody rigidbody;

    public event EventHandler<ShootArgs> Shoot;
    public class ShootArgs : EventArgs
    {
        public Vector3 shootDir;
    }

    private float defaultFOV;
    private float currentFOV;

    void Start()
    {
        // Locking cursor
        Cursor.lockState = CursorLockMode.Locked;
        rigidbody = GetComponent<Rigidbody>();
        defaultFOV = Camera.main.fieldOfView;
    }

    void Update()
    {
        rigidbody.AddRelativeTorque(Input.GetAxis("Mouse Y") * MouseSpeed * 50 * Time.deltaTime, 0, 0);
        rigidbody.AddRelativeTorque(0, Input.GetAxis("Mouse X") * MouseSpeed * 50 * Time.deltaTime, 0);



        var vel = rigidbody.velocity;
       

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
            rigidbody.AddForce(transform.forward * forwardSpeed * 100 * Time.deltaTime);
        }
        if (Input.GetKey("s"))
        {
            rigidbody.AddForce(-transform.forward * backwardSpeed * 100 * Time.deltaTime);
        }
        if (Input.GetKey("a"))
        {
            rigidbody.AddRelativeTorque(0, 0, RotationSpeed / 10);
        }
        if (Input.GetKey("d"))
        {
            rigidbody.AddRelativeTorque(0, 0, -RotationSpeed / 10);
        }
        if (Input.GetKey("space"))
        {
            rigidbody.AddForce(-vel * forwardSpeed * 10 * Time.deltaTime);
            rigidbody.AddTorque(-rigidbody.angularVelocity.normalized * 200 * RotationSpeed * Time.deltaTime);
        }


        if (Input.GetMouseButtonDown(0))
        {
            Vector3 shootDir = transform.forward;
            Shoot?.Invoke(this, new ShootArgs { shootDir = shootDir });
        }
    }
}
