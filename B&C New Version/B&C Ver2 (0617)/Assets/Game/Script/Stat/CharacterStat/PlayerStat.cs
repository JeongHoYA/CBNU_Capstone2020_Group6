using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : CharacterStat
{
    public CharController charController;

    public bool LevelClear = false;
    public int coinCount = 0;


    public override void Die()
    {
        if (!LevelClear)
        {
            base.Die();
            Vector3 pos = charController.transform.position;
            charController.transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, 1f, 0), 0.5f);
        }
    }


    public void Clear()
    {
    }
}
