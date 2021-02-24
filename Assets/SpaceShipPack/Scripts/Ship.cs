using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float backwardSpeed = 10f;
    public float RotationSpeed = 5f;
    public float TurnSpeed = 10f;
    public float MaxSpeed = 100f;

    [HideInInspector] public Vector3 currentVelocity;
    [HideInInspector] public int HEALTH = 100;
    private new Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        currentVelocity = rigidbody.velocity;
    }

    public void ThrustForward(float speed)
    {
        rigidbody.AddForce(transform.forward * forwardSpeed * 100 * Time.deltaTime * speed);
    }

    public void ThrustBackwards(float speed)
    {
        rigidbody.AddForce(-transform.forward * backwardSpeed * 100 * Time.deltaTime * speed);
    }

    public void TurnLeft(float speed)
    {
        rigidbody.AddRelativeTorque(speed * TurnSpeed * 50 * Time.deltaTime, 0, 0);
    }

    public void TurnRight(float speed)
    {
        rigidbody.AddRelativeTorque(0, speed * TurnSpeed * 50 * Time.deltaTime, 0);
    }

    public void RollLeft(float speed)
    {
        rigidbody.AddRelativeTorque(0, 0, RotationSpeed * speed * Time.deltaTime * 100);
    }
    public void RollRight(float speed)
    {
        rigidbody.AddRelativeTorque(0, 0, -RotationSpeed * speed * Time.deltaTime * 100);
    }

    public void Stabilise(float speed)
    {
        rigidbody.AddForce(-currentVelocity * forwardSpeed * 10 * Time.deltaTime);
        rigidbody.AddTorque(-rigidbody.angularVelocity.normalized * 200 * RotationSpeed * Time.deltaTime);
    }

    public void Damage(int amount)
    {
        HEALTH -= amount;
        Debug.Log(HEALTH);
        if (HEALTH <= 0)
        {
            Destroy(gameObject);
        }
    }

}
