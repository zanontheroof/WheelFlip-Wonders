using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // The target to orbit around
    public float rotationSpeed = 2.0f; // Speed of camera rotation
    public float zoomSpeed = 2.0f; // Speed of zooming
    public float minDistance = 1.0f; // Minimum zoom distance
    public float maxDistance = 10.0f; // Maximum zoom distance

    private float xRotation = 0;
    private float yRotation = 0;
    private float currentDistance = 5.0f; // Initial zoom distance

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the game window
        Cursor.visible = false; // Hide the cursor
        xRotation = transform.eulerAngles.y;
        yRotation = transform.eulerAngles.x;
    }

    void Update()
    {
        HandleMouseInput();
    }

    void LateUpdate()
    {
        RotateCamera();
    }

    void HandleMouseInput()
    {
        xRotation += Input.GetAxis("Mouse X") * rotationSpeed;
        yRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Limit the vertical rotation angle to avoid camera flipping
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        // Zoom in and out with the mouse scroll wheel
        currentDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }

    void RotateCamera()
    {
        Quaternion rotation = Quaternion.Euler(yRotation, xRotation, 0);
        Vector3 position = rotation * new Vector3(0, 0, -currentDistance) + target.position;

        // Perform a raycast to check for collisions
        RaycastHit hit;
        if (Physics.Linecast(target.position, position, out hit))
        {
            // If a collision is detected, move the camera to the collision point
            transform.position = hit.point;
        }
        else
        {
            // If no collision, update camera position normally
            transform.rotation = rotation;
            transform.position = position;
        }
    }
}
