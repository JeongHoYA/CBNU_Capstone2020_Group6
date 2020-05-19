using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField mapNameField;                                                                             // 맵 이름 입력칸
    public GameObject notValidNamePanel;                                                                            // 무효한 이름 알림 패널

    public GameObject loadMapButton;                                                                                // 새 맵 버튼 프리팹
    public Transform loadMapPanel;                                                                                  // 새 맵 패널 위치
    public GameObject[] buttons = new GameObject[100];                                                              // 새 맵 버튼 오브젝트 리스트

    public BaseSettings settings;                                                                                   // 세팅 클래스

    public static string nameFromMainScene;                                                                         // 설정에서 선택한 맵의 이름 전역변수 
    public static string locationFromMainScene;                                                                     // 선택한 맵의 저장경로 전역변수

    string savedMapListPath;                                                                                        // 저장된 맵의 리스트가 저장된 파일 주소
    public List<string> savedMapList = new List<string>();                                                          // 저장된 맵의 리스트


    private void Start()
    {
        savedMapListPath = Application.dataPath + "/MapFolder/Maplist.txt";

        if (!File.Exists(savedMapListPath))
            File.WriteAllText(savedMapListPath, "");
        
        else
        {
            savedMapList.Clear();

            string[] lines = File.ReadAllLines(savedMapListPath);
            foreach (string l in lines)
                savedMapList.Add(l);     
        }
        
    }



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
            // 새 디렉토리를 만들어 거기에 맵네임+setting.cfg 이름을 가진 json형식의 설정 파일 생성

            using (StreamWriter mapListFile = new StreamWriter(savedMapListPath, true))
            {
                mapListFile.WriteLine(settings.mapName);
                savedMapList.Add(settings.mapName);
            }

            Invoke("StartBuildScene", 3f);
            // StartBuildScene 3초 후에 실행"
            
        } 
    }                                                                                    // Start Build 버튼을 눌렀을 때 시행

    public void LoadBuildPanel()
    {
        int cnt = 0;
        foreach (string l in savedMapList)
        {
            buttons[cnt] = Instantiate(loadMapButton, loadMapPanel.transform) as GameObject;
            buttons[cnt].name = l;
            cnt++;
        }
    }                                                                                 // Load Build 버튼을 눌렀을 때 시행

    public void LoadBuildMaptoBuildMode()
    {
        foreach (GameObject l in buttons)
        {
            Destroy(l);
        }
    }                                                                        // 저장된 맵 패널에서 뒤로가기를 눌렀을 때 시행

    public void LoadBuildMode()
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