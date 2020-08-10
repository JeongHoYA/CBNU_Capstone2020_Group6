using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClearMenu : MonoBehaviour
{
    public PlayerStat playerStat;

    public GameObject restartPanel;
    public GameObject clearPanel;

    public Slider healthBar;
    public Text timer;
    public Text coin;

    float minute, second, milliSecond;



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

        if (playerStat.LevelClear)
            clearPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
