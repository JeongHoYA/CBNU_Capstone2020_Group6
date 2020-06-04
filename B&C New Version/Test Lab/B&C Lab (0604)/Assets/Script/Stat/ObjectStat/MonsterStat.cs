using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : CharacterStat
{
    public EnemyController monster;

    public override void Die()
    {
        if(!isDead)
        {
            base.Die();
            monster.isDead = true;
            monster.monsterCollider.enabled = false;
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, 0.5f, 0), 0.5f);

            Invoke("DestroyObject", 3f);
        }
    }

    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
