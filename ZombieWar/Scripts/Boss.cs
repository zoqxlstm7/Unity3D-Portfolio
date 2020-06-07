using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Boss : Enemy, IPunInstantiateMagicCallback
{
    const string BOSS_CLEAR_TEXT = "좀비왕 처치!";

    public override void ResetForAlive()
    {
    }

    protected override void OnDead()
    {
        Animator.PlayAnimator("Die");

        // 좀비 데스 효과음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.Zombie_Death_SOUND);

        // 마스터 클라이언트에서만 실행
        if (GameManager.Instance.NetworkManager.IsMasterClient)
        {
            agent.destination = transform.position;
        }

        // 보스 처치 텍스트 출력
        (PanelManager.GetPanel(typeof(GameOverPanel)) as GameOverPanel).SetBossClearText(BOSS_CLEAR_TEXT);
    }

    /// <summary>
    /// 객체가 생성되었을 때 호출되는 콜백 함수
    /// </summary>
    /// <param name="info"></param>
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.IsMine)
        {
            // 룸 내 플레이어 수에 따른 체력 셋팅 RPC 실행
            photonView.RPC("SetHpRPC", RpcTarget.AllBuffered);
            // 스폰매니저에 생성된 보스 등록
            photonView.RPC("RegistBossRPC", RpcTarget.AllBuffered);
        }

        BossPanel bossPanel = PanelManager.GetPanel(typeof(BossPanel)) as BossPanel;
        // 보스패널 내 보스 체력바 업데이트 활성화
        bossPanel.IsGenerateBoss = true;
        // 보스패널 내 시네마틱 업데이트 활성화
        bossPanel.StartCinematic();
    }

    /// <summary>
    /// 룸 내 플레이어 수에 따른 체력 셋팅 RPC
    /// </summary>
    [PunRPC]
    void SetHpRPC()
    {
        maxHp = PhotonNetwork.CurrentRoom.PlayerCount * maxHp;
        currentHp = maxHp;
    }

    /// <summary>
    /// 스폰매니저에 보스 등록 RPC
    /// </summary>
    [PunRPC]
    void RegistBossRPC()
    {
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager.BossEnemy = this;
    }
}
