using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임오버시 데미지 정보를 담을 클래스
/// </summary>
[System.Serializable]
public class DamageInfo
{
    public GameObject parent;       // 데미지 정보 UI 부모 객체
    public Text nameText;           // 플레이어 닉네임
    public Image damageProgress;    // 데미지 프로그레스 이미지
    public Text damageText;         // 데미지 텍스트
}

public class GameOverPanel : BasePanel
{
    const float LERP_GAGE_RATE = 1.0f;          // Lerp 게이지 표현시 사용될 시간 비율

    [SerializeField] Text[] gameOverTexts;      // 게임오버시 출력될 텍스트
    [SerializeField] Text potgNameText;         // 최고의 플레이어가 출력될 텍스트
    [SerializeField] DamageInfo[] damageInfos;  // 게임오버시 데미지정보를 나타낼 클래스 배열

    public override void InitializePanel()
    {
        base.InitializePanel();

        // 데미지 정보 UI 숨김 처리
        for (int i = 0; i < damageInfos.Length; i++)
        {
            damageInfos[i].damageProgress.fillAmount = 0f;
            damageInfos[i].parent.SetActive(false);
        }

        Close();
    }

    /// <summary>
    /// 업데이트 처리
    /// </summary>
    public override void UpdatePanel()
    {
        base.UpdatePanel();

        UpdateDamageInfo();
    }

    /// <summary>
    /// 데미지 정보 업데이트
    /// </summary>
    void UpdateDamageInfo()
    {
        // 현재 등록된 플레이어 리스트를 얻더옴
        List<Player> players = (PanelManager.GetPanel(typeof(PlayersInfoPanel)) as PlayersInfoPanel).Players;

        // 플레이어 리스트에 값이 없다면 리턴
        if (players.Count <= 0)
            return;

        // 가한데미지를 기준으로 내림 차순으로 정렬
        players.Sort((Player a, Player b) => { return b.AmountOfDamage - a.AmountOfDamage; });

        // 최고의 플레이어 닉네임 출력
        potgNameText.text = players[0].NickName;

        // 가한 최고 데미지
        //float maxDamage = players[0].AmountOfDamage;
        // 총 데미지 량
        float maxDamage = 0;
        for (int i = 0; i < players.Count; i++)
        {
            maxDamage += players[i].AmountOfDamage;
        }

        // 플레이어 리스트를 돌며 닉네임, 데미지 정보 표시
        for (int i = 0; i < players.Count; i++)
        {
            damageInfos[i].parent.SetActive(true);
            damageInfos[i].nameText.text = players[i].NickName;
            damageInfos[i].damageProgress.fillAmount = Mathf.Lerp(damageInfos[i].damageProgress.fillAmount, players[i].AmountOfDamage / maxDamage, Time.deltaTime * LERP_GAGE_RATE);
            damageInfos[i].damageText.text = players[i].AmountOfDamage.ToString();
        }
    }

    /// <summary>
    /// 보스 클리어시의 메세지 입력
    /// </summary>
    /// <param name="gameOverMsg">입력될 메세지</param>
    public void SetBossClearText(string gameOverMsg)
    {
        for (int i = 0; i < gameOverTexts.Length; i++)
        {
            gameOverTexts[i].text = gameOverMsg;
        }
    }

    /// <summary>
    /// 방 떠나기
    /// </summary>
    public void OnGotoLobbyScene()
    {
        // 방을 떠남
        GameManager.Instance.NetworkManager.LeftRoom();
    }
}
