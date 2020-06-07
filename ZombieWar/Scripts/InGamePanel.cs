using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : BasePanel
{
    [SerializeField] Text remainText;       // 남은 탄알 표시
    [SerializeField] Text nickNameText;     // 닉네임 표시
    [SerializeField] Text granadeCountText; // 보유중인 수류탄 갯수 표시
    [SerializeField] Text healPackCountText;// 보유중인 힐팩 갯수 표시
    [SerializeField] Image hpBar;           // hp 표시
    [SerializeField] Image reloadBar;       // 재장전 시간 표시

    /// <summary>
    /// 초기화
    /// </summary>
    public override void InitializePanel()
    {
        base.InitializePanel();

        ActivatedReloadBar();
    }

    /// <summary>
    /// 재장전 UI 노출/숨김
    /// </summary>
    public void ActivatedReloadBar()
    {
        reloadBar.gameObject.SetActive(!reloadBar.gameObject.activeSelf);
    }

    /// <summary>
    /// 업데이트 처리
    /// </summary>
    public override void UpdatePanel()
    {
        if (!FindObjectOfType<GameManager>())
            return;

        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero == null)
            return;

        base.UpdatePanel();

        UpdateConsumalbeItem();
        UpdateRemainBullet();
        UpdateHpBar();
    }

    /// <summary>
    /// 소모품 갯수 현황 업데이트
    /// </summary>
    void UpdateConsumalbeItem()
    {
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero;

        granadeCountText.text = "x " + player.GrenadeCount;
        healPackCountText.text = "x " + player.HealPackCount;
    }

    /// <summary>
    /// 남아있는 탄알 업데이트
    /// </summary>
    void UpdateRemainBullet()
    {
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero;

        remainText.text = player.StartGun.RemainBulletCount + " / " + player.StartGun.ReloadableBulletCount;
    }

    /// <summary>
    /// HP 업데이트
    /// </summary>
    void UpdateHpBar()
    {
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero;

        hpBar.fillAmount = player.CurrentHP / player.MaxHP;
    }

    /// <summary>
    /// 재장전 UI 업데이트
    /// </summary>
    /// <param name="currentReloadTime">진행된 시간</param>
    /// <param name="reloadTime">최종 재장전 시간</param>
    public void UpdateReloadBar(float currentReloadTime, float reloadTime)
    {
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero.IsDead)
            return;

        reloadBar.fillAmount = currentReloadTime / (reloadTime + Player.RELOAD_DELAY_TIME);
    }

    /// <summary>
    /// 닉네임 설정 처리
    /// </summary>
    /// <param name="nickName">설정할 닉네임</param>
    public void SetNickName(string nickName)
    {
        nickNameText.text = nickName;
    }
}
