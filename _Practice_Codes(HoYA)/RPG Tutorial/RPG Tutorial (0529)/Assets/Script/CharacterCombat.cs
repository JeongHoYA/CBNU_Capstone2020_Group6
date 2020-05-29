using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour
{
    CharacterStats myStats;                                                                                         // 캐릭터 스탯 클래스

    public event System.Action OnAttack;                                                                            // '공격중' 시스템 액션

    public float attackSpeed = 1f;                                                                                  // 캐릭터 공격속도
    public float attackCooldown = 0f;                                                                               // 캐릭터 공격 쿨타임 계산 변수
    public float attackDelay = 0.6f;                                                                                // 캐릭터 공격 딜레이


    private void Start()
    {
        myStats = GetComponent<CharacterStats>();
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;
    }



    public void Attack(CharacterStats targetStats)
    {
        if(attackCooldown <= 0)
        {
            StartCoroutine(DoDamage(targetStats, attackDelay));

            if (OnAttack != null)
                OnAttack();

            attackCooldown = 1f / attackSpeed;
        }  
    }                                                               // 설정한 공격속도에 따라 적을 공격

    IEnumerator DoDamage(CharacterStats stats, float delay)
    {
        yield return new WaitForSeconds(delay);

        stats.TakeDamage(myStats.damage.GetValue());
    }
}   
