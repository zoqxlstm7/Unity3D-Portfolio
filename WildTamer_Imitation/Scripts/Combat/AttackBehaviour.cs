using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBehaviour : MonoBehaviour
{
    #region Variables
    public int priority = 0;                        // 우선순위
    [HideInInspector]
    public LayerMask targetMask;                    // 타겟 레이어

    [SerializeField] protected int damage;          // 데미지

    [SerializeField] float coolTime;                // 쿨타임
    public float currentCoolTime = 0.0f;            // 현재 쿨타임

    [SerializeField] protected float hitRange;      // 데미지를 받는 범위
    #endregion Variables

    #region Property
    // 공격 가능 여부 반환
    public bool IsAvailalbe => (currentCoolTime >= coolTime);
    #endregion Property

    #region Unity Methods
    private void Start()
    {
        // 공격을 바로 할 수 있도록 현재 쿨타임 초기화
        currentCoolTime = coolTime;
    }

    private void Update()
    {
        // 쿨타임 계산
        if (currentCoolTime < coolTime)
        {
            currentCoolTime += Time.deltaTime;
        }
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 공격 실행 로직 함수
    /// </summary>
    /// <param name="target">타겟</param>
    public abstract void ExcuteAttack(GameObject target = null);
    #endregion Other Methods
}
