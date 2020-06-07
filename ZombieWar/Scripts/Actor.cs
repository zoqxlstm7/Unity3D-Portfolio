using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum MotionState
{
    NONE,
    SHOOT,
    RELOAD,
    ATTACK,
    Throw,
    DIE
}

public class Actor : MonoBehaviourPun
{
    AnimatorController animator;        // 애니메이터 객체 반환
    public AnimatorController Animator
    {
        get
        {
            if (animator == null)
                animator = new AnimatorController(GetComponent<Animator>());

            return animator;
        }
    }

    [SerializeField] protected float maxHp;             // 최대 체력
    public float MaxHP
    {
        get => maxHp;
    }
    [SerializeField] protected float currentHp;         // 현재 체력
    public float CurrentHP
    {
        get => currentHp;
    }
    protected bool isDead;                              // 사망 여부
    public bool IsDead
    {
        get => isDead;
    }

    protected int amountOfDamage = 0;                   // 가한 피해량
    public int AmountOfDamage
    {
        get => amountOfDamage;
    }

    [SerializeField] protected float speed;             // 이동 속도
    [SerializeField] protected Actor attacker;                           // 공격자

    private void Start()
    {
        InitializeActor();
    }

    private void Update()
    {
        UpdateActor();
    }

    /// <summary>
    /// 초기화 처리 함수
    /// </summary>
    protected virtual void InitializeActor()
    {
        currentHp = maxHp;
    }

    /// <summary>
    /// 업데이트 처리 함수
    /// </summary>
    protected virtual void UpdateActor()
    {
    }

    /// <summary>
    /// 공격 처리
    /// </summary>
    /// <param name="damage"></param>
    public virtual void OnAttack(int damage)
    {
        //Debug.Log("OnAttack: " + damage);

        // 공격 애니메이션
        Animator.PlayAnimator("Attack");
    }

    /// <summary>
    /// 가한 피해량 합산 함수
    /// </summary>
    /// <param name="damage">가한 데미지</param>
    public void AddAmountOfDamage(int damage)
    {
        // 데미지 누적 처리
        amountOfDamage += damage;
    }

    /// <summary>
    /// 데미지를 입었을 때의 처리
    /// </summary>
    /// <param name="damage">받는 데미지</param>
    /// <param name="attacker">공격자 객체</param>
    public virtual void OnTakeHit(int damage, Actor attacker)
    {
        //Debug.Log("OnTakeHit: " + damage);

        // 공격자 지정
        this.attacker = attacker;
        // 체력감소 처리
        DecreaseHP(damage);
    }

    /// <summary>
    /// 체력 감소 처리
    /// </summary>
    /// <param name="damage">받는 데미지</param>
    protected virtual void DecreaseHP(int damage)
    {
        if (isDead)
            return;

        // 체력감소 RPC 실행
        photonView.RPC("DecreaseHpRPC", RpcTarget.All, damage);
    }

    /// <summary>
    /// 체력감소 RPC 처리
    /// </summary>
    /// <param name="damage"></param>
    [PunRPC]
    public void DecreaseHpRPC(int damage)
    { 
        //Debug.Log(name + photonView.ViewID + ": DecreaseHpRPC ");
        currentHp -= damage;

        if (currentHp <= 0)
        {
            currentHp = 0;
            isDead = true;

            OnDead();
        }
    }

    /// <summary>
    /// 회복될 때의 처리
    /// </summary>
    /// <param name="value">회복량</param>
    public virtual void OnTakeHeal(int value)
    {
        IncreaseHP(value);
    }

    /// <summary>
    /// 체력 회복 함수
    /// </summary>
    /// <param name="value">회복량</param>
    protected void IncreaseHP(int value)
    {
        if (isDead)
            return;

        // 체력 회복 RPC 실행
        photonView.RPC("IncreaseHpRPC", RpcTarget.All, value);
    }

    /// <summary>
    /// 체력 회복 RPC 함수
    /// </summary>
    /// <param name="value">회복량</param>
    [PunRPC]
    public void IncreaseHpRPC(int value)
    {
        //Debug.Log("Recovery: " + value);
        currentHp += value;

        if(currentHp > maxHp)
            currentHp = maxHp;
    }

    /// <summary>
    /// 사망 후 처리
    /// </summary>
    protected virtual void OnDead()
    {
        //Debug.Log("Dead");
        Animator.PlayAnimator("Die");
    }
}
