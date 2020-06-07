using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPack : ThrowItem
{
    const string HEALPACK_EFFECT_FILE_PATH = "Effect/HealPackEffect"; // 힐팩 이펙트 파일 경로

    /// <summary>
    /// 목표지점에 도착한 후에 처리를 실행
    /// </summary>
    /// <param name="attacker">공격자 객체</param>
    public override void AfterThrow(Actor attacker)
    {
        // 범위 내 콜라이더를 가진 객체를 얻어옴
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        // 길이가 0이상일 때
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                // 레이어가 플레이어인 객체 중 첫번째 걸린 플레이어만 체력회복 후 종료
                if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Actor target = colliders[i].gameObject.GetComponent<Actor>();

                    // 마스터 클라이언트에서만 회복 처리
                    if (GameManager.Instance.NetworkManager.IsMasterClient)
                    {
                        // 회복 처리
                        target.OnTakeHeal(value);
                    }

                    // 정보 UI 노출
                    GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().AcquireUIManager.ActivatedInfoUI(value, target, AcquireType.HEAL);

                    break;
                }
            }
        }

        // 힐팩 이펙트 생성
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Generate(HEALPACK_EFFECT_FILE_PATH, transform.position);

        base.AfterThrow(attacker);
    }

    /// <summary>
    /// 기즈모 표시
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
