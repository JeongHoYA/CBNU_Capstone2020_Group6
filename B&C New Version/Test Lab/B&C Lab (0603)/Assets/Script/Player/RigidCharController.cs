using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidCharController : MonoBehaviour
{
    private Rigidbody playerBody;                                                           // 플레이어 리지드바디 스크립트
    public Transform camPivot;                                                              // 카메라 피벗 트랜스폼
    public Camera clearCamera;                                                              // 카메라 오브젝트
    public LayerMask groundMask;                                                            // 바닥 확인용 레이어마스크

    float moveSpeed = 4f;                                                                   // 플레이어 속도

    float u = 0, r = 0;                                                                     // 플레이어 입력 (u : 전후, r : 좌우)
    Vector3 forward, right, rotation;                                                       // 플레이어 이동용 벡터

    bool jump;                                                                              // 플레이어의 점프 입력 여부
    bool dash;                                                                              // 플레이어 대쉬 여부
    public bool Grounded;


    private void Start()
    {
        playerBody = GetComponent<Rigidbody>();

        /* 쿼터뷰 형식을 위한 로테이션 설정 */
        forward = clearCamera.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

    
    private void FixedUpdate()
    {
        PlayerInput();
        PlayerRotate();
        PlayerMove();

        if (isGrounded())
            Grounded = true;
        else
            Grounded = false;

    }

    private void LateUpdate()
    {
        /* 카메라 회전 고정 */
        camPivot.localEulerAngles = -transform.localEulerAngles;
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

        if (Input.GetKey(KeyCode.Space))
            jump = true;
        else
            jump = false;

        if (Input.GetKeyDown(KeyCode.LeftShift))
            dash = true;
        else
            dash = false;
    }

    private void PlayerRotate()
    {
        if (u == 1)
        {
            if (r == 1)
                rotation = new Vector3(0, 90, 0);
            else if (r == -1)
                rotation = new Vector3(0, 0, 0);
            else
                rotation = new Vector3(0, 45, 0);
        }
        else if (u == -1)
        {
            if (r == 1)
                rotation = new Vector3(0, 180, 0);
            else if (r == -1)
                rotation = new Vector3(0, -90, 0);
            else
                rotation = new Vector3(0, -135, 0);
        }
        else
        {
            if (r == 1)
                rotation = new Vector3(0, 135, 0);
            else if (r == -1)
                rotation = new Vector3(0, -45, 0);
        }
        transform.localEulerAngles = rotation;
    }

    private void PlayerMove()
    {
        Vector3 heading = new Vector3();

        if (u != 0 && r != 0)
        {
            u *= 0.7071f;
            r *= 0.7071f;
        }

        if (jump && isGrounded())
        {
            playerBody.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }

        if (u != 0 || r != 0)
        {
            Vector3 rightMovement = right * Time.deltaTime * r;
            Vector3 forwardMoveMent = forward * Time.deltaTime * u;
            Vector3 downMovement = 

            heading = Vector3.Normalize(rightMovement + forwardMoveMent);            
        }
        playerBody.velocity = heading * moveSpeed;
    }                                                            // 플레이어 움직임 설정

    private bool isGrounded()
    {
        return Physics.CheckSphere(camPivot.position, 0.1f, groundMask);
    }                                                            // 플레이어 isGrounded 여부

}
