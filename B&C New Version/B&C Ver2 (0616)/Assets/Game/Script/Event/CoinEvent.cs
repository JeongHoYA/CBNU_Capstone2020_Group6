using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerStat player = other.transform.root.GetComponent<PlayerStat>();

        if (player != null)
        {
            player.coinCount++;
            Destroy(this.gameObject);
        }
    }
}
