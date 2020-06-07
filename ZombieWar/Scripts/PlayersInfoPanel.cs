using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 정보를 표시할 객체
/// </summary>
[System.Serializable]
public class PlayerInfo
{
    public GameObject parent;   // 부모 이미지 오브젝트
    public Text nickNameText;   // 닉네임 텍스트
    public Image hpBar;         // hp 이미지
}

public class PlayersInfoPanel : BasePanel
{
    [SerializeField] PlayerHUD[] playerHUDs;    // 플레이어들의 체력바를 띄워줄 객체
    [SerializeField] PlayerInfo[] playerInfos;  // 플레이어 정보를 표시할 객체를 담을 배열

    List<Player> players = new List<Player>();  // 등록된 플레이어를 관리
    public List<Player> Players
    {
        get => players;
    }

    int registIndex = 0;                        // 등록된 플레이어 인덱스

    /// <summary>
    /// 초기화
    /// </summary>
    public override void InitializePanel()
    {
        base.InitializePanel();

        // 정보를 나타내는 부모 오브젝트 숨김 처리
        for (int i = 0; i < playerInfos.Length; i++)
        {
            playerInfos[i].parent.SetActive(false);
        }

        // 플레이어 HUD 숨김 처리
        for (int i = 0; i < playerHUDs.Length; i++)
        {
            playerHUDs[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 플레이어 등록
    /// </summary>
    /// <param name="player">등록할 플레이어</param>
    public void ResistPlayer(Player player)
    {
        // 같은 객체가 없는경우만 실행
        if (!players.Contains(player))
        {
            players.Add(player);

            if(registIndex < playerInfos.Length)
            {
                // 플레이어 HUD 셋팅
                playerHUDs[registIndex].gameObject.SetActive(true);
                playerHUDs[registIndex].SetHUD(player.transform);

                // 정보를 나타내는 부모 오브젝트 표시
                playerInfos[registIndex].parent.SetActive(true);
                registIndex++;
            }
        }
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();

        // 등록된 정보가 없는경우 리턴
        if (players.Count == 0)
            return;

        // 플레이어 정보 업데이트
        for (int i = 0; i < players.Count; i++)
        {
            UpdatePlayersInfo(players[i], playerInfos[i], playerHUDs[i]);
        }
    }

    /// <summary>
    /// 플레이어 정보 업데이트
    /// </summary>
    /// <param name="player">업데이트할 플레이어</param>
    /// <param name="playerInfo">정보를 표시할 객체</param>
    /// <param name="playerHUD">HUD 객체</param>
    void UpdatePlayersInfo(Player player, PlayerInfo playerInfo, PlayerHUD playerHUD)
    {
        // 닉네임 업데이트
        playerInfo.nickNameText.text = player.NickName;
        // 체력바 업데이트
        playerInfo.hpBar.fillAmount = player.CurrentHP / player.MaxHP;

        // HUD 닉네임 업데이트
        playerHUD.NickNameText.text = player.NickName;
        // HUD 체력바 업데이트
        playerHUD.HpBar = player.CurrentHP / player.MaxHP;
    }
}
