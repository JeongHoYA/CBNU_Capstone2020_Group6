using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks                   // 포톤 Pun의 콜백을 인식할 수 있는 모노비헤이비어
{
    private readonly string gameVersion = "1";                          // 게임 버전 스트링

    public Text connectionInfoText;                                     // 연결정보 텍스트 오브젝트
    public Button joinButton;                                           // 조인 버튼 오브젝트
    
    private void Start()
    {

    }
    
    public override void OnConnectedToMaster()
    {
        
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
    
    }
    
    public void Connect()
    {
        
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
    
    }
    
    public override void OnJoinedRoom()
    {
    
    }
}