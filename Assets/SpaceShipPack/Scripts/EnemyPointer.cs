using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointer : MonoBehaviour
{
    public float minSize = 0.15f;
    public float maxSize = 0.5f;
    private Vector3 targetPosition;
    private RectTransform pointerRectTransform;

    private void Awake() {

        pointerRectTransform = GetComponent<RectTransform>();
    }

    public void SetTarget(Vector3 targetPosition) {
        this.targetPosition = targetPosition;
    }

    private void Update() {
        if (targetPosition == null) {
            return;
        }

        Vector3 toPosition = this.targetPosition;
        Vector3 fromPosition = Camera.main.transform.position;

        Vector3 targetPositionViewPoint = Camera.main.WorldToViewportPoint(this.targetPosition);
        bool isOffScreen = targetPositionViewPoint.x >= 0 && targetPositionViewPoint.x <= 1 && targetPositionViewPoint.y >= 0 && targetPositionViewPoint.y <= 1 && targetPositionViewPoint.z <= 0;

        if (isOffScreen) {
            pointerRectTransform.localScale = new Vector3(0,0,0);
        } else {
            float sizeByDistance = 150.0f / Vector3.Distance(this.targetPosition, Camera.main.transform.position);
            float size = Mathf.Clamp(sizeByDistance, minSize, maxSize);
            pointerRectTransform.localScale = new Vector3(size,size,size);
            Vector3 myPos = Camera.main.WorldToScreenPoint(this.targetPosition);
            pointerRectTransform.position = myPos;
        }
    }
}
