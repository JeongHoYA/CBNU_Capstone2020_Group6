using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    public bool IsFirebaseReady { get; private set; }                       // 파이어베이스 고정 가능 여부
    public bool IsSignInOnProgress { get; private set; }                    // 중복로그인 방지 변수

    public InputField emailField;                                           // 이메일 인풋필드 오브젝트
    public InputField passwordField;                                        // 비밀번호 인풋필드 오브젝트
    public Button signInButton;                                             // 확인 버튼 오브젝트

    public static FirebaseApp firebaseApp;                                  // 파이어베이스 전체 애플리케이션 오브젝트
    public static FirebaseAuth firebaseAuth;                                // 파이어베이스 인증 관리 오브젝트

    public static FirebaseUser User;                                        // 이메일과 비밀번호에 대응되는 유저 정보를 가져와 할당하는 곳

    public void Start() // 게임 시작시 실행하는 함수
    {
        signInButton.interactable = false;                                  // 확인 버튼 비활성화

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>     // 파이어베이스 구동 가능 여부 체크
            {
                var result = task.Result;                                   // 작업의 결과 할당

                if (result != DependencyStatus.Available)                   // 작업의 결과가 '사용가능'이 아닐 시
                {                                                           // 로그에러 출력후 고정가능여부 false선언
                    Debug.LogError(result.ToString());
                    IsFirebaseReady = false;
                }
                else                                                        // 작업의 결과가 '사용가능'일 시
                {                                                           // 고정가능여부 true 선언후 각 변수에 기본값 할당
                    IsFirebaseReady = true;
                    firebaseApp = FirebaseApp.DefaultInstance;
                    firebaseAuth = FirebaseAuth.DefaultInstance;            // = FirebaseAuth.GetAuth(FirebaseApp);
                }

                signInButton.interactable = IsFirebaseReady;                // 확인버튼 활성화여부를 구동 가능 여부와 동기화
            }
        );                         
    }

    public void SignIn() // 로그인 시도시 실행하는 함수
    {
        if (!IsFirebaseReady || IsSignInOnProgress || User != null)         // 파이어베이스 미고정/로그인 진행 중/유저 할당 이후에는      
            return;                                                         // SignIn 함수 즉시 종료

        IsSignInOnProgress = true;                                          // 로그인 진행 여부 true선언
        signInButton.interactable = false;                                  // 로그인 확인버튼 비활성화

        firebaseAuth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text).ContinueWithOnMainThread
        (task => {                                                                   
            Debug.Log($"Sign in status : {task.Status}");                   // 이메일/비밀번호 로그인 시도, 상황로그 출력

            IsSignInOnProgress = false;                                     // 로그인 진행 여부 false 선언
            signInButton.interactable = true;                               // 로그인 확인버튼 활성화

            if (task.IsFaulted)                                             // 작업 실패시 에러로그 출력
                Debug.LogError(task.Exception);
            else if (task.IsCanceled)                                       // 작업 취소시 문자 출력
                Debug.LogError("Sign-in canceled");
            else                                                            // 작업 성공시 User에 작업 결과 할당, 로비 씬 불러오기
            {
                User = task.Result;
                Debug.Log(User.Email);
                SceneManager.LoadScene("Lobby");
            }
        });
    }
}