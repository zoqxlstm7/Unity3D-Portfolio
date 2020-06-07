using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningPanel : BasePanel
{
    [SerializeField] Text countText;    // 카운트를 표시할 텍스트
    [SerializeField] int count;         // 카운트 숫자
        
    float lastCountTime;                // 마지막 시간 저장
    bool isCountStart = false;          // 카운트 시작 플래그

    /// <summary>
    /// 초기화 처리
    /// </summary>
    public override void InitializePanel()
    {
        if (!FindObjectOfType<GameManager>())
            return;

        base.InitializePanel();

        isCountStart = true;
        lastCountTime = Time.time;
    }

    /// <summary>
    /// 업데이트 처리
    /// </summary>
    public override void UpdatePanel()
    {
        base.UpdatePanel();

        UpdateCount();
    }

    /// <summary>
    /// 스폰 카운트 처리 함수
    /// </summary>
    public void UpdateCount()
    {
        if (!isCountStart)
            return;

        // 1초마다 카운트 변경
        if(Time.time - lastCountTime >= 1f)
        {
            count--;
            countText.text = count.ToString();
            lastCountTime = Time.time;
        }

        // 카운트가 0이면 스폰 시작
        if (count <= -1)
            SpawnStart();
    }

    /// <summary>
    /// 스폰 시작 함수
    /// </summary>
    void SpawnStart()
    {
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnStart();
        Close();
    }
}
