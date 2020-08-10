using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHit : MonoBehaviour
{
    public CharacterStat stat;

    private void OnTriggerEnter(Collider other)
    {
        if(this.gameObject != other.transform.root.gameObject)
        {
            Debug.Log(other.transform.root.name + " attack " + this.gameObject.name);
            this.gameObject.GetComponent<CharacterStat>().TakeDamage(other.transform.root.gameObject.GetComponent<CharacterStat>().damage);
        }   
    }
}
