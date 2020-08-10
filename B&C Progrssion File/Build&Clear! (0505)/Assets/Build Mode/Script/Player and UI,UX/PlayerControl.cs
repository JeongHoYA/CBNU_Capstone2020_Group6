using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public World world;
    public CharacterController controller;

    public Transform groundCheck;
    public float groundDistance = 0.3f;

    public float speed = 3f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;

    Vector3 velocity;

    public bool isGrounded;


    // Update is called once per frame
    void Update()
    {
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (world.CheckForVoxelisSolid(groundCheck.position))
            isGrounded = true;
        else
            isGrounded = false;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
