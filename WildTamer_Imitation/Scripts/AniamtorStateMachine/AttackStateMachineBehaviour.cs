using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateMachineBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // IAttackable 인터페이스가 있다면 공격 로직 함수 호출
        animator.GetComponent<IAttackable>()?.OnExecuteAttack();
        // 애니메이션이 종료되었다면 대기 상태로 전환
        animator.GetComponent<Animal>()?.StateMachine.ChangeState<IdleState>();
    }
}
