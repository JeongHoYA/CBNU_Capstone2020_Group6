using UnityEngine;
using UnityEngine.UI;

public class PlayerNameText : MonoBehaviour
{
    private Text nameText;                                                  // 텍스트 오브젝트

    private void Start()
    {
        nameText = GetComponent<Text>();                                    // 오브젝트에 컴포넌트 할당

        if (AuthManager.User != null)                                       // User가 Null이 아니면 유저의 이메일 스트링 표시
            nameText.text = $"Hi! " + AuthManager.User.Email.ToString();
        else                                                                // User가 Null이면 에러 문자 표시
            nameText.text = "ERROR : AuthManager.User == NULL";
    }
}
