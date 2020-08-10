using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    Vector3 originPosition;

    public Transform player;


    float outradius = 15f;
    float lookradius = 7f;
    float stopdistacne = 1.5f;

    float rotSpeed = 3f;
    float moveSpeed = 3f;

    private void Start()
    {
        originPosition = transform.position;
        Debug.Log("originPosition = " + originPosition.x + " " + originPosition.y + " " + originPosition.z);
    }

    void Update()
    {
        float originDistance = Vector3.Distance(originPosition, transform.position);
        float playerDistance = Vector3.Distance(player.position, transform.position);

        if (originDistance <= outradius)
        {
            if (playerDistance <= stopdistacne)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position), rotSpeed * Time.deltaTime);
            }
            else if (playerDistance <= lookradius)
            {
                //Debug.Log("Start chasing at " + playerDistance);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position), rotSpeed * Time.deltaTime);
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
            else
                transform.position = originPosition;
        }
        else
            transform.position = originPosition;
    }
}
