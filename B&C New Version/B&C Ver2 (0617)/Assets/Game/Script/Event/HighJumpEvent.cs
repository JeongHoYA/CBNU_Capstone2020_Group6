using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighJumpEvent : MonoBehaviour
{
    public float jumpHeight = 12f;
    float originJumpheight = 0;

    private void OnTriggerEnter(Collider other)
    {
        CharController c = other.gameObject.GetComponent<CharController>();
        PlayerStat p = other.gameObject.GetComponent<PlayerStat>();
        originJumpheight = p.jumpHeight;
        p.jumpHeight = jumpHeight;
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerStat p = other.gameObject.GetComponent<PlayerStat>();
        p.jumpHeight = originJumpheight;
    }
}
