using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    public int maxhealth = 100;                                                                                     // 오브젝트 최대 체력
    public int currentHealth { get; private set; }                                                                  // 오브젝트 현재 체력

    public int damage;                                                                                              // 오브젝트 공격력


    private void Awake()
    {
        currentHealth = maxhealth;
    }

    private void Start()
    {
        
    }

    
    private void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    public virtual void Die()
    {
        Debug.Log(transform.name + " died");
        //Destroy(this.gameObject);
    }
}
