using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour
{
    NavMeshAgent agent;                                                                                             // 네비메시 에이전트 컴포넌트
    Animator animator;                                                                                              // 에니메이터 컴포넌트

    const float locomationAnimationSommthTime = 0.1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("SpeedPercent", speedPercent, locomationAnimationSommthTime, Time.deltaTime);
    }
}
