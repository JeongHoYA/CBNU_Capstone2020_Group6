using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSummonEvent : MonoBehaviour
{
    public GameObject Monster;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 mobpos = transform.position + new Vector3(0.5f, 1.5f, 0.5f);
        Instantiate(Monster, mobpos, Quaternion.identity);//, transform);
    }
}
