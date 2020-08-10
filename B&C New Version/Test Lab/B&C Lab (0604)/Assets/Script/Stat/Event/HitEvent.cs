using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStat))]
public class HitEvent : MonoBehaviour
{
    CharacterStat stat;
    CharacterStat otherStat;

    private void Start()
    {
        stat = GetComponent<CharacterStat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(this.gameObject != other.transform.root.gameObject)
        {
            if(other.gameObject.layer == 8)
            {
                otherStat = other.transform.gameObject.GetComponent<CharacterStat>();
                if (otherStat != null)
                {
                    Debug.Log(other.transform.name + " attack " + this.name);
                    stat.TakeDamage(otherStat.damage);
                }
            }
            else
            {
                otherStat = other.transform.root.gameObject.GetComponent<CharacterStat>();
                if (otherStat != null)
                {
                    Debug.Log(other.transform.root.name + " attack " + this.name);
                    stat.TakeDamage(otherStat.damage);
                }
            }
        }   
    }
}
