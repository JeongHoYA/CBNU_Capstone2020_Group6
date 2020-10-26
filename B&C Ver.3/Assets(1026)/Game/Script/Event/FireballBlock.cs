using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBlock : MonoBehaviour
{
    public GameObject bullet;

    GameObject player;
    float lookradius = 10f;

    public float attackPeriod = 3.0f;
    float time = 0f;

    float playerDistance;

    // Start is called before the first frame update
    void Start()
    {
        if(!World.Instance.isBuildMode)
        {
            player = GameObject.Find("ClearPlayer");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (World.Instance.isBuildMode)
            return;

        playerDistance = Vector3.Distance(player.transform.position, transform.position);

        time += Time.deltaTime;
        if (time > attackPeriod)
        {
            time = 0.0f;
            if (playerDistance <= lookradius)
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        GameObject bulletObject = Instantiate(bullet);
        bulletObject.transform.position = transform.position + Vector3.up * 0.25f;
        bulletObject.transform.forward = player.transform.position - transform.position;
    }
}
