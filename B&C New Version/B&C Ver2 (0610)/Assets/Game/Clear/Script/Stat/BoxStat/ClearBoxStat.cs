using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBoxStat : BoxStat
{
    public PlayerStat player;

    public override void Die()
    {
        player.LevelClear = true;
        base.Die();
    }
}
