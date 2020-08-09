using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State<Animal>
{
    #region Variables
    readonly int MoveHash = Animator.StringToHash("Move");      // 이동 애니메이션
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
        owner.animator.SetBool(MoveHash, true);
    }

    /// <summary>
    /// 상태 업데이트 처리 함수
    /// </summary>
    /// <param name="deltaTime">델타타임</param>
    public override void OnUpdate(float deltaTime)
    {
        Transform target = owner.SearchEnemy();
        // 타겟이 있다면
        if(target != null)
        {
            // 타겟이 겹치지 않도록 타겟지점 주변 지점을 받아옴
            Vector3 targetPos = target.GetComponent<AroundPointFinder>().FindMovePoint();
            // 타겟지점으로 경로 설정
            owner.agent.SetDestination(targetPos);

            // 플립설정
            owner.SetFlip(target.position);

            // 공격 가능 거리에 있다면 대기상태로 전환
            if (owner.IsTargetInAttackRange)
            {
                stateMachine.ChangeState<IdleState>();
            }
        }
        else
        {
            // 타겟이 없다면 대기상태로 전환
            stateMachine.ChangeState<IdleState>();
        }
    }

    /// <summary>
    /// 상태 탈출시 처리 함수
    /// </summary>
    public override void OnExit()
    {
        owner.animator.SetBool(MoveHash, false);
        // 경로 초기화
        owner.agent.ResetPath();
    }
    #endregion State Methods
}
