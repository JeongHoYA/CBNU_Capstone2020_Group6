using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBoxStat : BoxStat
{
    PlayerStat player;

    private void Start()
    {
        if (World.Instance.isBuildMode == false)
            player = GameObject.Find("ClearPlayer").GetComponent<PlayerStat>();           
    }

    public override void Die()
    {
        player.LevelClear = true;
        base.Die();
    }
}
