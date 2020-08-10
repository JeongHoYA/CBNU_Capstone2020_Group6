using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public World world;
    public Transform playerBody;

    float n = 100f;
    float xRotation = 0f;

    private void Update()
    {
        if(!world.inUI && !world.inPause)
        {
            float mouseX = Input.GetAxis("Mouse X") * n * world.settings.mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * n * world.settings.mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;

            transform.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation, -90, 90), 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
