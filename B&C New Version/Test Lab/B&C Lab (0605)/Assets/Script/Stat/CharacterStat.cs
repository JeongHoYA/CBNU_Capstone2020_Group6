﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    public int maxhealth = 100;                                                                                      // 오브젝트 최대 체력
    public int currentHealth { get; private set; }                                                                   // 오브젝트 현재 체력

    public int damage;                                                                                               // 오브젝트 공격력
    public int armor;                                                                                                // 오브젝트 방어력

    float basicMoveSpeed;                                                                                            // 오브젝트 기본 이동속도
    public float moveSpeed;                                                                                          // 오브젝트 이동속도
    public float jumpHeight;                                                                                         // 오브젝트 점프력

    public bool isSpeedChanged;                                                                                      // 오브젝트 속도변화 여부
    public bool isEnterDotDamageZone;                                                                                // 도트 데미지 존 입장 여부
    public bool isEnterDotHealZone;                                                                                  // 도트 힐 존 입장 여부
    public bool isDead;                                                                                              // 오브젝트 사망 여부

    

    public int dotdamage = 0;                                                                                        // 받는 도트 데미지의 크기
    public float dotTick = 0;                                                                                        // 받는 도트 데미지의 주기
    public float speedChangeRemainTime = 0;                                                                          // 속도 변경까지 남은 시간

    float dotDamageTimer = 0f;                                                                                       // 도트 데미지 타이머
    float dotHealTimer = 0f;                                                                                         // 도트 힐 타이머
    float speedRemainTimer = 0f;                                                                                     // 속도 유지시간 타이머

    private void Awake()
    {
        currentHealth = maxhealth;
        basicMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        if (isEnterDotDamageZone)
            IsEnteredDotDamageZone();

        if (isEnterDotHealZone)
            IsEnteredDotHealZone();

        if(speedChangeRemainTime > 0 && !isSpeedChanged)
            ReturnSpeed(speedChangeRemainTime);

    }

    public void TakeDamage(int damage)
    {
        damage -= armor;
        damage = Mathf.Clamp(damage, 1, int.MaxValue);

        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Die();
            
        }
    }                                                                             // 피격 함수

    public void TakeHeal(int damamge)
    {
        damage = Mathf.Clamp(damage, -100, -1);

        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + -damage + " heal.");

        if (currentHealth >= maxhealth)
        {
            currentHealth = maxhealth;
        }
    }                                                                              // 회복 함수

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
        moveSpeed = Mathf.Clamp(moveSpeed, 0.5f, 12f);
    }                                                                              // 속도 변경 함수

    public void ReturnSpeed(float time)
    {
        if (speedRemainTimer < time)
        {
            speedRemainTimer += Time.deltaTime;
        }
        else
        {
            moveSpeed = basicMoveSpeed;
            speedRemainTimer = 0;
            speedChangeRemainTime = 0;
        }
    }                                                                            // 속도 복구 함수

    public void IsEnteredDotDamageZone()
    {
        if (dotDamageTimer < dotTick)
        {
            dotDamageTimer += Time.deltaTime;
        }
        else
        {
            TakeDamage(dotdamage);
            dotDamageTimer = 0;

            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }
        }
    }                                                                           // 도트데미지 존 입장시 시간당 일정 데미지 피격

    public void IsEnteredDotHealZone()
    {
        if (dotHealTimer < dotTick)
        {
            dotHealTimer += Time.deltaTime;
        }
        else
        {
            TakeHeal(dotdamage);
            dotHealTimer = 0;

            if (currentHealth >= maxhealth)
            {
                currentHealth = maxhealth;
            }
        }
    }                                                                             // 도트 힐 존 입장시 시간당 일정 체력 회복



    public virtual void Die()
    {
        Debug.Log(transform.name + " died");
        isDead = true;
    }                                                                                      // 사망 버추얼 함수
}
