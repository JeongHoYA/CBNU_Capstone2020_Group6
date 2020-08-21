using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using System.Security.Permissions;

public class PlayerHealth : Photon.MonoBehaviour
{
    public Text healthText;

    private float health = 100;
    private float minHealth = 0;
    private float maxHealth = 100;


    void Start()
    {
        
    }
    
    void Update()
    {
        healthText.text = health.ToString();

        if (health > maxHealth)
            health = maxHealth;
        if (health < minHealth)
        {
            health = minHealth;
            // 사망 메소드
        }
    }
    

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            // 내 오브젝트의 변화 작성
            stream.SendNext(health);
        }
        else if(stream.isReading)
        {
            // 나를 제외한 모든 오브젝트의 변화 감지
            health = (float)stream.ReceiveNext();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "DamageZone")
        {
            if (photonView.isMine)
            {
                photonView.RPC("Damage", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    void Damage()
    {
        health = health - 20;
    }
}
