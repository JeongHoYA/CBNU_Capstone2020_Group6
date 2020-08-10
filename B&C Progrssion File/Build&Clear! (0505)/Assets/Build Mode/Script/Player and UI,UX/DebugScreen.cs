using TMPro;
using UnityEngine;


public class DebugScreen : MonoBehaviour
{

    World world;
    Player player;
    TextMeshProUGUI text;

    float frameRate;
    float timer;
    
    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        player = GameObject.Find("Player2").GetComponent<Player>();
        text = GetComponent<TextMeshProUGUI>();
    }

    
    void Update()
    {
        string debugText = "빌드 & 클리어! - 디버그 창\n";
        debugText += frameRate + " 프레임/초\n";
        debugText += "플레이어 월드 좌표 : " + Mathf.FloorToInt(player.transform.position.x) + " / " + Mathf.FloorToInt(player.transform.position.y) + " / " + Mathf.FloorToInt(player.transform.position.z) + "\n";
        debugText += "플레이어 청크 좌표 : " + world.playerChunkCoord.x + " / " + world.playerChunkCoord.z + "\n";

        if (player.isSprinting)
            debugText += "지금 달리고 있어요!\n";
        if (player.isFlying)
            debugText += "지금 날고 있어요!\n";
        if (player.isGhost)
            debugText += "고스트 모드에요!\n";
        if (world.inUI)
            debugText += "인벤토리 안을 보고 있어요!\n";

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
