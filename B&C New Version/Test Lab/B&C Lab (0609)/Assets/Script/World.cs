using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject buildPlayer;
    public GameObject clearPlayer;

    public GameObject Monster;
    public Light directionalLight;

    public Material daySky;
    public Material nightSky;

    public bool isDay;

    void Start()
    {
        isDay = true;
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

        if (Input.GetKeyDown(KeyCode.N))
        {
            isDay = !isDay;
            if (isDay == true)
            {
                RenderSettings.skybox = daySky;
                RenderSettings.fogDensity = 0f; 
                directionalLight.transform.rotation = Quaternion.Euler(110, 0, 0);
            }
            else
            {
                RenderSettings.skybox = nightSky;
                RenderSettings.fogDensity = 0.1f;
                directionalLight.transform.rotation = Quaternion.Euler(50, 0, 0);
            }
        }
    }
}
