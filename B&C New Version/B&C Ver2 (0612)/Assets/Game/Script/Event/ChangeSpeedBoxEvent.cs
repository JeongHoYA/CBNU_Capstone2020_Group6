using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeedBoxEvent : MonoBehaviour
{
    public float speed = 4f;
    public float remainTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        CharacterStat n = other.transform.root.gameObject.GetComponent<CharacterStat>();
        if (n.isSpeedChanged == false)
        {
            n.isSpeedChanged = true;
            n.SetSpeed(speed);
            n.speedChangeRemainTime = remainTime;
        }    
    }
    
    private void OnTriggerExit(Collider other)
    {
        CharacterStat n = other.transform.root.gameObject.GetComponent<CharacterStat>();
        if (n.isSpeedChanged == true)
        {
            n.isSpeedChanged = false;
            n.speedChangeRemainTime = remainTime; 
        }   
    } 
}
