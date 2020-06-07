using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    FollowCamera followCamera;      // follow 카메라 객체
    int watchPlayerIndex = 0;       // 관전 인덱스

    private void Start()
    {
        // follow 카메라 객체 캐싱
        followCamera = Camera.main.GetComponent<FollowCamera>();
    }
    /// <summary>
    /// 업데이트 처리
    /// </summary>
    private void Update()
    {
        // 게임매니저가 없을시 리턴
        if (!FindObjectOfType<GameManager>())
            return;

        InGameSceneManager inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();

        // 히어로 객체가 null일시 리턴
        if (inGameSceneManager.Hero == null)
            return;

        // 채팅을 입력중이라면 리턴
        if (inGameSceneManager.ChatManager.IsInputChat)
            return;

        // 게임 오버되도 채팅을 입력할 수 있도록
        UpdateChatKeyInput();

        // 게임오버시 리턴
        if (inGameSceneManager.IsGameOver)
            return;

        // 보스연출시 리턴
        BossPanel bossPanel = PanelManager.GetPanel(typeof(BossPanel)) as BossPanel;
        if (bossPanel.IsCinematic)
            return;

        UpdateInput();
    }

    /// <summary>
    /// 입력 업데이트 처리
    /// </summary>
    void UpdateInput()
    {
        // 플레이어 사망시 리턴 및 다른 플레이어 관전
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero.IsDead)
        {
            WatchingOtherPlayer();
            return;
        }

        UpdateMove();
        UpdateKeyInput();
        UpdateMouseInput();
    }

    /// <summary>
    /// 다른 플레이어 관전 처리 함수
    /// </summary>
    void WatchingOtherPlayer()
    {
        // 왼쪽 마우스 버튼이 눌린 경우
        if (Input.GetMouseButtonDown(0))
        {
            // 등록된 플레이어를 반환받음
            List<Player> players = (PanelManager.GetPanel(typeof(PlayersInfoPanel)) as PlayersInfoPanel).Players;

            // 등록된 플레이어가 없다면 리턴
            if (players.Count <= 0)
                return;

            // 등록된 플레이어 수를 초과했다면 인덱스 0으로 초기화
            if (watchPlayerIndex >= players.Count)
                watchPlayerIndex = 0;

            // 다른 플레이어로 카메라 타겟 변경
            followCamera.Target = players[watchPlayerIndex].transform;

            // 관전 인덱스 증가
            watchPlayerIndex++;
        }
    }

    /// <summary>
    /// 키보드 입력 처리
    /// </summary>
    void UpdateKeyInput()
    {
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero;

        // 수류탄 사용
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 수류탄 설정
            player.SetThrowItem(ThrowItemType.GRENADE);
        }

        // 힐팩 사용
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 힐팩 설정
            player.SetThrowItem(ThrowItemType.HEALPACK);
        }

        // 재장전 실행
        if (Input.GetKeyDown(KeyCode.R))
        {
            player.Reload();
        }
    }

    /// <summary>
    /// 채팅 키 입력 대기
    /// </summary>
    void UpdateChatKeyInput()
    {
        // 엔터키가 눌린 경우
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // 인풋필드에 포커스를 주거나 채팅 입력
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().ChatManager.ChatInputKey();
        }
    }

    /// <summary>
    /// 이동 처리 함수
    /// </summary>
    void UpdateMove()
    {
        float v, h;
        v = Input.GetAxisRaw("Vertical");
        h = Input.GetAxisRaw("Horizontal");

        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero.UpdateMove(v, h);
    }

    /// <summary>
    /// 마우스 입력 처리
    /// </summary>
    void UpdateMouseInput()
    {
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero;

        // 우클릭 시 총사용 상태로 전환
        if (Input.GetMouseButtonDown(1))
        {
            player.SetBackBaseWeaponState();
        }

        // 총 사용
        if (player.WeaponState == WeaponStyle.WEAPON)
            WeaponControl();
        // 근접 무기 사용
        else if (player.WeaponState == WeaponStyle.MELEE)
            MeleeControl();
        // 던지기 사용
        else if (player.WeaponState == WeaponStyle.THROW)
            ThrowControl();
    }

    /// <summary>
    /// 총 장착시 행동 처리
    /// </summary>
    void WeaponControl()
    {
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero;

        // 왼쪽 마우스 버튼이 눌린 지점에 대한 처리
        if (Input.GetMouseButton(0))
            player.Fire();
        else
            player.StopFire();
    }

    /// <summary>
    /// 던질 때의 행동 처리
    /// </summary>
    void ThrowControl()
    {
        // 왼쪽 마우스 버튼이 떼질 때 던짐 함수 실행
        if (Input.GetMouseButtonUp(0))
        {
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero.Throw();
        }
    }

    void MeleeControl()
    {

    }
}
