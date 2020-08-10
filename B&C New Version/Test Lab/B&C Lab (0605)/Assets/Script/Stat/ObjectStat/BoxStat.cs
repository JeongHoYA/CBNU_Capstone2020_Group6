using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxStat : CharacterStat
{
    public override void Die()
    {
        base.Die();
        Destroy(this.gameObject);
    }
}
