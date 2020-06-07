using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossPanel : BasePanel
{
    const string BOSS_NAME = "좀비왕";                 // 보스 이름
    const float OFFSET_Y = 4f;                          // Y축 오프셋

    [SerializeField] Image bossProgress;                // 진척도를 보여줄 이미지
    [SerializeField] Text progressText;                 // 진척도를 보여줄 텍스트
    [SerializeField] Text nameText;                     // 이름 표시 텍스트
    [SerializeField] Image cinematicImage;              // 보스 연출시 표시될 이미지
    [SerializeField] Text warnningText;                 // 보스 출현 텍스트

    [SerializeField] int cinematicTimeCount;            // 보스 연출 진행 시간 카운트
    [SerializeField] int warnningTimeCount;             // 보스 출현 텍스트 진행 시간 카운트
    FollowCamera followCamera;                          // follow 카메라 객체
    float actionStartTime;                              // 연출 시작시간

    float originTextAlphaValue;                         // 보스출현 텍스트 기존 알파값 저장

    public bool IsProgress { get; set; } = true;        // 진척도를 업데이트할지 여부
    public bool IsGenerateBoss { get; set; } = false;   // 보스 체력바 업데이트 여부
    public bool IsCinematic { get; set; } = false;      // 보스 연출 플래그
    public bool IsWarnning { get; set; } = false;       // 보스 출현 문구 플래그

    public override void InitializePanel()
    {
        base.InitializePanel();

        // follow 카메라 캐싱
        followCamera = FindObjectOfType<FollowCamera>();
        // 보스 출현 텍스트 숨김 처리
        warnningText.gameObject.SetActive(false);
        // 보스 연출 이미지 숨김 처리
        cinematicImage.gameObject.SetActive(false);

        //연출시 못움직이게 해야함
        // 연출 이미지 만들어야함
    }

    public override void UpdatePanel()
    {
        // 게임 매니저 객체가 없으면 리턴
        if (!FindObjectOfType<GameManager>())
            return;

        // 게임 오버면 리턴
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().IsGameOver)
            return;

        base.UpdatePanel();

        UpdateProgress();
        UpdateWarnningText();
        UpdateCienmatic();
        UpdateBossHpBar();
        UpdateNameTextMove();
    }

    /// <summary>
    /// 진척도 업데이트
    /// </summary>
    void UpdateProgress()
    {
        if (!IsProgress)
            return;

        SpawnManager spawnManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager;

        // 킬카운트가 보스생성 카운트보다 크다면 보스생성 카운트로 초기화
        if (spawnManager.KillCount > SpawnManager.GENERATE_BOSS_KILL_COUNT)
            spawnManager.KillCount = SpawnManager.GENERATE_BOSS_KILL_COUNT;

        // 프로그레스 진척도 표시
        bossProgress.fillAmount = (float)spawnManager.KillCount / (float)SpawnManager.GENERATE_BOSS_KILL_COUNT;
        // 텍스트 진척도 표시
        progressText.text = "보스 출현까지 " + (bossProgress.fillAmount * 100).ToString("N1") + "%";
    }

    /// <summary>
    /// 보스 출현 텍스트 시작 함수
    /// </summary>
    public void StartWarnningText()
    {
        IsWarnning = true;
        warnningText.gameObject.SetActive(true);
        // 기존 알파값 저장
        originTextAlphaValue = warnningText.color.a;

        actionStartTime = Time.time;
    }

    /// <summary>
    /// 보스 출현 텍스트 업데이트 처리
    /// </summary>
    void UpdateWarnningText()
    {
        if(!IsWarnning)
            return;

        Color newColor = warnningText.color;
        // PingPong: 타임값에 따라 0부터 length값까지 계속 반복
        newColor.a = Mathf.PingPong(Time.time * 0.8f, originTextAlphaValue);
        warnningText.color = newColor;

        // 연출 시간 검사
        if (Time.time - actionStartTime >= 1f)
        {
            warnningTimeCount--;
            actionStartTime = Time.time;
        }

        // 연출 시간이 종료되면 연출종료
        if (warnningTimeCount <= 0)
        {
            IsWarnning = false;
            warnningText.gameObject.SetActive(false);
            // 보스 생성 함수 호출
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager.GenerateBoss();
        }
    }

    /// <summary>
    /// 보스 연출 시작시 필요한 정보 초기화
    /// </summary>
    public void StartCinematic()
    {
        IsCinematic = true;
        cinematicImage.gameObject.SetActive(true);

        actionStartTime = Time.time;
    }

    /// <summary>
    /// 보스 연출 함수
    /// </summary>
    void UpdateCienmatic()
    {
        if (!IsCinematic)
            return;

        // 카메라 타겟 변경
        followCamera.Target = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager.BossEnemy.transform;

        // 연출 시간 검사
        if(Time.time - actionStartTime >= 1f)
        {
            cinematicTimeCount--;
            actionStartTime = Time.time;
        }

        // 연출 시간이 종료되면 연출종료
        if (cinematicTimeCount <= 0)
        {
            IsCinematic = false;
            cinematicImage.gameObject.SetActive(false);
            followCamera.Target = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero.transform;
        }
    }

    /// <summary>
    /// 보스 체력바 업데이트
    /// </summary>
    void UpdateBossHpBar()
    {
        if (!IsGenerateBoss)
            return;

        Boss boss = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager.BossEnemy;

        if (boss == null)
            return;

        // 보스 체력바 표시
        bossProgress.fillAmount = boss.CurrentHP / boss.MaxHP;
        // 보스 출현 텍스트
        progressText.text = BOSS_NAME + " 출현";
    }

    /// <summary>
    /// 네임 텍스트 이동 업데이트
    /// </summary>
    void UpdateNameTextMove()
    {
        if (!IsGenerateBoss)
            return;

        Vector3 bossPosition = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager.BossEnemy.transform.position;

        if (bossPosition == null)
            return;

        // screen point로 변환
        Vector3 pos = Camera.main.WorldToScreenPoint(new Vector3(bossPosition.x,
                                                                bossPosition.y + OFFSET_Y,
                                                                bossPosition.z));
        pos.z = 0;

        // 보스 이름 지정
        nameText.text = BOSS_NAME;
        // 좌표 업데이트
        nameText.transform.position = pos;
    }
}
