using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public PlayerStat player;
    public Text timer;
    float minute, second, milliSecond;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!player.isDead)
        {
            minute = (int)(Time.timeSinceLevelLoad / 60f);
            second = (int)(Time.timeSinceLevelLoad % 60f);
            milliSecond = (int)(Time.timeSinceLevelLoad * 10f) % 10;

            timer.text = minute.ToString("00") + ":" + second.ToString("00") + ":" + milliSecond.ToString("0");
        }
        else
            timer.text = minute.ToString("00") + ":" + second.ToString("00") + ":" + milliSecond.ToString("0");
    }
}
