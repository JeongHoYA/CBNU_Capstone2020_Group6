using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    public GameObject mainMenuObject;
    public GameObject settingsObject;

    [Header("Main menu UI Elements")]
    public TextMeshProUGUI seedField;

    [Header("Settings menu UI Elements")]
    public Slider viewDstSlider;
    public TextMeshProUGUI viewDisText;
    public Slider mouseSSSlider;
    public TextMeshProUGUI mouseSSText;
    public Toggle threadingToggle;
    public Toggle chunkAnimToggle;
    public TMP_Dropdown clouds;

    Settings settings;

    private void Awake()
    {
        if (!File.Exists(Application.dataPath +"/settings.cfg"))
        {
            Debug.Log("No settings file found, creating new one.");

            settings = new Settings();
            string jsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);
        }
        else
        {
            Debug.Log("Settings file found, loading settings.");

            string jsonImport = File.ReadAllText(Application.dataPath + "/settings.cfg");
            settings = JsonUtility.FromJson<Settings>(jsonImport);
        }
    }

    public void StartGame()
    {
        VoxelData.seed = Mathf.Abs(seedField.text.GetHashCode()) / VoxelData.WorldSizeInChunks;
        SceneManager.LoadScene("World", LoadSceneMode.Single);
    }

    public void EnterSettings()
    {
        viewDstSlider.value = settings.ViewDistance;
        UpdateviewDstSlider();
        mouseSSSlider.value = settings.mouseSensitivity;
        UpdateMouseSlider();
        threadingToggle.isOn = settings.enableThreading;
        chunkAnimToggle.isOn = settings.enableAnimatedChunks;
        clouds.value = (int)settings.clouds;

        mainMenuObject.SetActive(false);
        settingsObject.SetActive(true);
    }

    public void LeaveSettins()
    {
        settings.ViewDistance = (int)viewDstSlider.value;
        settings.mouseSensitivity = mouseSSSlider.value;
        settings.enableThreading = threadingToggle.isOn;
        settings.enableAnimatedChunks = chunkAnimToggle.isOn;
        settings.clouds = (CloudStyle)clouds.value;

        string jsonExport = JsonUtility.ToJson(settings);
        File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);

        mainMenuObject.SetActive(true);
        settingsObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateviewDstSlider()
    {
        viewDisText.text = "View Distance : " + viewDstSlider.value;
    }

    public void UpdateMouseSlider()
    {
        mouseSSText.text = "Mouse Sensitivity : " + mouseSSSlider.value.ToString("F1");
    }
}
