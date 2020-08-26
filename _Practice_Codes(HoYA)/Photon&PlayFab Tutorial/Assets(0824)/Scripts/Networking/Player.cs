using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class Player : Photon.PunBehaviour
{
    new Rigidbody rigidbody;                                                            // 리지드바디 컴포넌트

    private Vector3 camOffset = new Vector3(0, 2f, -10f);

    private float x, z;                                                                 // 입력축

    public float speed = 20f;                                                           // 물체 속도

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        if (photonView.isMine)
        {
            
        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.isMine)
        {
            x = Input.GetAxisRaw("Horizontal");
            z = Input.GetAxisRaw("Vertical");
        }
        else
        {

        }
    }

    private void FixedUpdate()
    {
        if(photonView.isMine)
        {
            rigidbody.AddForce(new Vector3(x, 0, z) * speed);
        }
    }

    private void LateUpdate()
    {
        if(photonView.isMine)
        {
            Camera.main.transform.position = transform.position + camOffset;
        }
    }
}
