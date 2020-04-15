using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMBase : MonoBehaviour
{
    public FsmState fsmState;                   // 현재 상태를 나타내는 열거형 변수
    protected bool isNewState;                     // 현재 상태가 변화했는지 여부

    //protected virtual void Start()
    //{
    //    // fsm 메인 함수 시작
    //    StartCoroutine(FsmMain());
    //}

    // 상태 enum값 변경 함수
    public void SetState(FsmState newState)
    {
        isNewState = true;
        fsmState = newState;
    }

    // FSM 메인 함수, 상태변수에 따른 코루틴함수 실행
    public abstract IEnumerator FsmMain();

    public abstract IEnumerator IDLE();     // 아이들 상태
    public abstract IEnumerator ATTACK();   // 공격 상태
    public abstract IEnumerator DIE();      // 사망 상태

    // FSM 메인 함수, 상태변수에 따른 코루틴함수 실행
    //IEnumerator FsmMain()
    //{
    //    while (true)
    //    {
    //        isNewState = false;
    //        yield return StartCoroutine(fsmState.ToString());
    //    }
    //}

    //public abstract IEnumerator IDLE();     // 아이들 상태
    //public abstract IEnumerator PATROLL();  // 순찰 상태
    //public abstract IEnumerator CHASING();  // 추적 상태
    //public abstract IEnumerator RETURN();   // 추적을 포기하고 돌아오는 상태
    //public abstract IEnumerator ATTACK();   // 공격 상태
    //public abstract IEnumerator TAKE_HIT(); // 데미지를 입을 때의 상태
    //public abstract IEnumerator DIE();      // 사망 상태
}

// fsm 상태 처리 열거형 변수
public enum FsmState
{
    IDLE,
    PATROLL,
    CHASING,
    RETURN,
    PREPARATION,
    ATTACK,
    TAKE_HIT,
    DIE
}
