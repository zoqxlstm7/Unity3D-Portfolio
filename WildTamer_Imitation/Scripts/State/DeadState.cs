using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State<Animal>
{
    #region Variables
    readonly string DEAD_UI_KEY = "Prefabs/AnimalDeadUI";       // dead UI 키값
    readonly float DISPLAY_UI_DISTANCE = 2f;                    // dead UI가 표시되는 거리
    readonly float MAX_ELASED_IN_STATE = 15f;                   // 상태에 머무는 최대시간

    readonly int DeadHash = Animator.StringToHash("Dead");      // 사망 애니메이션

    AnimalDeadUIManager animalDeadUIManager = null;             // dead UI 관리 객체
    AnimalDeadUI deadUI = null;                                 // dead UI

    Player player = null;                                       // 플레이어
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
        if (animalDeadUIManager == null)
            animalDeadUIManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().deadUIManager;

        if (player == null)
            player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().player;

        // 애니메이션 재생
        owner.animator.SetTrigger(DeadHash);
        // 길찾기 에이전트 정지
        owner.agent.StopAgent();

        // 아군 동물이 죽은 경우 처리
        if (owner.gameObject.layer == LayerMask.NameToLayer("Player"))
            UnitIsAlly();
    }

    /// <summary>
    /// 상태 업데이트 처리 함수
    /// </summary>
    /// <param name="deltaTime">델타타임</param>
    public override void OnUpdate(float deltaTime)
    {
        // 적군 동물이 죽은 경우 처리
        if (owner.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            UnitIsEnemy();

            // 최대시간이 지날동안 아무것도 하지않았다면 제거
            if (stateMachine.ElapsedTimeInState >= MAX_ELASED_IN_STATE)
            {
                RemoveOnly();
            }
        }
    }

    /// <summary>
    /// 상태 탈출시 처리 함수
    /// </summary>
    public override void OnExit()
    {
        if (deadUI != null)
        {
            // 공격 범위내에 있지않다면 deadUI 제거
            animalDeadUIManager.Remove(DEAD_UI_KEY, deadUI);
            deadUI = null;
        }
    }
    #endregion State Methods

    #region Other Methods
    /// <summary>
    /// 오브젝트 제거 함수
    /// </summary>
    void RemoveOnly()
    {
        if (deadUI != null)
        {
            animalDeadUIManager.Remove(DEAD_UI_KEY, deadUI);
            deadUI = null;
        }

        InGameSceneManager inGameSceneManager = Object.FindObjectOfType<InGameSceneManager>();
        inGameSceneManager.animalManager.Remove(owner.FilePath, owner);
    }
    /// <summary>
    /// 죽은 유닛이 적군인 경우 처리 함수
    /// </summary>
    void UnitIsEnemy()
    {
        float distance = Vector3.Distance(owner.transform.position, player.transform.position);

        // 플레이어가 UI노출 범위에 있다면 UI 노출
        if (distance <= DISPLAY_UI_DISTANCE)
        {
            // deadUI가 없을때만 생성
            if (deadUI == null)
            {
                deadUI = animalDeadUIManager.Generate(DEAD_UI_KEY, Vector3.zero);
                deadUI.owner = owner;
            }

            // 위치 업데이트
            Vector3 pos = Camera.main.WorldToScreenPoint(owner.transform.position);
            pos.z = 0;

            deadUI.transform.position = pos;
        }
        else
        {
            if (deadUI != null)
            {
                // 공격 범위내에 있지않다면 deadUI 제거
                animalDeadUIManager.Remove(DEAD_UI_KEY, deadUI);
                deadUI = null;
            }
        }
    }

    /// <summary>
    /// 죽은 유닛이 아군인 경우 처리
    /// </summary>
    void UnitIsAlly()
    {
        InGameSceneManager inGameSceneManager = Object.FindObjectOfType<InGameSceneManager>();
        // 제거
        inGameSceneManager.player.tamingList.Remove(owner);
        inGameSceneManager.animalManager.Remove(owner.FilePath, owner);
    }
    #endregion Other Methods
}
