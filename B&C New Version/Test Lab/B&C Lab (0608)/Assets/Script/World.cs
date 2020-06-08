using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject buildPlayer;
    public GameObject clearPlayer;

    public GameObject Monster;

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

        if (Input.GetKeyDown(KeyCode.M))
        {
            Instantiate(Monster, new Vector3(10, 10, 10), new Quaternion(0, 0, 0, 0));
        }
    }
}
