using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{
    public World world;                                                                                                     // 월드 오브젝트
    public GameObject pauseMenu;                                                                                            // 일시정지 메뉴 오브젝트

    public GameObject quitPanel;
    public GameObject quitwarningPanel;

    [Header("Setting Menu UI Elements")]
    public TextMeshProUGUI mapNameText;                                                                                     // 맵 이름 텍스트
    public Toggle daynightToggle;                                                                                           // 낮/밤 토글
    public Slider viewdistSlider;                                                                                           // 시야거리 슬라이더
    public TextMeshProUGUI viewdistText;                                                                                    // 시야거리 텍스트
    public Slider mousesenceSlider;                                                                                         // 마우스감도 슬라이더
    public TextMeshProUGUI mousesenceText;                                                                                  // 마우스감도 텍스트

    public TMP_Dropdown bgm;


    Settings settings;                                                                                                      // 세팅 클래스

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
    }                                                                                     // 일시정지 창 -> 설정 창

    public void FromSettingstoPause()
    { 
        settings.viewDistance = (int)viewdistSlider.value;
        settings.mouseSensitivity = mousesenceSlider.value;

        settings.isDay = daynightToggle.isOn;
        settings.BGMInspector = bgm.value;

        string jsonExport = JsonUtility.ToJson(settings);
        Debug.Log("Setting Change to : " + jsonExport);
        File.WriteAllText(world.mapFileLocation + world.mapName + " settings.cfg", jsonExport);

        world.isSettingsChanged = true;
    }                                                                                    // 설정 창 -> 일시정지창

    public void UpdateViewDistSlider()
    {
        viewdistText.text = "시야거리 : " + viewdistSlider.value;
    }                                                                                   // 시야거리 텍스트 업데이트

    public void UpdateMouseSlider()
    {
        mousesenceText.text = "마우스 감도 : " + mousesenceSlider.value.ToString("F1");
    }                                                                                      // 마우스 감도 텍스트 업데이트

    public void FromBuildQuitMenuToMainMenu()
    {
        if(!(World.Instance.hasSpawnBox && World.Instance.hasClearBox))
        {
            quitPanel.SetActive(false);
            quitwarningPanel.SetActive(true);
        }
        else
            SceneManager.LoadScene("Menu");
    }                                                                            // 빌드 모드 종료창 -> 메인 메뉴

    public void FromPauseToGame()
    {
        pauseMenu.SetActive(false);
        world.inPause = false;
    }                                                                                        // 일시정지 창 -> 게임

    public void Savebutton()
    {
        world.SaveWorld();
    }                                                                                             // 저장 버튼
}
