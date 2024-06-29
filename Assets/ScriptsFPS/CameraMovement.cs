using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up, mouseX * turnSpeed * Time.deltaTime);

            float rotateAmount = -mouseY * turnSpeed * Time.deltaTime;
            float desiredAngle = transform.eulerAngles.x + rotateAmount;
            if (desiredAngle < 180f)
                desiredAngle = Mathf.Clamp(desiredAngle, 0f, 80f);
            else
                desiredAngle = Mathf.Clamp(desiredAngle, 360f - 80f, 360f);
            transform.rotation = Quaternion.Euler(desiredAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
