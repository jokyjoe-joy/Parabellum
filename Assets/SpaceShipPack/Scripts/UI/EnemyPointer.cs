using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointer : MonoBehaviour
{
    public float minSize = 0.2f;
    public float maxSize = 0.6f;
    private Vector3 targetPosition;
    private RectTransform pointerRectTransform;

    private void Awake()
    {
        pointerRectTransform = GetComponent<RectTransform>();
    }

    public void SetTarget(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        if (targetPosition == null) return;

        bool isOffScreen = jokyUtilities.checkIfObjectIsOffScreen(targetPosition);

        // If target is not off-screen, then position pointer on target,
        // while scaling it according the distance between the camera and the target.
        if (isOffScreen) pointerRectTransform.localScale = new Vector3(0,0,0);
        else
        {
            float x = (this.targetPosition - Camera.main.transform.position).sqrMagnitude; // usually returns around 300000 - 500000
            x *= maxSize;
            float y = minSize * 300000.0f;
            float sizeByDistance = y / x;
            float size = Mathf.Clamp(sizeByDistance, minSize, maxSize);
            pointerRectTransform.localScale = new Vector3(size,size,size);
            Vector3 myPos = Camera.main.WorldToScreenPoint(this.targetPosition);
            pointerRectTransform.position = myPos;
        }
    }
}
