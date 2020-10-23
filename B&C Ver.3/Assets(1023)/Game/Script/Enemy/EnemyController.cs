using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    Vector3 originPosition;

    Animator animator;
    CharController player;
    CharacterController controller;
    MonsterStat monsterStat;

    float outradius = 15f;
    float lookradius = 7f;
    float stopdistacne = 1.5f;

    float rotSpeed = 10f;

    public bool isDead;

    private void Start()
    {
        originPosition = transform.position;
        player = GameObject.Find("ClearPlayer").GetComponent<CharController>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        monsterStat = GetComponent<MonsterStat>();

    }

    private void Update()
    {
        if (transform.position.y < -3 && !monsterStat.isDead)
        {
            monsterStat.isDead = true;
            monsterStat.Die();
        }
            

        float originDistance = Vector3.Distance(originPosition, transform.position);
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);

        if(!isDead && !player.playerStat.isDead)
        {
            if (originDistance <= outradius)
            {
                if (playerDistance <= stopdistacne)
                {
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isAttacking", true);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), rotSpeed * Time.deltaTime);
                }
                else if (playerDistance <= lookradius)
                {
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isAttacking", false);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), rotSpeed * Time.deltaTime);
                    controller.Move(transform.forward * monsterStat.moveSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = originPosition;
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isAttacking", false);
                }
            }
            else
            {
                transform.position = originPosition;
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", false);
            }
        }
        else if(player.playerStat.isDead)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
        }
        else if(isDead)
        {
            animator.SetBool("isDead", true);
        }
    }
}
