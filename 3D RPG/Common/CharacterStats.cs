using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public delegate void OnTakeHit();   // 피격 처리 콜백 함수
    public OnTakeHit onTakeHit;

    public delegate void OnDeath();     // 사망 처리 콜백 함수
    public OnDeath onDeath;

    public float maxHealth;             //최고 체력 정보
    public float currentHealth;         //현재 체력 정보

    public Stats damage;        // 데미지 스탯
    public Stats armor;         // 방어 스탯

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        int finalValue = damage - armor.GetValue();
        finalValue = Mathf.Clamp(finalValue, 1, damage);    //clamp를 활용한 데미지 최소,최댓값 보정

        currentHealth -= finalValue;
        Debug.Log(transform.name + " Take Damage: " + damage);

        // 사망 처리
        if (currentHealth <= 0)
        {
            // 사망 처리 콜백 함수 처리
            if (onDeath != null)
                onDeath.Invoke();
        }
        else
        {
            // 피격 처리 콜백 함수 호출
            if (onTakeHit != null)
                onTakeHit.Invoke();
        }
    }
}
