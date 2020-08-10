using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject buildPlayer;
    public GameObject clearPlayer;

    void Start()
    {
        buildPlayer.SetActive(false);
        clearPlayer.SetActive(true);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            buildPlayer.SetActive(!buildPlayer.activeSelf);
            clearPlayer.SetActive(!clearPlayer.activeSelf);
        }
    }
}
