using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabAuth : MonoBehaviour
{
    public MPManager mp;                                                                // 멀티플레이 매니저(Photon) 스크립트

    public InputField user;                                                             // 유저이름 인풋필드
    public InputField email;                                                            // 유저 이메일 인풋필드
    public InputField pass;                                                             // 유저 비밀번호 인풋필드
    public Text Message;                                                                // 메시지 텍스트
    
    public bool IsAuthenticated = false;                                                // 로그인 성공 여부

    LoginWithPlayFabRequest loginRequest;                                               // Playfab계정식 로그인요청



    void Start()
    {
        email.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void Login()
    {
        // 로그인 요청 인자 생성
        loginRequest = new LoginWithPlayFabRequest();
        loginRequest.Username = user.text;
        loginRequest.Password = pass.text;

        // 로그인 시도
        PlayFabClientAPI.LoginWithPlayFab(loginRequest,
        result => {
            // 계정 탐색 성공시
            IsAuthenticated = true;
            Message.text = "Welcome " + user.text;
            mp.userName = user.text;
            mp.ConnectToMaster();
            Debug.Log("You are now Logged in");
        },
        error => {
            //계정 탐색 실패시
            IsAuthenticated = false;
            if(error.Error.Equals("AccountNotFound"))
            {
                email.gameObject.SetActive(true);
                Message.text = "User not Found\nGo to Register Field";
            }
            else
                Message.text = "Failed to Login\n[" + error.ErrorMessage + "]";         
        }, null);
    }

    public void Register()
    {
        // 레지스터 요청 인자 생성
        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
        request.Email = email.text;
        request.Username = user.text;
        request.Password = pass.text;

        // 계정 생성 시도
        PlayFabClientAPI.RegisterPlayFabUser(request,
        result => {
            email.gameObject.SetActive(false);
            Message.text = "You're account has been created";
        },
        error => {
            Debug.Log(error.Error);
            if(email.gameObject.activeSelf == false)
            {
                email.gameObject.SetActive(true);
                Message.text = "Please enter your Email to register";
            }
            else
                Message.text = "Failed to create your account\n[" + error.ErrorMessage + "]";
        });
    }
}
