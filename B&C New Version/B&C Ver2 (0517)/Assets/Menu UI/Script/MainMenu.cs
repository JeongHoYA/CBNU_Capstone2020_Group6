using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField mapNameField;
    public GameObject notValidNamePanel;

    public BaseSettings settings;

    public static string nameFromMainScene;
    public static string locationFromMainScene;

    public void NewBuildMap()
    {
        settings.mapName = mapNameField.text;
        nameFromMainScene = settings.mapName;

        if(settings.mapName == null)
        {
            notValidNamePanel.SetActive(true);
        }
        else if(Directory.Exists(Application.dataPath + "/MapFolder/" + settings.mapName + "/"))
        {
            notValidNamePanel.SetActive(true);
        }
        else
        {
            Directory.CreateDirectory(Application.dataPath + "/MapFolder/" + settings.mapName + "/");
            locationFromMainScene = Application.dataPath + "/MapFolder/" + settings.mapName + "/";

            string jsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText(locationFromMainScene + settings.mapName + " settings.cfg", jsonExport);

            Invoke("StartBuildScene", 3f);
        }
    }

    

    public void LoadBuildMap()
    {

    }

    private void StartBuildScene()
    {
        SceneManager.LoadScene("Build Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

[System.Serializable]
public class BaseSettings
{
    public string mapName = "";                                                             // 맵 이름

    [Range(2, 4)]
    public int viewDistance = 2;                                                            // 가시거리

    public bool isDay = true;                                                               // 낮/밤 여부

    [Range(0.1f, 2.0f)]
    public float mouseSensitivity = 1f;                                                     // 마우스 민감도
}