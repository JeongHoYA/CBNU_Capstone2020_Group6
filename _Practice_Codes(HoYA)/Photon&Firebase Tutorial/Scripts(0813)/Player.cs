using Photon.Pun;
using UnityEngine;

public class Player : MonoBehaviourPun
{
    private Rigidbody2D playerRigidbody;                                        // 리지드바디 컴포넌트
    private SpriteRenderer spriteRenderer;                                      // 스프라이트 렌더러 컴포넌트
    
    public float speed = 3f;                                                    // 오브젝트 이동속도

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (photonView.IsMine)
            spriteRenderer.color = Color.blue;
        else
            spriteRenderer.color = Color.red;
        // 로컬 오브젝트는 파란색, 아닐 시 빨간색
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;
        // 로컬 오브젝트가 아닐 시 반환

        float input = InputButton.VerticalInput;

        float distance = input * speed * Time.deltaTime;
        Vector3 targetPosition = transform.position + Vector3.up * distance;

        playerRigidbody.MovePosition(targetPosition);
        // 입력값에 따라 오브젝트 이동
    }
}
