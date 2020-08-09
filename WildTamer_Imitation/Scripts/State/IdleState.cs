using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<Animal>
{
    #region Variables
    readonly float MIN_ELAPSED_IN_STATE = 0f;   // 상태에 머무르는 최소시간
    readonly float MAX_ELAPSED_IN_STATE = 5f;   // 상태에 머무르는 최대시간

    float ranInterval = 0.0f;                   // 랜덤 시간 간격

    Player player = null;                       // 플레이어
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
        if(player == null)
            player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().player;

        // 정지거리 초기화
        owner.agent.StopDistance = 0f;
        // 랜덤 시간 간격 계산
        ranInterval = Random.Range(MIN_ELAPSED_IN_STATE, MAX_ELAPSED_IN_STATE);
    }

    /// <summary>
    /// 상태 업데이트 처리 함수
    /// </summary>
    /// <param name="deltaTime">델타타임</param>
    public override void OnUpdate(float deltaTime)
    {
        // 사망했다면 리턴
        if (owner.isDead)
        {
            stateMachine.ChangeState<DeadState>();
            return;
        }

        // 유닛이 적군인 경우 기본적인 대기상태 패턴 실행
        if(owner.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 이동중이 아니라면 기본적인 대기상태 패턴 실행
            BaseIdlePattern();
        }
        else if(owner.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (player.InputDirection != Vector3.zero)
            {
                // 플레이어가 이동중이라면 따라가기상태로 전환
                stateMachine.ChangeState<FollowMoveState>();
            }
            else
            {
                // 이동중이 아니라면 기본적인 대기상태 패턴 실행
                BaseIdlePattern();
            }
        }
    }

    /// <summary>
    /// 상태 탈출시 처리 함수
    /// </summary>
    public override void OnExit()
    {
    }
    #endregion State Methods

    #region Other Methods
    /// <summary>
    /// 기본적인 대기상태 행동 패턴
    /// </summary>
    void BaseIdlePattern()
    {
        Transform target = owner.SearchEnemy();
        // 타겟이 있다면
        if (target != null)
        {
            if (owner.IsTargetInAttackRange)
            {
                // 공격행동이 없다면 리턴
                if (owner.CurrentAttackBehaviour == null)
                    return;

                // 플립 설정
                owner.SetFlip(target.position);
                // 타겟이 공격 사정거리에 있다면 공격상태로 전환
                stateMachine.ChangeState<AttackState>();
            }
            else
            {
                // 타겟이 공격 사정거리에 없다면 이동상태로 전환
                stateMachine.ChangeState<MoveState>();
            }
        }
        else
        {
            if(owner.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (stateMachine.ElapsedTimeInState >= ranInterval)
                {
                    // 타겟이 없다면 웨이포인트상태로 전환
                    stateMachine.ChangeState<MoveWaypointState>();
                }
            }
        }
    }
    #endregion Other Methods
}
