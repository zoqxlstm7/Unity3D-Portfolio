using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour_Melee : AttackBehaviour
{
    #region AttackBehaviour Methods
    /// <summary>
    /// 공격 로직 수행 함수
    /// </summary>
    /// <param name="target">타겟</param>
    public override void ExcuteAttack(GameObject target = null)
    {
        // IDamageable 인터페이스를 가지고 있다면 데미지를 입힘
        target.GetComponent<IDamageable>()?.TakeDamage(damage);
    }
    #endregion AttackBehaviour Methods
}
