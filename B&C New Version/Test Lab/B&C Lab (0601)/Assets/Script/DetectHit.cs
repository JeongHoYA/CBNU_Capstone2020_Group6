using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(this.name +  " Hit");
    }
}
