using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Security.Permissions;

public class PlayerInfo : MonoBehaviourPun, IPunObservable
{
    public Text healthText;                                                                 // 플레이어 체력 텍스트 오브젝트
    public Text userText;                                                                   // 플레이어 이름 텍스트 오브젝트

    [HideInInspector]
    public string userName;                                                                 // 유저 이름

    private float health = 100;                                                             // 플레이어 현재 체력
    private float minHealth = 0;                                                            // 플레이어 최소 체력
    private float maxHealth = 100;                                                          // 플레이어 최대 체력


    void Start()
    {
        if (photonView.IsMine)
        {
            userText.text = userName;
        }
        else
        {

        }
    }
    
    void Update()
    {
        healthText.text = health.ToString();

        if (health > maxHealth)
            health = maxHealth;
        if (health <= minHealth)
        {
            health = maxHealth;
            transform.position = MPManager.Instance.spawnPoints[Random.Range(0, MPManager.Instance.spawnPoints.Count)].transform.position;
        }
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            // 내 오브젝트의 변화 작성
            stream.SendNext(userName);
            stream.SendNext(health);
        }
        else if(stream.IsReading)
        {
            // 나를 제외한 모든 오브젝트의 변화 감지
            userName = (string)stream.ReceiveNext();
            health = (float)stream.ReceiveNext();
            userText.text = userName;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "DamageZone")
        {
            if (photonView.IsMine)
            {
                photonView.RPC("Damage", RpcTarget.All);
            }
        }
        if (collision.gameObject.tag == "ClearBox")
        {
            if (photonView.IsMine)
            {
                MPManager.Instance.SetWinner(userName);
            }
        }
    }

    [PunRPC]
    void Damage()
    {
        health = health - 20;
    }
}
