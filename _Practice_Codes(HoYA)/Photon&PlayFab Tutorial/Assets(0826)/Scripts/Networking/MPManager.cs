using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MPManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static MPManager Instance                                                    // 퍼블릭 싱글톤 인스턴스
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<MPManager>();
            return instance;
        }
    }
    private static MPManager instance;                                                  // 프라이빗 싱글톤 인스턴스

    public PlayfabAuth auth;                                                            // PlayFab인증 스크립트

    public string gameVersion;                                                          // 게임 버전 스트링
    public Text connectState;                                                           // 연결상태 알림창
    public Text timerText;                                                              // 게임 시작 타이머 텍스트
    public Text VictoryText;                                                            // 승리 정보 텍스트

    public GameObject[] disableOnConnected;                                             // 연결시 비활성화할 오브젝트 모음
    public GameObject[] enableOnConnected;                                              // 연결시 활성화할 오브젝트 모음
    public GameObject[] disableOnJoined;                                                // 방 입장시 비활성화할 오브젝트 모음
    public GameObject[] enableOnJoined;                                                 // 방 입장시 활성화할 오브젝트 모음
    public GameObject[] disableOnRoomFull;                                              // 만석 시 활성화할 오브젝트 모음
    public GameObject[] enableOnRoomFull;                                               // 게임 시작 직전 활성화할 오브젝트 모음(카운터)
    public GameObject[] disableOnGameStart;                                             // 게임 시작시 비활성화할 오브젝트 모음
    public GameObject[] enableOnGameEnds;                                               // 게임 종료시 활성화할 오브젝트 모음

    [HideInInspector]
    public string userName;                                                             // 유저 이름
    [HideInInspector]
    public int currentPlayer = 0;                                                       // 해당 방에 존재하는 플레이어 수

    private byte maxPlayer = 2;                                                         // 한 방 내 최대 플레이어수
    [HideInInspector]
    public List<GameObject> spawnPoints = new List<GameObject>();                       // 스폰 지점 리스트

    [HideInInspector]
    public bool gameStarts = false;                                                     // 게임 시작 여부
    [HideInInspector]
    public bool thisPlayerWon = false;                                                  // 해당 플레이어 승리 여부
    private float timer = 3;                                                            // 타이머 변수


    void Start()
    {
        foreach(Transform child in gameObject.transform)
        {
            spawnPoints.Add(child.gameObject);
        }
    }

    void Update()
    {
        Debug.Log(currentPlayer);

        if(!gameStarts)
        {
            if (currentPlayer == maxPlayer)
            {
                foreach (GameObject disable in disableOnRoomFull)
                    disable.SetActive(false);
                foreach (GameObject enable in enableOnRoomFull)
                    enable.SetActive(true);

                timer -= Time.deltaTime;
                timerText.text = "Starts in : " + (int)timer;
                if (timer < 0)
                {
                    gameStarts = true;
                    foreach (GameObject disable in disableOnGameStart)
                        disable.SetActive(false);
                }
            }
        }     
    }

    private void FixedUpdate()
    {
        connectState.text = "Connection State : " + PhotonNetwork.IsConnected;
    }



    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 내 오브젝트의 변화 작성
            stream.SendNext(currentPlayer);
            stream.SendNext(thisPlayerWon);
        }
        else if (stream.IsReading)
        {
            // 나를 제외한 모든 오브젝트의 변화 감지
            currentPlayer = (int)stream.ReceiveNext();
            thisPlayerWon = (bool)stream.ReceiveNext();
        }
    }

    // 마스터서버에 연결하는 함수
    public void ConnectToMaster()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    // 마스터 서버에 연결된 직후 실행되는 함수
    public override void OnConnectedToMaster()
    {
        foreach (GameObject disable in disableOnConnected)
            disable.SetActive(false);
            
        foreach (GameObject enable in enableOnConnected)
            enable.SetActive(true);
    }

    // 무작위 방에 입장을 시도하는 함수
    public void CreatorJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // 무작위 방 입장 시도를 실패할 때 실행되는 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions rm = new RoomOptions
        {
            MaxPlayers = maxPlayer,
            IsVisible = true
        };

        int rndID = Random.Range(0, 3000);
        PhotonNetwork.CreateRoom("Default: " + rndID, rm, TypedLobby.Default);
    }

    // 방에 입장된 이후 실행되는 함수
    public override void OnJoinedRoom()
    {
        photonView.RPC("AddPlayerCount", RpcTarget.All);

        foreach (GameObject disable in disableOnJoined)
            disable.SetActive(false);
        foreach (GameObject enable in enableOnJoined)
            enable.SetActive(true);

        Vector3 pos = spawnPoints[PhotonNetwork.CountOfPlayers % maxPlayer].transform.position;
        GameObject player = PhotonNetwork.Instantiate("Player", pos, Quaternion.identity, 0);
        player.GetComponent<PlayerInfo>().userName = userName;
    }

    // 승자 설정 함수
    public void SetWinner(string s)
    {
        photonView.RPC("HasWon", RpcTarget.All, s);
    }


    public void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    [PunRPC]
    void AddPlayerCount()
    {
        currentPlayer++;
    }

    [PunRPC]
    void HasWon(string s)
    {
        foreach (GameObject enable in enableOnGameEnds)
            enable.gameObject.SetActive(true);

        VictoryText.text = "'" + s + "' has won the Game";
        thisPlayerWon = true;
    }

}
