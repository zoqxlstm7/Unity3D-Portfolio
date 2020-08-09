using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour_RoundBomb : AttackBehaviour
{
    #region Variables
    readonly string BOMB_FILE_PATH = "Prefabs/Bomb";    // 폭탄 파일 경로

    [Header("Bomb Variables")]
    [SerializeField] float startAngle = 0f;             // 시작 각도
    [SerializeField] float endAngel = 360f;             // 끝각도
    [SerializeField] float angleInterval = 60f;         // 각도 간격
    [SerializeField] float distance = 2f;               // 생성될 거리
    #endregion Variables

    #region AttackBehaviour Methods
    /// <summary>
    /// 공격 로직 수행 함수
    /// </summary>
    /// <param name="target">타겟</param>
    public override void ExcuteAttack(GameObject target = null)
    {
        InGameSceneManager inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();

        for (float fireAngle = startAngle; fireAngle < endAngel; fireAngle += angleInterval)
        {
            // 방향을 구하고 생성지점 계산
            Vector3 dir = new Vector3(Mathf.Cos(fireAngle * Mathf.Deg2Rad), Mathf.Sin(fireAngle * Mathf.Deg2Rad), 0);
            dir *= distance;
            Vector3 generatePos = transform.position + dir;

            // 폭탄 생성
            Bomb bomb = inGameSceneManager.bombManager.Generate(BOMB_FILE_PATH, generatePos);
            bomb.InitBomb(damage, targetMask, hitRange);
        }
    }
    #endregion AttackBehaviour Methods
}
