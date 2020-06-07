using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneManager : BaseSceneManager
{
    const float START_TIME_VALUE = 3.0f;                            // 시작까지 걸리는 시간
    const float TEXT_UPDATE_INTERVAL = 0.15f;                       // 텍스트 업데이트 주기
    const string LOADING_TEXT_VALUE = "플레이어를 기다리는 중...";    // 출력할 로딩 텍스트

    [SerializeField] Text loadingText;                              // 참조할 로딩 텍스트 객체
    [SerializeField] Text playerCountText;                          // 참조할 접속한 플레이어 텍스트
    [SerializeField] Button gameStartButton;                        // 참조할 게임시작 버튼

    int textIndex;                                                  // 텍스트 인덱스
    float lastUpdateTime;                                           // 마지막 업데이트 시간

    bool isStart = false;                                           // 게임시작 버튼이 눌렸는지 여부

    public override void UpdateManager()
    {
        base.UpdateManager();

        // 마스터 클라이언트만 게임시작 버튼 노출
        gameStartButton.gameObject.SetActive(GameManager.Instance.NetworkManager.IsMasterClient);

        TextUpdate();
        PlayerCountUpdate();
    }

    /// <summary>
    /// 텍스트 업데이트 처리
    /// </summary>
    void TextUpdate()
    {
        if(Time.time - lastUpdateTime > TEXT_UPDATE_INTERVAL)
        {
            // 인덱스만큼 글자 출력
            loadingText.text = LOADING_TEXT_VALUE.Substring(0, textIndex + 1);

            textIndex++;
            if (textIndex >= LOADING_TEXT_VALUE.Length)
                textIndex = 0;

            lastUpdateTime = Time.time;
        }
    }

    /// <summary>
    /// 현재 룸 접속 플레이어 수 검사
    /// </summary>
    void PlayerCountUpdate()
    {
        playerCountText.text = "현재 접속\n"
            + GameManager.Instance.NetworkManager.PlayerCountInRoom + " / " + NetworkManager.MAX_PLAYER;
    }

    /// <summary>
    /// 인게임 씬으로 이동 버튼
    /// </summary>
    public void GotoNextScene()
    {
        if (!isStart)
        {
            GameManager.Instance.NetworkManager.OnGotoInGameScene();
            isStart = true;
        }
    }
}
