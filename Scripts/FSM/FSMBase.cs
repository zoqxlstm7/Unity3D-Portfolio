using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public partial class FSMBase : MonoBehaviour
{
    protected UnitState state;      //현재 상태를 나타낼 열거형 변수
    protected Animator animator;
    protected NavMeshAgent agent;

    protected bool isNewState;      //현재 상태가 변화했는지 체크
    float smoothTime = 0.1f;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void OnEnable()
    {
        state = UnitState.Run;
        StartCoroutine("FSMMain");
    }

    //현재 유닛 상태를 설정
    public void SetState(UnitState newState)
    {
        isNewState = true;
        state = newState;
    }

    //현재 유닛 상태를 반환
    public UnitState GetState()
    {
        return state;
    }

    IEnumerator FSMMain()
    {
        while (true)
        {
            isNewState = false;
            yield return StartCoroutine(state.ToString());
        }
    }

    protected virtual IEnumerator Idle()
    {
        do
        {
            yield return null;
            if (isNewState)
                break;

            animator.SetFloat("speedPercent", 0f, smoothTime, Time.deltaTime);

            //액션 구현
        } while (!isNewState);
    }

    protected virtual IEnumerator Run()
    {
        do
        {
            yield return null;
            if (isNewState)
                break;

            //액션 구현
            float speedPercent = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("speedPercent", speedPercent, smoothTime, Time.deltaTime);

        } while (!isNewState);
    }

    protected virtual IEnumerator Attack()
    {
        yield return null;
    }
}

public enum UnitState
{
    Idle,
    Run,
    Attack,
    Skill01,
    Skill02,
    Die
}
