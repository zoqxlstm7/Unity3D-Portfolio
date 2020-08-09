using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour_RoundShot : AttackBehaviour
{
    #region Variables
    readonly string PROJECTILE_FILE_PATH = "Prefabs/Projectile";    // 발사체 파일 경로

    Actor owner;                                                    // 소유자

    [Header("Projectile Variables")]
    [SerializeField] float startAngle = 0f;                         // 시작 각도
    [SerializeField] float endAngel = 360f;                         // 끝각도
    [SerializeField] float angleInterval = 15f;                     // 각도 간격
    [SerializeField] float distance = 0.5f;                         // 생성거리
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        owner = GetComponent<Actor>();
    }
    #endregion Unity Methods

    #region AttackBehaviour Methods
    /// <summary>
    /// 공격 로직 수행 함수
    /// </summary>
    /// <param name="target">타겟</param>
    public override void ExcuteAttack(GameObject target = null)
    {
        for (float fireAngle = startAngle; fireAngle < endAngel; fireAngle += angleInterval)
        {
            // 방향을 구하고 생성거리 계산 
            Vector3 dir = new Vector3(Mathf.Cos(fireAngle * Mathf.Deg2Rad), Mathf.Sin(fireAngle * Mathf.Deg2Rad), 0);
            Vector3 generatePos = transform.position + dir * distance;

            // 발사체 생성
            Projectile projectile = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().projectileManager.Generate(PROJECTILE_FILE_PATH, generatePos);
            // 발사체 발사 실행
            projectile.Fire(owner, damage, dir, transform.position);
        }
    }
    #endregion AttackBehaviour Methods
}
