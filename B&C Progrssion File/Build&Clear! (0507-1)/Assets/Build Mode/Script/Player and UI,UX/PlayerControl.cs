using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerControl : MonoBehaviour
{
    public World world;                                             // 월드 오브젝트
    public Transform cam;                                           // 카메라 트랜스폼
    public ToolBar toolbar;                                         // 툴바 오브젝트
    public CharacterController controller;                          // 캐릭터 컨트롤러 오브젝트
    public Transform groundCheck;                                   // 그라운드체크 트랜스폼
       
    private float horizontal;                                       // 캐릭터 좌우
    private float vertical;                                         // 캐릭터 전후

    public Transform highlightBlock;                                // 제거할 복셀의 좌표
    public Transform placeBlock;                                    // 생성할 복셀의 좌표
    public float checkIncrement = 0.1f;                             // 시야 정중앙에 위치한 복셀의 좌표를 파악하는 간격
    public float reach = 8f;                                        // 생성, 제거가 가능한 반경

    public float walkSpeed = 3f;                                    // 걷기 속도
    public float sprintSpeed = 6f;                                  // 달리기 속도
    public float jumpHeight = 2f;                                   // 점프 높이
    public float gravity = -9.81f;                                  // 중력가속도

    Vector3 velocity;                                               // 플레이어 속도(속력 + 방향벡터)

    public bool isGrounded;                                         // 플레이어가 지면 위에 서있나 여부
    public bool isSprinting;                                        // 플레이어 달리기 여부
    public bool isFlying = false;                                   // 플레이어 날기 여부
    public bool isGhost = false;                                    // 플레이어 고스트 여부



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

        PlayerFlyInput();

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
    

    private void PlayerFlyInput()
    {
        if (isFlying)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                gravity = -9.81f;
                isFlying = false;
            }
            /*
            if (Input.GetKeyDown(KeyCode.G) && isGhost == false)
            {
                isGhost = true;
            }
                
            else if (Input.GetKeyDown(KeyCode.G) && isGhost == true)
            {
                isGhost = false;
            }
            */
        }
        else
        {
            isGhost = false;
            if (Input.GetKeyDown(KeyCode.F))
            {
                gravity = 0;
                isFlying = true;
            }
        }
    }


    private void PlayerMove()
    {
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        if (isSprinting)
            controller.Move(move * sprintSpeed * Time.deltaTime);
        else
            controller.Move(move * walkSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (isFlying)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (isSprinting)
                    velocity.y = 6f;
                else
                    velocity.y = 3f;
                isGrounded = false;
            }
            else if (Input.GetKey(KeyCode.C))
            {
                if (isSprinting)
                    velocity.y = -6f;
                else
                    velocity.y = -3f;
            }
            else
                velocity.y = 0;
        }
        else
        {
            if (isGrounded && velocity.y < 0)
                velocity.y = -1f;
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
