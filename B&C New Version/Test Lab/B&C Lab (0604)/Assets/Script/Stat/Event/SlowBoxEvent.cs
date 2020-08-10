using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBoxEvent : MonoBehaviour
{
    public float speed = 2f;

    private void OnTriggerStay(Collider other)
    {
        CharacterStat n = other.transform.root.gameObject.GetComponent<CharacterStat>();
        if (n.isSlowed == false)
        {
            n.SetSpeed(speed);
            n.isSlowed = true;
        }    
    }
    
    private void OnTriggerExit(Collider other)
    {
        CharacterStat n = other.transform.root.gameObject.GetComponent<CharacterStat>();
        if (n.isSlowed == true)
        {
            n.ReturnSpeed();
            n.isSlowed = false;
        }   
    } 
}
