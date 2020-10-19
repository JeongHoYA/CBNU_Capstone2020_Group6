using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageMonsterStat : CharacterStat
{
    public MageEnemyController monster;

    public override void Die()
    {
        base.Die();
        monster.isDead = true;
        monster.transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, 0.5f, 0), 0.5f);

        Destroy(this.gameObject, 3f);
    }
}
