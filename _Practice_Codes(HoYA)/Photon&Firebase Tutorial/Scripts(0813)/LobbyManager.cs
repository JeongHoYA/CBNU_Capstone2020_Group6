using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks                           // 포톤 Pun의 콜백을 인식할 수 있는 모노비헤이비어
{
    private readonly string gameVersion = "1";                                  // 게임 버전 스트링

    public Text connectionInfoText;                                             // 연결정보 텍스트 오브젝트
    public Button joinButton;                                                   // 조인 버튼 오브젝트

    private void Start()
    {
        // 마스터서버 접속 시도
        PhotonNetwork.GameVersion = gameVersion;                                // 포톤에 현재 게임버전 정보 전송
        PhotonNetwork.ConnectUsingSettings();                                   // 포톤에 여러 세팅옵션 정보 전송

        joinButton.interactable = false;                                        // 접속 버튼 비활성화
        connectionInfoText.text = "Connecting to Master Server...";             // 연결정보 작성
    }
    
    public override void OnConnectedToMaster()                                  // 서버 접속 성공시 실행되는 함수
    {
        joinButton.interactable = true;                                         // 접속 버튼 활성화
        connectionInfoText.text = "Online : Connected to Master Server";        // 연결정보 작성
    }
    
    public override void OnDisconnected(DisconnectCause cause)                  // 서버 접속 실패시 실행되는 함수
    {
        joinButton.interactable = false;                                        // 접속 버튼 비활성화
        connectionInfoText.text = "Offline : Connection Disabled";              // 연결정보 및 원인 작성
        connectionInfoText.text += cause.ToString();

        PhotonNetwork.ConnectUsingSettings();                                   // 재접속 시도
    }
    
    public void Connect()                                                       // Join버튼 클릭시 실행할 함수
    {
        joinButton.interactable = false;                                        // 접속 버튼 비활성화

        if(PhotonNetwork.IsConnected)                                           // 접속 성공시 정보작성 및 랜덤방 진입시도
        {
            connectionInfoText.text = "Connecting to Random Room...";       
            PhotonNetwork.JoinRandomRoom();                                 
        }
        else                                                                    // 접속 실패시 정보작성 및 재접속 시도
        {
            connectionInfoText.text = "Offline : Connection Disabled - Try reconnecting...";

            PhotonNetwork.ConnectUsingSettings();
        }
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)   // 방 참가 실패시 실행되는 함수
    {
        connectionInfoText.text = "There is no empty room, Creating new room";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });     // 최대 정원이 2명이고 이름이 null인 새 방 생성
    }
    
    public override void OnJoinedRoom()                                         // 방 참가 성공 혹은 방 생성시 실행되는 함수
    {
        connectionInfoText.text = "Connnected with Room.";
        PhotonNetwork.LoadLevel("Main");                                        // 방장의 방으로 유저들이 이동, 자동 동기화
    }
}