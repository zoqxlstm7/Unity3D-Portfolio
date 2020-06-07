using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPun
{
    public const int GENERATE_BOSS_KILL_COUNT = 300;   // 보스 생성까지 킬카운트

    const float MIN_GENERATE_SIZE = -50;        // 생성지점 최소 좌표
    const float MAX_GENERATE_SIZE = 50;         // 생성지점 최대 좌표

    const float SPAWN_TIME = 20f;               // 주기적 스폰 시간

    public int KillCount { get; set; } = 0;     // 죽은 숫자
    [SerializeField] int spawnCount = 15;      // 한번에 스폰될 양
    [SerializeField] Transform bossSpawnPoint;  // 보스 생성 지점

    Boss boss;
    public Boss BossEnemy
    {
        get => boss;
        set => boss = value;
    }

    float lastActionTime;                       // 마지막 행동 시간
    bool isStart;                               // 게임 시작되었는지 여부
    bool isGenerateBoss;                        // 보스 출현 여부

    /// <summary>
    /// 게임 스타트 함수
    /// </summary>
    public void GameStart()
    {
        lastActionTime = Time.time - SPAWN_TIME;
        isStart = true;

        // 방에 들어온 플레이어 수에 따라 한번에 스폰될 양 변경
        spawnCount *= GameManager.Instance.NetworkManager.PlayerCountInRoom;
        Debug.Log("GameStart");
    }

    private void Update()
    {
        // 게임매니저가 없는 경우 리턴
        if (!FindObjectOfType<GameManager>())
            return;

        // 마스터 클라이언트가 아닌경우 리턴
        if (!GameManager.Instance.NetworkManager.IsMasterClient)
            return;

        // 게임 오버시 리턴
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().IsGameOver)
            return;

        // 킬카운트를 채웠을 때 보스 출현
        if(KillCount >= GENERATE_BOSS_KILL_COUNT && !isGenerateBoss)
        {
            BossPanel bossPanel = PanelManager.GetPanel(typeof(BossPanel)) as BossPanel;

            // 보스 출현 진척도 업데이트 false;
            bossPanel.IsProgress = false;
            // 보스 출현 텍스트 함수 시작
            bossPanel.StartWarnningText();

            isGenerateBoss = true;
        }

        // 보스 생성 시 스폰 중단
        if (isGenerateBoss)
            return;

        // 최대 스폰 제한
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EnemyManager.GetCount() > spawnCount)
            return;

        // 스폰 시작이 아니라면 리턴
        if (!isStart)
            return;

        // 주기적 스폰
        if (Time.time - lastActionTime >= SPAWN_TIME)
        {
            Spawn();
            lastActionTime = Time.time;
        }
    }

    /// <summary>
    /// 에너미 생성 함수
    /// </summary>
    void Spawn()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EnemyManager.Generate(EnemyManager.ENEMY_FILE_PATH_0, GeneratePoint());
        }
    }

    /// <summary>
    /// 생성 지점 처리
    /// </summary>
    /// <returns>생성 지점 반환</returns>
    public Vector3 GeneratePoint()
    {
        float x = Random.Range(MIN_GENERATE_SIZE, MAX_GENERATE_SIZE);
        float z = Random.Range(MIN_GENERATE_SIZE, MAX_GENERATE_SIZE);

        Vector3 generatePoint = new Vector3(x, 0, z);

        return generatePoint;
    }

    /// <summary>
    /// 보스 생성함수
    /// </summary>
    public void GenerateBoss()
    {
        // 보스 생성
        PhotonNetwork.Instantiate("Prefabs/BossEnemy", bossSpawnPoint.position, new Quaternion(0, 180, 0, 0));
    }
}
