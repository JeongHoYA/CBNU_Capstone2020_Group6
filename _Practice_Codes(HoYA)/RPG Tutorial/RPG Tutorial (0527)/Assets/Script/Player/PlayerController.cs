using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    Camera cam;                                                                                     // 카메라 컴포넌트
    PlayerMotor motor;                                                                              // PlayerMotor 클래스

    public Interactable focus;                                                                      // 플레이어가 선택한 인터랙티브 클래스 오브젝트

    public LayerMask movementMask;                                                                  // Ground를 체크하는 레이어마스크



    void Start()
    {
        cam = Camera.main;
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        // 마우스가 타 게임오브젝트를 클릭할 시 아무것도 하지 않음
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // 좌클릭 시 해당 좌표로 이동
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100, movementMask))
            {
                motor.MovetoPoint(hit.point);
                RemoveFocus();
            }
        }

        // 우클릭 시 해당 좌표의 물건과 상호작용
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if(interactable != null)
                {
                    SetFocus(interactable);
                }
            }
        }
    }

    void SetFocus(Interactable newFocus)
    {
        if(newFocus != focus)
        {
            if (focus != null)
                focus.OnDefocused();

            focus = newFocus;
            motor.FollowTarget(newFocus);
        }
        newFocus.OnFocused(transform);      
    }                                                         // 포커스할 타겟을 설정하는 함수
        
    void RemoveFocus()
    {
        if (focus != null)
            focus.OnDefocused();

        focus = null;
        motor.StopFollowingTarget();
    }                                                                           // 설정한 타겟을 없애는 함수
}
    