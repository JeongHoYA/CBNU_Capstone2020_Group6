using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MageEnemyController : MonoBehaviour
{
    public GameObject bullet;

    Vector3 originPosition;

    Animator animator;
    GameObject player;
    CharacterController controller;
    MageMonsterStat monsterStat;

    float outradius = 15f;
    float lookradius = 10f;

    float rotSpeed = 10f;

    public bool isDead;
    bool attack;

    public float attackPeriod;

    float time = 0.0f;

    private void Start()
    {
        originPosition = transform.position;
        player = GameObject.Find("ClearPlayer");//.GetComponent<CharController>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        monsterStat = GetComponent<MageMonsterStat>();

        isDead = false;
        attack = false;
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > attackPeriod )
        {
            time = 0.0f;
            if(attack)
            {
                animator.SetBool("isAttacking", true);
                Shoot();
            }
            animator.SetBool("isAttacking", false);
        }


        if (transform.position.y < -3 && !monsterStat.isDead)
        {
            monsterStat.isDead = true;
            monsterStat.Die();
        }
            

        float originDistance = Vector3.Distance(originPosition, transform.position);
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);

        if(!isDead)
        {
            if (originDistance <= outradius)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), rotSpeed * Time.deltaTime);

                if (playerDistance <= lookradius)
                {                    
                    attack = true;
                }
                else
                {
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isAttacking", false);
                    attack = false;
                }
            }
            else
            {
                transform.position = originPosition;
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", false);
                attack = false;
            }
        }
        /*
        else if(player.playerStat.isDead)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            attack = false;
        }
        */
        else if(isDead)
        {
            animator.SetBool("isDead", true);
        }
    }

    private void Shoot()
    {
        GameObject bulletObject = Instantiate(bullet);
        bulletObject.transform.position = transform.position + Vector3.up * 0.5f;
        bulletObject.transform.forward = transform.forward;
    }
}
