using TMPro;
using UnityEngine;


public class DebugScreen : MonoBehaviour
{

    public PlayerControl player;
    public TextMeshProUGUI text;

    float frameRate;
    float timer;

    void Update()
    {
        Vector3Int playerpos = Vector3Int.FloorToInt(player.transform.position);

        string debugText = "빌드 & 클리어! - 디버그 창\n";
        debugText += frameRate + " 프레임/초\n";
        debugText += "플레이어 월드 좌표 : " + playerpos.x + " / " + playerpos.y + " / " + playerpos.z + "\n";
        debugText += "플레이어 청크 좌표 : " + playerpos.x / Chunk.chunkWidth + " / " + playerpos.z / Chunk.chunkWidth + "\n";

        if(player.isFlying)
            debugText += "지금 날고 있어요.\n";

        if (player.world.inUI)
            debugText += "인벤토리가 열려 있어요.\n";


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