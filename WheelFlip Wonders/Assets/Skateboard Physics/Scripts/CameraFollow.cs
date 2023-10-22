using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          // The object to follow
    public float followSpeed = 5f;   // Adjust this to control the follow speed

    private void Update()
    {
        if (target == null)
        {
            // If the target is not set, do nothing
            return;
        }

        // Calculate the new camera position
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Smoothly interpolate between the current position and the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
