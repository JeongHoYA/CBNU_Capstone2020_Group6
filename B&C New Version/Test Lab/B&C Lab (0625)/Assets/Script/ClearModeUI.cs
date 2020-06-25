using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HoYA.Scoreboards;

public class ClearModeUI : MonoBehaviour
{
    public PlayerStat playerStat;

    public GameObject restartPanel;
    public GameObject clearPanel;

    public Slider healthBar;
    public Text timer;
    public Text coin;

    public Scoreboard scoreboard;
    public InputField input;

    float minute, second, milliSecond;

    bool clearPanelOn = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = playerStat.currentHealth;

        if (!playerStat.isDead && !playerStat.LevelClear)
        {
            minute = (int)(Time.timeSinceLevelLoad / 60f);
            second = (int)(Time.timeSinceLevelLoad % 60f);
            milliSecond = (int)(Time.timeSinceLevelLoad * 10f) % 10;

            timer.text = minute.ToString("00") + ":" + second.ToString("00") + ":" + milliSecond.ToString("0");
        }
        else
            timer.text = minute.ToString("00") + ":" + second.ToString("00") + ":" + milliSecond.ToString("0");

        coin.text = playerStat.coinCount.ToString();

        if (playerStat.isDead)
            restartPanel.SetActive(true);

        if (playerStat.LevelClear && !clearPanelOn)
        {
            clearPanel.SetActive(true);
            clearPanelOn = true;
        }
            
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GetUserName()
    {

        string userName = input.text;
        Debug.Log(userName);
        Debug.Log(timer.text);
        scoreboard.SavePath = Application.dataPath + "/highScore.json";
        scoreboard.AddEntry(new ScoreboardEntryData() { entryName = userName, entryScore = timer.text });
    }
}
