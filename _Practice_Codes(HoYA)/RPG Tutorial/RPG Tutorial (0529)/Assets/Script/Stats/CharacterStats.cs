using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxhealth = 100;                                                                                     // 오브젝트 최대 체력
    public int currentHealth { get; private set; }                                                                  // 오브젝트 현재 체력

    public Stat damage;                                                                                             // 오브젝트 공격력
    public Stat armor;                                                                                              // 오브젝트 방어력



    private void Awake()
    {
        currentHealth = maxhealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            TakeDamage(10);
    }

    public void TakeDamage(int damage)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }                                                                           // 오브젝트가 데미지를 받을 때 호출되는 함수

    public virtual void Die()
    {
        // 플레이어, 몬스터 등 종류에 따라 죽음이 오버라이딩 됨
        Debug.Log(transform.name + " died");
    }                                                                                    // 오브젝트가 죽을 때 호출되는 함수
}
