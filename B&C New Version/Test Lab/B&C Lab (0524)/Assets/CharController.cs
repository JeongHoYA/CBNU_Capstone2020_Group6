using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    public CharacterController controller;
    public GameObject camPivot;
    public Camera clearCamera;

    float moveSpeed = 4f;
    float jumpHeight = 1.5f;
    float gravity = -9.81f;

    float u = 0, r = 0;
    Vector3 forward, right, up;

    bool jump;



    private void Start()
    {
        forward = clearCamera.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }


    private void Update()
    {
        PlayerInput();
        Move();
    }

    private void LateUpdate()
    {
        camPivot.transform.localEulerAngles = -transform.localEulerAngles;
    }

    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            u = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            u = -1;
        else
            u = 0;

        if (Input.GetKey(KeyCode.RightArrow))
            r = 1;
        else if (Input.GetKey(KeyCode.LeftArrow))
            r = -1;
        else
            r = 0;

        if (Input.GetKeyDown(KeyCode.Space))
            jump = true;
        else
            jump = false;
    }

    private void Move()
    {
        if (u != 0 && r != 0)
        {
            u *= 0.7071f;
            r *= 0.7071f;
        }
        
        if(u != 0 || r != 0)
        {
            Vector3 rightMovement = right * Time.deltaTime * r;
            Vector3 forwardMoveMent = forward * Time.deltaTime * u;

            Vector3 heading = Vector3.Normalize(rightMovement + forwardMoveMent);
            controller.Move(heading * moveSpeed * Time.deltaTime);
        }
        up.y += gravity * Time.deltaTime;
        controller.Move(up * Time.deltaTime);

        if(jump && controller.isGrounded)
        {
            up.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
