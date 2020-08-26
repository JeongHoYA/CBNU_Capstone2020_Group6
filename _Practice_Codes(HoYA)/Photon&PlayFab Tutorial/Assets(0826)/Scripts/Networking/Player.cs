using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    new Rigidbody rigidbody;                                                            // 리지드바디 컴포넌트

    private Vector3 camOffset = new Vector3(0, 2f, -10f);

    private float x, z;                                                                 // 입력축

    public float speed = 20f;                                                           // 물체 속도

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            
        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            if (!MPManager.Instance.gameStarts || MPManager.Instance.thisPlayerWon)
                return;

            x = Input.GetAxisRaw("Horizontal");
            z = Input.GetAxisRaw("Vertical");
        }
        else
        {

        }
    }

    private void FixedUpdate()
    {
        if(photonView.IsMine)
        {
            rigidbody.AddForce(new Vector3(x, 0, z) * speed);
        }
    }

    private void LateUpdate()
    {
        if(photonView.IsMine)
        {
            Camera.main.transform.position = transform.position + camOffset;
        }
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 내 오브젝트의 변화 작성
            
        }
        else if (stream.IsReading)
        {
            // 나를 제외한 모든 오브젝트의 변화 감지
            
        }
    }
}
