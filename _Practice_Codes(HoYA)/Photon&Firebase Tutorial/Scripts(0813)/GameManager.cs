using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance                                          // 퍼블릭 싱글톤 인스턴스
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    private static GameManager instance;                                        // 프라이빗 싱글톤 인스턴스

    public Text scoreText;                                                      // 점수 표시 텍스트
    public Transform[] spawnPositions;                                          // 플레이어들의 생성 위치
    public GameObject playerPrefab;                                             // 플레이어 프리팹
    public GameObject ballPrefab;                                               // 볼 프리팹

    private int[] playerScores;                                                 // 플레이어들의 점수 저장

    private void Start()
    {
        playerScores = new[] { 0, 0 };                                          // 점수 초기화

        SpawnPlayer();
        if (PhotonNetwork.IsMasterClient)
            SpawnBall();
        // 서로의 플레이어를 소환하고 방장은 볼도 소환함
    }

    private void SpawnPlayer()
    {
        int localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Transform spawnPosition = spawnPositions[localPlayerIndex % spawnPositions.Length];
        // 로컬 플레이어 인덱스를 받아와 인덱스에 맞는 스폰 포지션 지정

        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition.position, spawnPosition.rotation);
        // 플레이어를 로컬 오브젝트로 생성한 후 타인에게 해당 오브젝트를 리모트로 생성시켜 줌
    }

    private void SpawnBall()
    {
        PhotonNetwork.Instantiate(ballPrefab.name, Vector2.zero, Quaternion.identity);
        // 로컬 오브젝트로 생성한 후 타인에게 해당 오브젝트를 리모트로 생성시켜 줌
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
        // 내가 방을 나갈 시 내 씬이 로비로 변경됨
    }

    public void AddScore(int playerNumber, int score)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        playerScores[playerNumber - 1] += score;
        photonView.RPC("RPCUpdateScoreText", RpcTarget.All, playerScores[0].ToString(), playerScores[1].ToString());
        // 마스터 서버에서 플레이어 스코어를 계산한 후
        // RPC를 이용해 네트워크상 모든 유저들의 컴퓨터에서 RPCUpdateScoreText 함수 실행
    }

    
    [PunRPC]
    private void RPCUpdateScoreText(string player1ScoreText, string player2ScoreText)
    {
        scoreText.text = $"{player1ScoreText} : {player2ScoreText}";
        // 스코어 텍스트를 해당 스코어텍스트로 변경
    }
}