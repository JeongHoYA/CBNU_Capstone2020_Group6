using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int damage;

    float speed;

    float destroytimer;

    float timer;

    // Start is called before the first frame update
    void Start()
    {
        damage = 10;
        speed = 5f;
        destroytimer = 5f;
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > destroytimer)
        {
            Destroy(this.gameObject);
        }

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        PlayerStat player = other.gameObject.GetComponent<PlayerStat>();
        if (player != null)
        {
            player.TakeDamage(damage);            
        }
        Destroy(this.gameObject);
    }
}
