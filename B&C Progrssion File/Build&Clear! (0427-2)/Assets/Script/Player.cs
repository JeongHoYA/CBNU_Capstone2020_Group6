using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    private Transform cam;                                          // 카메라 트랜스폼
    private World world;                                            // 월드 오브젝트 

    private float horizontal;                                       // 플레이어 좌우 이동
    private float vertical;                                         // 플레이어 전후 이동
    private float mouseHorizontal;                                  // 마우스 좌우 각도
    private float mouseVertical;                                    // 마우스 상하 각도
    private Vector3 velocity;                                       // 플레이어 속도 (속력 + 벡터)

    private float verticalMomentum = 0;                             // 플레이어 추락 속력
    private bool jumpRequest;                                       // 연속점프 방지용 변수

    public bool isGrounded;                                         // 플레이어가 지면에 닿아있나 여부
    public bool isSprinting;                                        // 플레이어 달리기 여부

    public float playerWidth = 0.15f;                               // 플레이어 어께넓이
    public float walkSpeed = 3f;                                    // 플레이어 걷기 속력
    public float sprintSpeed = 6f;                                  // 플레이어 달리기 속력
    public float jumpForce = 5f;                                    // 플레이어 점프력
    public float gravity = -9.8f;                                   // 플레이어 중력가속도

    public Transform highlightBlock;                                // 제거할 복셀의 좌표
    public Transform placeBlock;                                    // 생성할 복셀의 좌표
    public float checkIncrement = 0.1f;                             // 시야 정중앙에 위치한 복셀의 좌표를 파악하는 간격
    public float reach = 8f;                                        // 생성, 제거가 가능한 반경

    public TextMeshProUGUI selectedBlockText;                       // 선택된 복셀에 대한 텍스트 창
    public int selectedBlockIndex = 1;                              // 선택된 복셀의 인덱스 값




    private void Start()
    {
        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();

        Cursor.lockState = CursorLockMode.Locked;
        selectedBlockText.text = (world.blockTypes[selectedBlockIndex].blockName + " 선택됨").ToString();
    }

    private void FixedUpdate()
    {

        CalculateVelocity();
        if (jumpRequest)
            Jump();

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);

    }

    private void Update()
    {
        GetPlayerInputs();
        PlaceCursorBlocks();
    }





    /* 플레이어의 행동 구현 */

    private void GetPlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint"))
            isSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            isSprinting = false;

        if (isGrounded && Input.GetButtonDown("Jump"))
            jumpRequest = true;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll != 0)
        {
            if (scroll > 0)
                selectedBlockIndex++;
            else
                selectedBlockIndex--;

            if (selectedBlockIndex > world.blockTypes.Length - 1)
                selectedBlockIndex = 1;

            if (selectedBlockIndex < 1)
                selectedBlockIndex = world.blockTypes.Length - 1;

            selectedBlockText.text = (world.blockTypes[selectedBlockIndex].blockName + " 선택됨").ToString();
        }

        if(highlightBlock.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
                world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, selectedBlockIndex);

            if (Input.GetMouseButtonDown(1))
                world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);
        }
    }                               // 플레이어 키 입력과 블록 에디팅

    private void CalculateVelocity()
    {
        // 중력가속도 구현
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        // 달리기 구현
        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        // 상하전후좌우 충돌시 속도 상실 구현
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(velocity.y);
    }                             // 플레이어 속도 계산

    void Jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }                                                  // 점프 구현

    private void PlaceCursorBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while (step < reach)
        {
            Vector3 pos = cam.position + (cam.forward * step);

            if(!world.CheckForVoxelisAir(pos) && world.IsVoxelInWorld(pos))
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
    }                             // 월드 내 플레이어의 좌표





    /* 플레이어의 상하전후좌우 충돌 확인 */

    private float checkDownSpeed(float downSpeed)
    {
        if (
            world.CheckForVoxelisSolid(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxelisSolid(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxelisSolid(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxelisSolid(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))
           )
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return downSpeed;
        }
    }
    private float checkUpSpeed(float upSpeed)
    {

        if (
            world.CheckForVoxelisSolid(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxelisSolid(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxelisSolid(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxelisSolid(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth))
           )
            return 0;
        else
            return upSpeed;
    }
    public bool front
    {
        get
        {
            if (
                world.CheckForVoxelisSolid(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
                world.CheckForVoxelisSolid(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
                )
                return true;
            else
                return false;
        }
    }
    public bool back
    {
        get
        {
            if (
                world.CheckForVoxelisSolid(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                world.CheckForVoxelisSolid(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
                )
                return true;
            else
                return false;
        }
    }
    public bool left
    {
        get
        {
            if (
                world.CheckForVoxelisSolid(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxelisSolid(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
                )
                return true;
            else
                return false;
        }
    }
    public bool right
    {
        get
        {
            if (
                world.CheckForVoxelisSolid(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxelisSolid(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
                )
                return true;
            else
                return false;
        }
    }
}
