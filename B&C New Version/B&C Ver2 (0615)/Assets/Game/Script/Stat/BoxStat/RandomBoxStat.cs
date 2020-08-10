using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBoxStat : ObjectStat
{
    public GameObject[] potions;

    public override void Die()
    {
        base.Die();

        Vector3 pos = transform.position + new Vector3(0, 0.5f, 0);
        int r = Random.Range(0, potions.Length);

        Instantiate(potions[r], pos, Quaternion.identity);

        Destroy(this.gameObject);
    }
}
