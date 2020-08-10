using TMPro;
using UnityEngine;


public class DebugScreen : MonoBehaviour
{

    World world;
    TextMeshProUGUI text;

    float frameRate;
    float timer;
    
    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        text = GetComponent<TextMeshProUGUI>();
    }

    
    void Update()
    {
        string debugText = "빌드 & 클리어!";
        debugText += "\n";
        debugText += frameRate + " 프레임/초";
        debugText += "\n";
        debugText += "플레이어 월드 좌표 : " + Mathf.FloorToInt(world.player.transform.position.x) + " / " + Mathf.FloorToInt(world.player.transform.position.y) + " / " + Mathf.FloorToInt(world.player.transform.position.z);
        debugText += "\n";
        debugText += "플레이어 청크 좌표 : " + world.playerChunkCoord.x + " / " + world.playerChunkCoord.z;

        text.text = debugText.ToString();

        if (timer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
            timer += Time.deltaTime;
    }
}
