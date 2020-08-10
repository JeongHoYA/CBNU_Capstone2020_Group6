using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public World world;
    public Transform cam;
    public ToolBar toolbar;

    private float horizontal;
    private float vertical;

    public Transform highlightBlock;                                // 제거할 복셀의 좌표
    public Transform placeBlock;                                    // 생성할 복셀의 좌표
    public float checkIncrement = 0.1f;                             // 시야 정중앙에 위치한 복셀의 좌표를 파악하는 간격
    public float reach = 8f;                                        // 생성, 제거가 가능한 반경

    public CharacterController controller;

    public Transform groundCheck;

    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    Vector3 velocity;

    public bool isGrounded;
    public bool isSprinting;
    public bool isFlying;
    public bool isGhost;



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            world.inUI = !world.inUI;
        }

        if (!world.inUI)
        {
            PlayerInput();

            PlayerMove();

            PlaceCursorBlocks();
        }
    }

    private void PlayerInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Sprint"))
            isSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            isSprinting = false;

        if (world.CheckForVoxelisSolid(groundCheck.position))
            isGrounded = true;
        else
            isGrounded = false;

        if (highlightBlock.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
                if (world.IsVoxelInWorld(placeBlock.position) && toolbar.slots[toolbar.slotIndex].HasItem)
                {
                    world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, toolbar.slots[toolbar.slotIndex].itemSlot.stack.id);
                    toolbar.slots[toolbar.slotIndex].itemSlot.Take(1);
                }

            if (Input.GetMouseButtonDown(1))
                if (world.IsVoxelInWorld(highlightBlock.position))
                    world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);
        }
    }
    

    private void PlayerMove()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        if (isSprinting)
            controller.Move(move * sprintSpeed * Time.deltaTime);
        else
            controller.Move(move * walkSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void PlaceCursorBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while (step < reach)
        {
            Vector3 pos = cam.position + (cam.forward * step);

            if (!world.CheckForVoxelisAir(pos) && world.IsVoxelInWorld(pos))
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;
            }
            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;
        }
        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);
    }                             // 월드 내 플레이어가 Edit할 Voxel 좌표 파악
}
