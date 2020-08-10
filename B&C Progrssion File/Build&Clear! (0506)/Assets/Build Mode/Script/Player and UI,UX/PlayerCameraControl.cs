using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraControl : MonoBehaviour
{
    public World world;

    public float mouseSensitivity = 1f;

    public Transform playerBody;

    float xRotation = 0f;

    private void Update()
    {
        if (!world.inUI)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;

            transform.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation, -90, 90), 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
