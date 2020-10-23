using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Linq;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    /* UI 오브젝트들 모음 */
    public TMP_InputField mapNameField;                                                                             // 맵 이름 입력칸
    public GameObject notValidNamePanel;                                                                            // 무효한 이름 알림 패널

    public GameObject buildPanel;                                                                                   // 빌드 캔버스
    public GameObject clearPanel;                                                                                   // 클리어 캔버스

    public GameObject loadMapButton;                                                                                // 맵 버튼 프리팹
    public Transform buildLoadMapPanel;                                                                             // 빌드 맵 패널 위치
    public Transform clearLoadMapPanel;                                                                             // 클리어 맵 패널 위치

    public GameObject scenetrans;                                                                                   // 씬 전환 패널

    public Scoreboard scoreboard;                                                                                   // 점수판

    public BaseSettings baseSettings;                                                                               // 세팅 클래스

    public GameObject[] buttons = new GameObject[100];                                                              // 맵 버튼 오브젝트 리스트


    /* 전역 변수 및 저장할 데이터 모음 */
    public static string nameFromMainScene;                                                                         // 설정에서 선택한 맵의 이름 전역변수 
    public static string locationFromMainScene;                                                                     // 선택한 맵의 저장경로 전역변수
    public static bool isOpendinBuildMode;                                                                          // 빌드 모드로 열렸는지 여부

    string savedMapListPath;                                                                                        // 저장된 맵의 리스트가 저장된 파일 주소
    public List<string> savedMapList = new List<string>();                                                          // 저장된 맵의 리스트

    /* 메뉴의 싱글톤화 관련 변수들 */
    private static MainMenu _instance;
    public static MainMenu Instance { get { return _instance; } }




    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
        // 현재 생성한 메뉴를 유일한 메뉴(싱글톤)으로 만들어줌
    }


    private void Start()
    {
        StartCoroutine(EndScene());
        nameFromMainScene = null;
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
        baseSettings.mapName = mapNameField.text;
        nameFromMainScene = baseSettings.mapName;

        
        if(baseSettings.mapName == null)
        {
            notValidNamePanel.SetActive(true);
        } 
        else if(Directory.Exists(Application.dataPath + "/MapFolder/" + baseSettings.mapName + "/"))
        {
            notValidNamePanel.SetActive(true);
        } // 맵네임 칸이 공백이거나 이미 존재하는 이름일 시 notValidNamePanel을 활성화

        else
        {
            Directory.CreateDirectory(Application.dataPath + "/MapFolder/" + baseSettings.mapName + "/");
            locationFromMainScene = Application.dataPath + "/MapFolder/" + baseSettings.mapName + "/";

            string jsonExport = JsonUtility.ToJson(baseSettings);
            File.WriteAllText(locationFromMainScene + baseSettings.mapName + " settings.cfg", jsonExport);
            // 새 디렉토리를 만들어 거기에 맵네임+setting.cfg 이름을 가진 json형식의 설정 파일 생성

            using (StreamWriter mapListFile = new StreamWriter(savedMapListPath, true))
            {
                mapListFile.WriteLine(baseSettings.mapName);
                savedMapList.Add(baseSettings.mapName);
            }
            isOpendinBuildMode = true;

            StartCoroutine(StartScene());
        }
    }                                                                                    // BuildMenuPanel - New Map버튼을 눌렀을 때 시행

    public void LoadBuildPanel()
    {
        int cnt = 0;
        foreach (string l in savedMapList)
        {
            buttons[cnt] = Instantiate(loadMapButton, buildLoadMapPanel.transform) as GameObject;
            buttons[cnt].name = l;
            cnt++;
        }
    }                                                                                 // BuildMenuPanel - Load Map버튼을 눌렀을 때 시행

    public void LoadBuildPaneltoBuildMode()
    {
        foreach (GameObject l in buttons)
        {
            Destroy(l);
        }
    }                                                                      // 빌드 모드 저장된 맵 패널에서 뒤로가기를 눌렀을 때 시행

    public void LoadBuildMap()
    {
        if (nameFromMainScene != null)
        {
            baseSettings.mapName = nameFromMainScene;
            locationFromMainScene = Application.dataPath + "/MapFolder/" + baseSettings.mapName + "/";
            isOpendinBuildMode = true;

            StartCoroutine(StartScene());
        }
        else
            Debug.Log("Select Map First");
    }                                                                                   // 빌드 모드에서 저장된 맵 로드 함수

    public void DeleteBuildMap()
    {
        if (nameFromMainScene != null)
        {
            locationFromMainScene = Application.dataPath + "/MapFolder/" + nameFromMainScene + "/";

            // 파일 삭제
            DirectoryInfo di = new DirectoryInfo(locationFromMainScene);
            if (di.Exists == true)
            {
                Debug.Log("Data found, start deleating");
                if (File.Exists(Application.dataPath + "/MapFolder/" + nameFromMainScene + ".meta"))
                    File.Delete(Application.dataPath + "/MapFolder/" + nameFromMainScene + ".meta");
                di.Delete(true);
                locationFromMainScene = "";
            }

            // 텍스트 파일 내용 삭제
            File.WriteAllText(savedMapListPath, String.Empty);

            // 저장된 맵 리스트에서 해당 맵 이름 삭제
            int cnt = 0;
            foreach (string l in savedMapList)
            {
                if (l == nameFromMainScene)
                {
                    Debug.Log("delete " + savedMapList[cnt]);
                    savedMapList.RemoveAt(cnt);
                    break;
                }
                cnt++;
            }

            // 텍스트 파일 내용 쓰기
            foreach (string l in savedMapList)
            {
                using (StreamWriter mapListFile = new StreamWriter(savedMapListPath, true))
                {
                    mapListFile.WriteLine(l);
                }
            }

            // 버튼 오브젝트 갱신
            foreach (GameObject l in buttons)
                Destroy(l);

            cnt = 0;
            foreach (string l in savedMapList)
            {
                buttons[cnt] = Instantiate(loadMapButton, buildLoadMapPanel.transform) as GameObject;
                buttons[cnt].name = l;
                cnt++;
            }

            nameFromMainScene = null;
        }
        else
            Debug.Log("Select Map First");
    }                                                                                 // 빌드 모드에서 맵을 삭제하는 함수



    public void RandomClearMap()
    {
        int ran = UnityEngine.Random.Range(0, savedMapList.Count());

        nameFromMainScene = savedMapList[ran];
        locationFromMainScene = Application.dataPath + "/MapFolder/" + nameFromMainScene + "/";
        isOpendinBuildMode = false;

        StartCoroutine(StartScene());
    }                                                                                 // 무작위 맵 선택 후 시작

    public void LoadClearPanel()
    {
        int cnt = 0;
        foreach (string l in savedMapList)
        {
            buttons[cnt] = Instantiate(loadMapButton, clearLoadMapPanel.transform) as GameObject;
            buttons[cnt].name = l;
            cnt++;
        }
    }                                                                                 // ClearManePanel - Load Map버튼을 눌렀을 때 시행

    public void LoadClearPaneltoClearMode()
    {
        foreach (GameObject l in buttons)
        {
            Destroy(l);
        }
    }                                                                      // 클리어 모드 저장된 맵 패널에서 뒤로가기를 눌렀을 때 시행
    
    public void LoadClearMap()
    {
        if (nameFromMainScene != null)
        {
            baseSettings.mapName = nameFromMainScene;
            locationFromMainScene = Application.dataPath + "/MapFolder/" + baseSettings.mapName + "/";
            isOpendinBuildMode = false;

            StartCoroutine(StartScene());
        }
        else
            Debug.Log("Select Map First");
    }                                                                                   // 클리어 모드에서 저장된 맵 로드 함수

    public void LoadClearRank()
    {
        if(nameFromMainScene != null)
        {
            scoreboard.SavePath = Application.dataPath + "/MapFolder/" + nameFromMainScene + "/highscore.json";
            scoreboard.ShowEntry();
        }
        else
            Debug.Log("Select Map First");
    }

    IEnumerator StartScene()
    {
        buildPanel.SetActive(false);
        clearPanel.SetActive(false);
        scenetrans.SetActive(true);
        scenetrans.GetComponent<Animator>().SetTrigger("end");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Game");
    }                                                                                       // 게임 씬 로드 함수

    IEnumerator EndScene()
    {
        yield return new WaitForSeconds(1.2f);
        scenetrans.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }                                                                                       // 게임 종료 함수


}



/* 세팅 클래스 */
[System.Serializable]
public class BaseSettings
{
    public string mapName = "";                                                             // 맵 이름
    public bool isCanBeCleared = false;                                                     // 클리어모드에 올릴 수 있나 여부
    public int cpX = 5, cpY = 5, cpZ = 5;                                                   // 클리어모드 플레이어의 스폰좌표

    /* 빌드 모드의 세팅 */
    [Range(2, 4)]
    public int viewDistance = 2;                                                            // 가시거리

    public bool isDay = true;                                                               // 낮/밤 여부
    public int BGMInspector = 0;                                                            // 브금 인스펙터

    [Range(0.1f, 2.0f)]
    public float mouseSensitivity = 1f;                                                     // 마우스 민감도
}

