using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class Player : Photon.PunBehaviour
{
    new Rigidbody rigidbody;

    private float x, z;

    public float speed = 20f;

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
}
