using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<Animal>
{
    #region Variables
    readonly int AttackHash = Animator.StringToHash("Attack");  // 공격 애니메이션
    #endregion Variables

    #region State Methods
    /// <summary>
    /// 상태 초기화 함수
    /// </summary>
    public override void OnInitialize()
    {
    }

    /// <summary>
    /// 상태 진입시 처리 함수
    /// </summary>
    public override void OnEnter()
    {
        // 공격 행동을 할 수 있으면서 타겟이 살아있다면
        if (owner.CurrentAttackBehaviour.IsAvailalbe && !owner.CheckTargetIsDead)
        {
            // 공격 플래그 변경
            owner.IsAttack = true;

            owner.animator.SetTrigger(AttackHash);
        }
        else
        {
            stateMachine.ChangeState<IdleState>();
        }
    }

    /// <summary>
    /// 상태 업데이트 처리 함수
    /// </summary>
    /// <param name="deltaTime">델타타임</param>
    public override void OnUpdate(float deltaTime)
    {
    }

    /// <summary>
    /// 상태 탈출시 처리 함수
    /// </summary>
    public override void OnExit()
    {
    }
    #endregion State Methods
}
