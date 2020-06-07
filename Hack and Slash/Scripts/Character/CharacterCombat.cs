using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour
{
    CharacterStats myStats;                 //현재 유닛의 스탯정보
    CharacterStats targetStats;             //대상 유닛의 스탯정보

    public float attackCoolDown;            //공격 쿨다운
    public float attackSpeed = 2f;          //공격 속도

    public event System.Action OnAttack;    //공격 애니메이션을 실행할 이벤트함수

    private void Start()
    {
        myStats = GetComponent<CharacterStats>();
    }

    public void Attack(CharacterStats newTargetStats)
    {
        if (newTargetStats == null)
            return;

        if (newTargetStats.isDie)
            return;

        if (Time.time > attackCoolDown)
        {
            attackCoolDown = Time.time + attackSpeed;

            //새로운 스탯정보가 들어올 때만 실행
            if (targetStats != newTargetStats)
                targetStats = newTargetStats;

            //어택 애니메이션 재생
            if (OnAttack != null)
                OnAttack();
        }
    }

    //스킬로 인한 데미지
    public void SkillAttack(CharacterStats newTargetStats)
    {
        if (newTargetStats.isDie)
            return;

        newTargetStats.TakeDamage(myStats.damage.GetValue());
    }

    //애니메이션 클립에 연결되어 실행
    public void DoDamage()
    {
        if (targetStats != null)
        {
            targetStats.TakeDamage(myStats.damage.GetValue());
            targetStats = null;
        }
    }
}
