using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    Vector3 originPosition;

    public Animator animator;
    public CharController player;
    public Collider monsterCollider;

    float outradius = 15f;
    float lookradius = 7f;
    float stopdistacne = 1.5f;

    float rotSpeed = 3f;
    float moveSpeed = 3f;

    public bool isDead;

    private void Start()
    {
        originPosition = transform.position;
    }

    private void Update()
    {
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
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
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
