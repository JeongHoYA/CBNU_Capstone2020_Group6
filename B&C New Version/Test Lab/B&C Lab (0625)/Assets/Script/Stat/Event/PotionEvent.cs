using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionEvent : MonoBehaviour
{
    int speed = 1;
    int attack = 10;
    int armor = 5;


    private void OnTriggerEnter(Collider other)
    {
        PlayerStat player = other.transform.root.GetComponent<PlayerStat>();

        if (player != null)
        {
            if (name.Contains("Bottle_Armor"))
                player.armor += armor;
            else if (name.Contains("Bottle_Attack"))
                player.damage += attack;
            else if (name.Contains("Bottle_Speed"))
                player.moveSpeed += speed;
            else
                Debug.Log("It's just a juice...");

            Debug.Log(this.name);
            Destroy(this.gameObject);
        }
    }
}
