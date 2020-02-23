using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : CharacterStats
{
    CharacterUI enemyUI;

    //Enable된 경우 유닛 정보 초기화
    private void OnEnable()
    {
        enemyUI = GetComponent<CharacterUI>();

        SetRivival();

        enemyUI.FollowTargetHpBar();
        enemyUI.ActivatedHpBar(true);
        enemyUI.UpdateHpBar(maxHealth, GetCurrentHealth()); 
    }

    public override void Die()
    {
        base.Die();

        /*에너미 오브젝트의 die 처리는 상태 패턴에서 처리*/
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        enemyUI.CreateDamageText(damage);
        enemyUI.UpdateHpBar(maxHealth, GetCurrentHealth());
    }
}
