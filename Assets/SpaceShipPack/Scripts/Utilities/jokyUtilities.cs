using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jokyUtilities : MonoBehaviour
{
    public static GameObject GetClosestGameObject(GameObject[] enemies, Transform transform)
    {
		// create bestTarget
        GameObject bestTarget = null;
		
		// create closest distance and set it to infinity
        float closestDistanceSqr = Mathf.Infinity;
		
		// get currentPosition
        Vector3 currentPosition = transform.position;
		
		// go through each given target
        foreach (GameObject potentialTarget in enemies)
        {
			// calculate vector towards target
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
			// getting magnitude of distance (avoiding square root calculation with sqrMagnitude)
            float dSqrToTarget = directionToTarget.sqrMagnitude;
			// if current target is closer than the current closest one
            if (dSqrToTarget < closestDistanceSqr)
            {
				// save this target's distance as closest
                closestDistanceSqr = dSqrToTarget;
				// save this target as the bestTarget
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }

    public static bool checkIfObjectIsOffScreen(Vector3 targetPosition)
    {
        Vector3 fromPosition = Camera.main.transform.position;
        Vector3 targetPositionViewPoint = Camera.main.WorldToViewportPoint(targetPosition);
        return targetPositionViewPoint.x >= 0 && targetPositionViewPoint.x <= 1 && targetPositionViewPoint.y >= 0 && targetPositionViewPoint.y <= 1 && targetPositionViewPoint.z <= 0;
    }
}
