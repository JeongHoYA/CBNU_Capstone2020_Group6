using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{
    public World world;                                                                         // 월드 오브젝트
    public CharacterController controller;                                                      // 캐릭터 컨트롤러 오브젝트
    public Camera cam;                                                                          // 카메라 오브젝트
    public ToolBar toolbar;                                                                     // 툴바 오브젝트

    public Transform groundCheck;                                                               // isGround를 확인할 오브젝트의 트랜스폼
    public float groundDistance = 0.05f;                                                        // isGround 판단 반경
    //public LayerMask groundMask;                                                                // 레이어마스크인 블록에 접촉 시 그라운드 판정

    public bool isGrounded;                                                                     // 플레이어의 발이 땅에 닿아있나 여부
    public bool isSprinting;                                                                    // 플레이어가 뛰고 있나 여부
    public bool isFlying;

    public Transform highlightBlock;                                                            // 제거할 블록의 위치
    public Transform placeBlock;                                                                // 생성할 블록의 위치
    public float checkIncrement = 0.1f;                                                         // 블록 위치 판단 단위
    public float reach = 8f;                                                                    // 블록 제거/생성 가능한 거리

    public float walkSpeed = 5f;                                                                // 플레이어 걷기 속도
    public float sprintSpeed = 10f;                                                             // 플레이어 달리기 속도
    public float jumpHeight = 2f;                                                               // 플레이어 점프 높이
    public float gravity = -9.81f;                                                              // 중력가속도

    Vector3 velocity;                                                                           // 플레이어 속도(속력 + 방향벡터)



    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            world.inUI = !world.inUI;

        if (Input.GetKeyDown(KeyCode.P))
            world.inPause = !world.inPause;


        if(!world.inUI && !world.inPause)
        {
            PlayerMovement();

            placeCursorBlocks();

            if (Input.GetMouseButtonDown(1) && highlightBlock.gameObject.activeSelf)
            {
                DestroyCursorBlocks();
            }
            if (Input.GetMouseButtonDown(0) && toolbar.slots[toolbar.slotIndex].HasItem && highlightBlock.gameObject.activeSelf)
            {
                Vector3 instantiatePos = placeBlock.transform.position;
                if (world.IsVoxelInWorld(instantiatePos))
                {
                    // 오브젝트 생성
                    Instantiate(world.blocks[toolbar.slots[toolbar.slotIndex].itemSlot.blockID].block, instantiatePos, Quaternion.identity, world.GetChunkFromVector3(instantiatePos).transform);
                    // 복셀맵 리스트 값 변경
                    world.GetChunkFromVector3(instantiatePos).EditVoxelMap(instantiatePos, toolbar.slots[toolbar.slotIndex].itemSlot.blockID);
                }
            }// 오브젝트 생성 및 제거 기능  
        }
    }






    private void PlayerMovement()
    {
        /* isGrounded판단 */
        if (world.CheckVoxel(groundCheck.position) != 0)
            isGrounded = true;
        else
            isGrounded = false;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;


        /* 플레이어 전후좌우 움직임 */
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * x + transform.forward * z;


        /* 플레이어 스프린트 & 점프 움직임 */
        if (Input.GetButton("Sprint")) {
            isSprinting = true;
            controller.Move(move * sprintSpeed * Time.deltaTime);
        }
        else {
            isSprinting = false;
            controller.Move(move * walkSpeed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        /* 플레이어 날기 */
        if (isFlying){
            isGrounded = false;
            if (Input.GetKeyDown(KeyCode.F)){
                gravity = -9.81f;
                isFlying = false; 
            }
            if (Input.GetKey(KeyCode.Space)){
                if (isSprinting)
                    velocity.y = sprintSpeed;
                else
                    velocity.y = walkSpeed;
                isGrounded = false;
            }
            else if (Input.GetKey(KeyCode.C)){
                if (isSprinting)
                    velocity.y = -sprintSpeed;
                else
                    velocity.y = -walkSpeed;
            }
            else
                velocity.y = 0;
        }
        else{
            if (Input.GetKeyDown(KeyCode.F)){
                gravity = 0;
                isFlying = true;
            }
        }



        


        /* 중력가속도 적용 */
        velocity.y += gravity * Time.deltaTime;


        /* 최종적인 플레이어 이동 반환 */
        controller.Move(velocity * Time.deltaTime);
    }                                                            // 플레이어 움직임 관리

    
    private void placeCursorBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while(step < reach)
        {
            Vector3 pos = cam.transform.position + (cam.transform.forward * step);

            if (world.CheckVoxel(pos) != 0)
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
    }                                                         // 플레이어가 가리키고 있는 블록 표시
    

    private void DestroyCursorBlocks()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, reach))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            if (hit.transform.tag.Equals("Block"))
            {
                // 오브젝트 파괴
                Destroy(hit.transform.gameObject);
                // 복셀맵 리스트 값 변경
                Vector3 destroypos = hit.transform.position;
                world.GetChunkFromVector3(destroypos).EditVoxelMap(destroypos, 0);
            }
        }
    }                                                       // RayCast를 이용해 플레이어가 지목한 블록 파괴
}
