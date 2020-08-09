using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWaypointState : State<Animal>
{
    #region Variables
    readonly int MoveHash = Animator.StringToHash("Move");      // 이동 애니메이션

    Vector3 targetPos = Vector3.zero;                           // 타겟 지점
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
        // 정지거리 초기화
        owner.agent.StopDistance = 2f;

        // 웨이포인트 인덱스 계산
        owner.waypointIndex = (owner.waypointIndex + 1) % owner.waypoints.Length;
        owner.animator.SetBool(MoveHash, true);

        // 타겟지점으로 경로 설정
        targetPos = owner.waypoints[owner.waypointIndex].position;
        owner.agent.SetDestination(targetPos);
        // 플립설정
        owner.SetFlip(owner.waypoints[owner.waypointIndex].position);
    }

    /// <summary>
    /// 상태 업데이트 처리 함수
    /// </summary>
    /// <param name="deltaTime">델타타임</param>
    public override void OnUpdate(float deltaTime)
    {
        Transform target = owner.SearchEnemy();
        // 타겟이 있다면
        if (target != null)
        {
            // 타겟이 있다면 대기상태로 전환
            stateMachine.ChangeState<IdleState>();
        }
        else
        {
            // 정지거리내에 도착하며 대기상태로 전환
            if (Vector3.Distance(owner.transform.position, targetPos) <= owner.agent.StopDistance)
            {
                stateMachine.ChangeState<IdleState>();
            }
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
