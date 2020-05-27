using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;                                                                                   // 물건 상호작용 반경
    public Transform interactionTransform;                                                                      // 상호작용이 일어나는 위치 트랜스폼

    Transform player;                                                                                           // 플레이어 트랜스폼

    bool isFocus = false;                                                                                       // 포커스되고있나 여부
    bool hasInteracted = false;                                                                                 // 상호작용을 했나 여부


    private void OnDrawGizmosSelected()
    {
        if (interactionTransform == null)
            interactionTransform = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }                                                                      // 씬에서 선택될 시 상호작용 반경 표현


    private void Update()
    {
        if(isFocus && !hasInteracted)
        {
            float distance = Vector3.Distance(player.position, interactionTransform.position);
            if(distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }


    public virtual void Interact()
    {
        // 자식 클래스에서 오버라이드할 수 있는 함수
        Debug.Log("Interaction with " + transform.name);
    }                                                                           // 상호작용 관련 버추얼 함수

    public void OnFocused (Transform playerTransform)
    {
        isFocus = true;
        player = playerTransform;
        hasInteracted = false;
    }                                                        // 포커스될 시 호출

    public void OnDefocused()
    {
        isFocus = false;
        player = null;
        hasInteracted = false;
    }                                                                                // 포커스를 해제할 시 호출

    
}
