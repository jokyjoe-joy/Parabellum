using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 10;
    private Vector3 shootDir;

    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
    }

    private void Update()
    {
        // Move in given direction
        transform.position += shootDir * Time.deltaTime * moveSpeed * 150;
    }
}
