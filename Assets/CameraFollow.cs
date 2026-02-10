using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    private Vector3 currentVelocity;

    private void LateUpdate() {
        if (target != null) {
            if (target.position.y > transform.position.y) 
            {
                Vector3 targetPosition = new Vector3(transform.position.x, target.position.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
            }
        }
    }
}
