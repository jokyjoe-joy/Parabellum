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

        pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
    }

    private void Update() {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy == null) {
            pointerRectTransform.localScale = new Vector3(0,0,0);
            return;
        }
        targetPosition = enemy.transform.position;

        Vector3 toPosition = targetPosition;
        Vector3 fromPosition = Camera.main.transform.position;

        Vector3 targetPositionViewPoint = Camera.main.WorldToViewportPoint(targetPosition);
        bool isOffScreen = targetPositionViewPoint.x >= 0 && targetPositionViewPoint.x <= 1 && targetPositionViewPoint.y >= 0 && targetPositionViewPoint.y <= 1 && targetPositionViewPoint.z <= 0;

        if (isOffScreen) {
            pointerRectTransform.localScale = new Vector3(0,0,0);
        } else {
            float sizeByDistance = 150.0f / Vector3.Distance(targetPosition, Camera.main.transform.position);
            float size = Mathf.Clamp(sizeByDistance, minSize, maxSize);
            pointerRectTransform.localScale = new Vector3(size,size,size);
            Vector3 myPos = Camera.main.WorldToScreenPoint(targetPosition);
            pointerRectTransform.position = myPos;
        }
    }
}
