using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    public Transform[] pfAsteroids;
    public float maxDistance = 100.0f;
    public int amountOfAsteroids = 50;
    public float asteroidSizeMin = 1.0f;
    public float asteroidSizeMax = 5.0f;
    void Start()
    {
        // Randomly select a prefab from pfAsteroids, then instantiate it.
        // After that, set AsteroidManager as parent and set random scale (based on min, max size) 
        // and random rotation.
        for (int i=0; i < amountOfAsteroids; i++) {
            Transform asteroid = pfAsteroids[Random.Range(0, pfAsteroids.Length)];
            Transform asteroidInstance = Instantiate(asteroid, Random.insideUnitSphere * maxDistance, Quaternion.identity);
            asteroidInstance.SetParent(transform);
            float size = Random.Range(asteroidSizeMin, asteroidSizeMax);
            asteroidInstance.localScale = new Vector3(size,size,size);
            asteroidInstance.rotation = Random.rotation;
        }
    }
}
