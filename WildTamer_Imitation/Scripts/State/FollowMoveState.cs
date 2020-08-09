using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMoveState : State<Animal>
{
    #region Variables
    readonly int MoveHash = Animator.StringToHash("Move");  // 이동 애니메이션

    Player player;                                          // 플레이어
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

        // 애니메이션 재생
        owner.animator.SetBool(MoveHash, true);
    }

    /// <summary>
    /// 상태 업데이트 처리 함수
    /// </summary>
    /// <param name="deltaTime">델타타임</param>
    public override void OnUpdate(float deltaTime)
    {
        if (player.InputDirection != Vector3.zero)
        {
            // 테이밍한 객체들이 겹치지 않도록 플레이어 주변 지점을 받아옴
            Vector3 targetPos = player.aroundPointFinder.FindMovePoint(owner);
            // 플레이어가 이동중이라면 플레이어 부근 지점으로 이동
            owner.agent.SetDestination(targetPos);

            // 플립설정
            owner.SetFlip(targetPos);
        }
        else
        {
            // 플레이어가 이동을 멈췄다면 대기상태로 전환
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
