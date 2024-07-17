using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera: MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 5f;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized; // Movement direction 

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime); // Move position based on input and speed

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        float zoomAmount = scrollInput * zoomSpeed;

        transform.Translate(0f, 0f, zoomAmount); // Applies zoom amount to the camera's position along the Z-axis
    }
}
