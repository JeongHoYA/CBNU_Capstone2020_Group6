using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{
    public World world;
    public GameObject pauseMenu;

    [Header("Setting Menu UI Elements")]
    public TextMeshProUGUI mapNameText;
    public Toggle daynightToggle;
    public Slider viewdistSlider;
    public TextMeshProUGUI viewdistText;
    public Slider mousesenceSlider;
    public TextMeshProUGUI mousesenceText;


    Settings settings;

    private void Awake()
    {
        
    }

    private void Start()
    {
        if (!File.Exists(world.mapFileLocation + world.mapName + " settings.cfg"))
        {
            Debug.Log("No settings file found ,Create new one");
            string jsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText(world.mapFileLocation + world.mapName + " settings.cfg", jsonExport);
        }
        else
        {
            Debug.Log("Settings files found, Load settings");
            string jsonImport = File.ReadAllText(world.mapFileLocation + world.mapName + " settings.cfg");
            settings = JsonUtility.FromJson<Settings>(jsonImport);
        }
    }

    public void FromPausetoSetting()
    {
        mapNameText.text = "맵 이름 : " + settings.mapName;

        daynightToggle.isOn = settings.isDay;

        viewdistSlider.value = settings.viewDistance;
        UpdateViewDistSlider();

        mousesenceSlider.value = settings.mouseSensitivity;
        UpdateMouseSlider();
    }

    public void FromSettingstoPause()
    { 
        settings.viewDistance = (int)viewdistSlider.value;
        settings.mouseSensitivity = mousesenceSlider.value;

        settings.isDay = daynightToggle.isOn;

        world.isSettingsChanged = true;

        string jsonExport = JsonUtility.ToJson(settings);
        Debug.Log("Setting Change to : " + jsonExport);
        File.WriteAllText(world.mapFileLocation + world.mapName + " settings.cfg", jsonExport);
    }

    public void UpdateViewDistSlider()
    {
        viewdistText.text = "시야거리 : " + viewdistSlider.value;
    }

    public void UpdateMouseSlider()
    {
        mousesenceText.text = "마우스 감도 : " + mousesenceSlider.value.ToString("F1");
    }

    public void FromBuildQuitMenuToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void FromPauseToGame()
    {
        pauseMenu.SetActive(false);
        world.inPause = false;
    }

    public void Savebutton()
    {
        world.SaveWorld();
    }
}
