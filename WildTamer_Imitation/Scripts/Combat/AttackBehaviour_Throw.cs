using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour_Throw : AttackBehaviour
{
    #region Variables
    readonly string PROJECTILE_FILE_PATH = "Prefabs/Projectile";    // 발사체 파일 경로

    Actor owner;                                                    // 소유자
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
        // 발사체 생성
        Projectile projectile = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().projectileManager.Generate(PROJECTILE_FILE_PATH, transform.position);

        // 방향을 구하고 발사체 발사 실행
        Vector3 dir = (target.transform.position - transform.position).normalized;
        projectile.Fire(owner, damage, dir, transform.position);
    }
    #endregion AttackBehaviour Methods
}
