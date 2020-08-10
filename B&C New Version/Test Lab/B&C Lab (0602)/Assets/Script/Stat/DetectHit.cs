using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHit : MonoBehaviour
{
    public CharacterStat stat;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(this.name +  " hit by " + other.transform.root.name);
        this.gameObject.GetComponent<CharacterStat>().TakeDamage(other.transform.root.gameObject.GetComponent<CharacterStat>().damage);
    }
}
