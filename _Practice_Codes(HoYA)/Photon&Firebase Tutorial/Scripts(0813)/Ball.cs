using Photon.Pun;
using UnityEngine;

public class Ball : MonoBehaviourPun
// 해당 오브젝트는 방장의 컴퓨터에만 생성되고 움직임
// 다른 플레이어들은 동기화를 통해 움직임
{
    public bool IsMasterClientLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;
    // 마스터 클라이언트 여부와 컴퓨터에서 생성된 로컬 오브젝트인지 여부 판단
    // true 일 시 해당 스크립트를 실행하는 컴퓨터가 호스트(마스터 클라이언트)이며
    // 해당 스크립트를 갖고 있는 오브젝트가 호스트 측에서 생성된 오브젝트이다.

    private Vector2 direction = Vector2.right;                                      // 공 오브젝트의 방향
    private readonly float speed = 10f;                                             // 공 오브젝트의 이동속도
    private readonly float randomRefectionIntensity = 0.1f;                         // 공 오브젝트의 입/반사각 랜덤상수
    
    private void FixedUpdate()
    {
        if (!IsMasterClientLocal || PhotonNetwork.PlayerList.Length < 2)
            return;
        // 마스터 서버가 아니거나 방이 다 차지 않았으면 공을 움직이지 않음

        float distance = speed * Time.deltaTime;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance); 
        // 해당 오브젝트의 속도와 레이캐스트 선언

        if (hit.collider != null)
        {
            Goalpost goalPost = hit.collider.GetComponent<Goalpost>();
            
            if(goalPost != null)
            {
                if (goalPost.playerNumber == 1)
                    GameManager.Instance.AddScore(2, 1);

                else if (goalPost.playerNumber == 2)
                    GameManager.Instance.AddScore(1, 1);
            }

            direction = Vector2.Reflect(direction, hit.normal);
            direction += Random.insideUnitCircle * randomRefectionIntensity;
        }
        // 다른 물체와 히트할 튕겨나가고 골대면 상대에게 점수도 추가

        transform.position = (Vector2)transform.position + direction * distance;
        // 포지션 지정
    }
}