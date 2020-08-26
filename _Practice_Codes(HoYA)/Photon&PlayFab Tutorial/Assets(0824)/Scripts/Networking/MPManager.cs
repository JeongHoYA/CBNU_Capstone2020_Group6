using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class MPManager : Photon.MonoBehaviour
{
    public PlayfabAuth auth;                                                            // PlayFab인증 스크립트

    public string gameVersion;                                                          // 게임 버전 스트링
    public Text connectState;                                                           // 연결상태 알림창

    public GameObject[] disableOnConnected;                                             // 연결시 비활성화할 오브젝트 모음
    public GameObject[] enableOnConnected;                                              // 연결시 활성화할 오브젝트 모음
    public GameObject[] disableOnJoined;                                                // 방 입장시 비활성화할 오브젝트 모음

    public string userName;                                                             // 유저 이름

    private List<GameObject> spawnPoints = new List<GameObject>();                      // 스폰 지점 리스트


    void Start()
    {
        foreach(Transform child in gameObject.transform)
        {
            spawnPoints.Add(child.gameObject);
        }
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        connectState.text = PhotonNetwork.connectionStateDetailed.ToString();
    }

    public void ConnectToMaster()
    {
        //PhotonNetwork.connectionStateDetailed();
        PhotonNetwork.ConnectUsingSettings(gameVersion);
    }

    public virtual void OnConnectedToMaster()
    {
        foreach (GameObject disable in disableOnConnected)
            disable.SetActive(false);
            
        foreach (GameObject enable in enableOnConnected)
            enable.SetActive(true);
    }

    public void CreatorJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnPhotonRandomJoinFailed()
    {
        RoomOptions rm = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true
        };

        int rndID = Random.Range(0, 3000);
        PhotonNetwork.CreateRoom("Default: " + rndID, rm, TypedLobby.Default);
    }

    public virtual void OnJoinedRoom()
    {
        foreach (GameObject disable in disableOnJoined)
            disable.SetActive(false);

        Vector3 pos = spawnPoints[PhotonNetwork.countOfPlayers - 1].transform.position;
        GameObject player = PhotonNetwork.Instantiate("Player", pos, Quaternion.identity, 0);
        player.GetComponent<PlayerInfo>().userName = userName;
    }
}
