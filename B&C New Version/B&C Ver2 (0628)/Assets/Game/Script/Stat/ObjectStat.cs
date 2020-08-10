using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStat : MonoBehaviour
{

    public int maxhealth = 100;                                                                                      // 오브젝트 최대 체력
    public int currentHealth;                                                                                        // 오브젝트 현재 체력

    public int damage;                                                                                               // 오브젝트 공격력
    public int armor;                                                                                                // 오브젝트 방어력

    public bool isDead;                                                                                              // 오브젝트 사망 여부


    private void Start()
    {
        currentHealth = maxhealth;
    }

    public void TakeDamage(int takendamage)
    {
        takendamage -= armor;
        takendamage = Mathf.Clamp(takendamage, 1, int.MaxValue);

        currentHealth -= takendamage;
        Debug.Log(transform.name + " takes " + takendamage + " damage.");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }                                                                             // 피격 함수


    public virtual void Die()
    {
        Debug.Log(transform.name + " died");
        isDead = true;
    }                                                                                      // 사망 버추얼 함수
}
