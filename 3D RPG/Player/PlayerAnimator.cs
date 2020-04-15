using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator
{
    public State state;                     // 플레이어의 상태를 알려줄 열거형 변수

    // 상태 설정 함수
    public void SetState(State newState)
    {
        state = newState;
    }

    private void Update()
    {
        // 이동 애니메이션 호출
        if (state == State.IDLE)
            MoveAnimation();
    }

    // 이동 애니메이션 처리
    public override void MoveAnimation()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        animator.SetBool("run", v != 0 || h != 0);
    }

    // 공격 애니메이션 처리
    public override void AttackAnimation()
    {
        animator.SetTrigger("attack01");
    }

    // 매개변수에 따라 트리거 파라미터 형식의 애니메이션 처리
    public override void TriggerAnimation(string animationName)
    {
        animator.SetTrigger(animationName);
    }

    // 매개변수에 따라 bool 파라미터 형식의 애니메이션 처리
    public override void BoolAnimation(string animationName, bool isSet)
    {
        animator.SetBool(animationName, isSet);
    }
}
