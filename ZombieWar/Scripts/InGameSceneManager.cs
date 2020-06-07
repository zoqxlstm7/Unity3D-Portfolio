using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSceneManager : BaseSceneManager
{
    const float GAME_OVER_DELAY_TIME = 2f;  // 게임오버 딜레이 시간

    // 플레이어 객체 반환
    [SerializeField] Player player;
    public Player Hero
    {
        get => player;
        set => player = value;
    }

    // 패널 관리 객체 반환
    //[SerializeField] PanelManager panelManager;
    //public PanelManager PanelManager
    //{
    //    get => panelManager;
    //}

    // 스폰 관리 객체 반환
    [SerializeField] SpawnManager spawnManager;
    public SpawnManager SpawnManager
    {
        get => spawnManager;
    }

    // 총알 관리 객체 반환
    [SerializeField] BulletManager bulletManager;
    public BulletManager BulletManager
    {
        get => bulletManager;
    }

    // 에너미 관리 객체 반환
    [SerializeField] EnemyManager enemyManager;
    public EnemyManager EnemyManager
    {
        get => enemyManager;
    }

    // 탄피 관리 객체 반환
    [SerializeField] EmptyShellManager emptyShellManager;
    public EmptyShellManager EmptyShellManager
    {
        get => emptyShellManager;
    }

    // 이펙트 관리 객체 반환
    [SerializeField] EffectManager effectManager;
    public EffectManager EffectManager
    {
        get => effectManager;
    }

    // 던지기 관리 객체 반환
    [SerializeField] ThrowManager throwManager;
    public ThrowManager ThrowManager
    {
        get => throwManager;
    }

    // 획득 UI 관리 객체 반환
    [SerializeField] AcquireUIManager acquireUIManager;
    public AcquireUIManager AcquireUIManager
    {
        get => acquireUIManager;
    }

    // 아이템 드랍 관리 객체 반환
    [SerializeField] ItemDropManager itemDropManager;
    public ItemDropManager ItemDropManager
    {
        get => itemDropManager;
    }

    // 채팅 관리 객체 반환
    [SerializeField] ChatManager chatManager;
    public ChatManager ChatManager
    {
        get => chatManager;
    }

    // 에너미 캐시 관리 객체 반환
    CacheManager enemyCacheManager = new CacheManager();
    public CacheManager EnemyCacheManager
    {
        get => enemyCacheManager;
    }

    // 탄알 캐시 관리 객체 반환
    CacheManager bulletCacheManager = new CacheManager();
    public CacheManager BulletCacheManager
    {
        get => bulletCacheManager;
    }

    // 탄피 캐시 관리 객체 반환
    CacheManager emptyShellCacheManager = new CacheManager();
    public CacheManager EmptyShellCacheManager
    {
        get => emptyShellCacheManager;
    }

    // 이펙트 캐시 관리 객체 반환
    CacheManager effectCacheManager = new CacheManager();
    public CacheManager EffectCacheManager
    {
        get => effectCacheManager;
    }

    public bool IsGameOver { get; set; } = false;     // 게임오버 플래그

    private void Start()
    {
        // 게임 매니저 객체가 없으면 리턴
        if (!FindObjectOfType<GameManager>())
            return;

        // 배틀 BGM 재생
        GameManager.Instance.SoundManager.StopBGM();
        GameManager.Instance.SoundManager.PlayBGM(AudioNameConstant.BATTLE_SOUND);
    }

    public override void UpdateManager()
    {
        base.UpdateManager();

        GameOverCheck();
    }

    /// <summary>
    /// 스폰 시작 함수
    /// </summary>
    public void SpawnStart()
    {
        // 스폰매니저에 스폰 명령 실행
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager.GameStart();
    }

    /// <summary>
    /// 게임이 종료되는 조건인지 검사
    /// </summary>
    void GameOverCheck()
    {
        // 게임 매니저가 없으면 리턴
        if (!FindObjectOfType<GameManager>())
            return;

        // 인게임씬 매니저
        InGameSceneManager inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();
        // 게임오버 패널
        GameOverPanel gameOverPanel = PanelManager.GetPanel(typeof(GameOverPanel)) as GameOverPanel;

        // 게임 오버가 된 경우 리턴
        if (inGameSceneManager.IsGameOver)
            return;

        // 보스 생성이 완료된 경우 실행
        if((PanelManager.GetPanel(typeof(BossPanel)) as BossPanel).IsGenerateBoss)
        {
            // 보스가 죽은 경우 게임 오버 처리
            if (inGameSceneManager.SpawnManager.BossEnemy.IsDead)
            {
                StartCoroutine(GameOverDelay(inGameSceneManager, gameOverPanel));

                return;
            }
        }

        // 등록된 모든 플레이어를 얻어옴
        List<Player> players = (PanelManager.GetPanel(typeof(PlayersInfoPanel)) as PlayersInfoPanel).Players;

        // 등록된 플레이어가 있는 경우 실행
        if (players.Count > 0)
        {
            // 살아있는 플레이어가 있는지 여부
            bool isAnyoneAlive = false;

            // 한명이라도 살아있는지 검사
            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].IsDead)
                {
                    isAnyoneAlive = true;
                    break;
                }
            }

            // 살아있는 플레이어가 없다면 게임오버 처리
            if (!isAnyoneAlive)
            {
                StartCoroutine(GameOverDelay(inGameSceneManager, gameOverPanel));

                return;
            }
        }
    }

    /// <summary>
    /// 게임 종료 패널을 보여주기까지 딜레이 시간을 줌
    /// </summary>
    /// <param name="inGameSceneManager"></param>
    /// <param name="gameOverPanel"></param>
    /// <returns></returns>
    IEnumerator GameOverDelay(InGameSceneManager inGameSceneManager, GameOverPanel gameOverPanel)
    {
        // 게임 오버 처리
        inGameSceneManager.IsGameOver = true;

        yield return new WaitForSeconds(GAME_OVER_DELAY_TIME);
        // 게임 오버 패널을 보여줌
        gameOverPanel.Show();
    }
}
