using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPointer : MonoBehaviour
{
    public float minSize = 0.2f;
    public float maxSize = 0.5f;
    public Sprite basicSprite;
    public Sprite targetedSprite;
    public Sprite arrowSprite;
    private Vector3 targetPosition;
    private RectTransform pointerRectTransform;
    private Image pointerImage;
    public bool isTargetedSprite;

    private void Awake()
    {
        pointerRectTransform = GetComponent<RectTransform>();
        pointerImage = GetComponent<Image>();
    }

    public void SetTarget(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        if (targetPosition == null) return;

        Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(targetPosition);
        // In case objects are far, still show on UI
        targetScreenPosition.z = 100;

        bool isOnScreen = jokyUtilities.checkIfObjectIsOnScreen(targetScreenPosition);
        // If target is not off-screen, then position pointer on target,
        // while scaling it according the distance between the camera and the target.
        if (isOnScreen) 
        {
            if (isTargetedSprite) pointerImage.sprite = targetedSprite;
            else pointerImage.sprite = basicSprite;


            float x = (targetPosition - Camera.main.transform.position).sqrMagnitude; // usually returns around 300000 - 500000
            x *= maxSize;
            float y = minSize * 300000.0f;
            float sizeByDistance = y / x;
            float size = Mathf.Clamp(sizeByDistance, minSize, maxSize);
            pointerRectTransform.localScale = new Vector3(size,size,size);
            pointerRectTransform.position = targetScreenPosition;
        }
        else
        {
            pointerRectTransform.localScale = new Vector3(1,1,1);

            pointerImage.sprite = arrowSprite;
            
            // stuff is flipping when behind player, avoid it with this
            if (targetScreenPosition.z < 0) targetScreenPosition *= -1;

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0)/2;
            // make 00 the center of the screen instead of bottom left
            targetScreenPosition -= screenCenter;

            // find angle from center of screen to mouse position
            float angle = Mathf.Atan2(targetScreenPosition.y, targetScreenPosition.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = -Mathf.Sin(angle);

            targetScreenPosition = screenCenter + new Vector3(sin*150, cos*150, 0);

            // y = mx+b
            float m = cos / sin;

            Vector3 screenBounds = screenCenter * 0.9f;

            // Check up and down first
            if (cos > 0)
                targetScreenPosition = new Vector3(screenBounds.y/m, screenBounds.y, 0);
            else
                targetScreenPosition = new Vector3(-screenBounds.y/m, -screenBounds.y, 0);
            
            // if out of bounds, get point on appropriate side
            if (targetScreenPosition.x > screenBounds.x) // right
                targetScreenPosition = new Vector3(screenBounds.x, screenBounds.x*m, 0);
            else if (targetScreenPosition.x < -screenBounds.x) // left
                targetScreenPosition = new Vector3(-screenBounds.x, -screenBounds.x*m, 0);
            // else in bounds

            // removing coordinate translation
            targetScreenPosition += screenCenter;

            pointerRectTransform.position = targetScreenPosition;
            pointerRectTransform.rotation = Quaternion.Euler(0, 0, angle*Mathf.Rad2Deg);
        }
    }
}
