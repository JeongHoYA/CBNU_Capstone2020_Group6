using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField mapNameField;                                                                             // 맵 이름 입력칸
    public GameObject notValidNamePanel;                                                                            // 무효한 이름 알림 패널

    public string[] mapNameList;

    public BaseSettings settings;                                                                                   // 세팅 클래스

    public static string nameFromMainScene;                                                                         // 설정에서 선택한 맵의 이름 전역변수 
    public static string locationFromMainScene;                                                                     // 선택한 맵의 저장경로 전역변수





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
        } // 맵네임 칸이 공백이거나 이미 존재하는 이름일 시 notValidNamePanel을 활성화

        else
        {
            Directory.CreateDirectory(Application.dataPath + "/MapFolder/" + settings.mapName + "/");
            locationFromMainScene = Application.dataPath + "/MapFolder/" + settings.mapName + "/";

            string jsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText(locationFromMainScene + settings.mapName + " settings.cfg", jsonExport);

            Invoke("StartBuildScene", 3f);
        } // 새 디렉토리를 만들어 거기에 맵네임+settin.cfg 이름을 가진 json형식의 설정 파일 생성 후 "StartBuildScene 3초 후에 생성"
    }                                                                                    // Start Build 버튼을 눌렀을 때 시행

    public void LoadBuildMap()
    {

    }

    private void StartBuildScene()
    {
        SceneManager.LoadScene("Build Scene");
    }                                                                               // 빌드 씬 로드 함수

    public void QuitGame()
    {
        Application.Quit();
    }                                                                                       // 게임 종료 함수
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