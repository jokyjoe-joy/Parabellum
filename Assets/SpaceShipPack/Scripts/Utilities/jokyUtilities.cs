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

    /// <summary>
    /// Returns whether the given object is visible by the main camera.
    /// </summary>
    /// <param name="target">gameObject of the given target</param>
    public static bool checkIfObjectIsOnScreen(GameObject target)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
            return true;
        else return false;
    }

    /// <summary>
    /// Returns whether the given object is visible by the main camera.
    /// </summary>
    /// <param name="target">Transform of the given target</param>
    public static bool checkIfObjectIsOnScreen(Transform target)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
            return true;
        else return false;
    }

    /// <summary>
    /// Returns whether the given object is visible by the main camera.
    /// </summary>
    /// <param name="screenPos">Screen position of the given target</param>
    public static bool checkIfObjectIsOnScreen(Vector3 screenPos)
    {
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
            return true;
        else return false;
    }

}
