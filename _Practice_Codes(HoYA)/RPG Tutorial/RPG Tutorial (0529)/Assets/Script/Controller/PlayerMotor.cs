using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{
    NavMeshAgent agent;                                                                         // 네비메시 에이전트 클래스
    Transform target;                                                                           // 추적할 타겟 트랜스폼

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        if(target != null)
        {
            agent.SetDestination(target.position);
            FaceTarget();
        }
    }

    public void MovetoPoint(Vector3 point)
    {
        agent.SetDestination(point);
    }                                                   // point위치로 최단 거리 이동명령 수행

    public void FollowTarget(Interactable newTarget)
    {
        agent.stoppingDistance = newTarget.radius * 0.8f;
        agent.updateRotation = false;

        target = newTarget.interactionTransform;
    }                                         // 타겟을 추적하는 함수, (타겟의 반경 * 0.8)거리에서 플레이어 정지

    public void StopFollowingTarget()
    {
        agent.stoppingDistance = 0;
        agent.updateRotation = true;

        target = null;
    }                                                        // 타켓의 추적을 멈추는 함수

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }                                                                        // 플레이어가 타겟을 항상 바라보게끔 하는 함수
}
