using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharController : MonoBehaviour
{
    public CharacterController controller;                                                  // 플레이어 컨트롤러 스크립트
    public Animator animator;                                                               // 에니메이터 스크립트
    public Transform camPivot;                                                              // 카메라 피벗 트랜스폼
    public Camera clearCamera;                                                              // 카메라 오브젝트

    public Collider spine;                                                                  // 창 콜라이더

    public LayerMask groundMask;                                                            // 바닥 확인용 레이어마스크

    float moveSpeed = 4f;                                                                   // 플레이어 속도
    float jumpHeight = 1.5f;                                                                // 플레이어 점프 높이
    float gravity = -9.81f;                                                                 // 중력가속도

    float u = 0, r = 0;                                                                     // 플레이어 입력 (u : 전후, r : 좌우)
    Vector3 forward, right, up, rotation;                                                   // 플레이어 이동용 벡터

    bool jump;                                                                              // 플레이어의 점프 입력 여부
    bool attack;                                                                            // 플레이어 공격 여부


    private void Start()
    {
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
        PlayerAnimation();
    }

    private void LateUpdate()
    {
        /* 카메라 회전 고정 */
        camPivot.localEulerAngles = -transform.localEulerAngles;
    }


    /* 플레이어 조작, 이동 관련 함수 */
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

        if (Input.GetKey(KeyCode.D))
            attack = true;
        else
            attack = false;
    }                                                           // 플레이어 키 입력

    private void PlayerRotate()
    {
        if (u == 1) {
            if(r == 1)
                rotation = new Vector3(0, 90, 0);
            else if(r == -1)
                rotation = new Vector3(0, 0, 0);
            else
                rotation = new Vector3(0, 45, 0);
        }
        else if (u == -1) {
            if (r == 1)
                rotation = new Vector3(0, 180, 0);
            else if (r == -1)
                rotation = new Vector3(0, -90, 0);
            else
                rotation = new Vector3(0, -135, 0);
        }
        else {
            if (r == 1)
                rotation = new Vector3(0, 135, 0);
            else if (r == -1)
                rotation = new Vector3(0, -45, 0);
        }
        transform.localEulerAngles = rotation;
    }                                                          // 플레이어 회전 설정

    private void PlayerMove()
    {
        if (u != 0 && r != 0) {
            u *= 0.7071f;
            r *= 0.7071f;
        }
        
        if(u != 0 || r != 0) {
            Vector3 rightMovement = right * Time.deltaTime * r;
            Vector3 forwardMoveMent = forward * Time.deltaTime * u;

            Vector3 heading = Vector3.Normalize(rightMovement + forwardMoveMent);
            controller.Move(heading * moveSpeed * Time.deltaTime);
        }

        if(isGrounded() && up.y < 0)
        {
            up.y = -2f;
        }

        if(jump && isGrounded()) {
            up.y = Mathf.Sqrt(jumpHeight * -4f * gravity);
        }

        if (attack)
            spine.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        else
            spine.gameObject.GetComponent<CapsuleCollider>().enabled = false;

        up.y += gravity * Time.deltaTime;
        controller.Move(up * Time.deltaTime);
    }                                                            // 플레이어 움직임 설정

    private void PlayerAnimation()
    {
        if (isGrounded())
            animator.SetBool("isJumping", true);
        else
            animator.SetBool("isJumping", false);

        if(r != 0 || u != 0)
            animator.SetBool("isRunning", true);
        else
            animator.SetBool("isRunning", false);

        if (attack)
            animator.SetBool("isAttacking", true);
        else
            animator.SetBool("isAttacking", false);
    }                                                       // 플레이어 애니메이션 설정



    /* 플레이어 관련 파라미터 반환 함수 */
    private bool isGrounded()
    {
        return Physics.CheckSphere(camPivot.position, 0.1f, groundMask);
    }                                                            // 플레이어 isGrounded 여부
}
