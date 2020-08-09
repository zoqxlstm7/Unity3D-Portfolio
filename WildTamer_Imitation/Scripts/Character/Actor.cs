using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Actor : MonoBehaviour
{
    #region Variables
    [SerializeField] protected float maxHP;         // 최대 체력
    public float MaxHP => maxHP;
    [SerializeField] protected float currentHP;     // 현재 체력
    public float CurrentHP => currentHP;

    public float attackRange;                       // 공격 범위
    public float viewRange;                         // 보는 범위
    public float speed;                             // 이동속도
    public bool isDead = false;                     // 사망 플래그

    protected Transform target;                     // 타겟
    public LayerMask targetMask;                    // 타겟 마스크

    [HideInInspector]
    public Animator animator;                       // 애니메이터
    [HideInInspector]
    public SpriteRenderer spriteRenderer;           // 스프라이트 렌더러
    #endregion Variables

    #region Property
    // 공격 가능 거리에 있는지 반환
    public bool IsTargetInAttackRange           
    {
        get
        {
            // 타겟이 null이면 리턴
            if (target == null)
                return false;

            // 공격 가능 거리인지 검사
            float distance = Vector3.Distance(transform.position, target.position);
            return (distance <= attackRange);
        }
    }

    // 사망한 타겟인지 반환
    public bool CheckTargetIsDead
    {
        get
        {
            if (target == null)
                return true;

            return target.GetComponent<Actor>().isDead;
        }
    }
    #endregion Property

    #region Unity Methods
    private void Start()
    {
        // 컴포넌트 할당
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        InitializeActor();
    }

    private void Update()
    {
        UpdateActor();
    }

    private void FixedUpdate()
    {
        FixedUpdateActor();
    }
    #endregion Unity Methods

    #region Other Methods
    // Actor 초기화 함수
    protected virtual void InitializeActor()
    {
        animator = GetComponent<Animator>();

        currentHP = maxHP;
    }
    // Actor 업데이트 함수
    protected virtual void UpdateActor() { }
    // Actor 고정 업데이트 함수
    protected virtual void FixedUpdateActor() { }

    /// <summary>
    /// 체력 감소 처리 함수
    /// </summary>
    /// <param name="damage">받은 데미지</param>
    protected void DecreaseHP(int damage)
    {
        // 사망시 리턴
        if (isDead)
            return;

        currentHP -= damage;

        if(currentHP <= 0)
        {
            isDead = true;
            OnDead();
        }
    }

    // 사망 시 처리 함수
    protected virtual void OnDead() { }

    /// <summary>
    /// 적을 찾는 함수
    /// </summary>
    /// <returns></returns>
    public Transform SearchEnemy()
    {
        target = null;

        // 범위 내의 적을 검출
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, viewRange, targetMask);
        // 타겟 인덱스
        int targetIndex = -1;

        if (colliders.Length > 0)
        {
            // 최소거리
            float minDistance = float.MaxValue;
            for (int i = 0; i < colliders.Length; i++)
            {
                // 대상이 사망했다면 무시
                if (colliders[i].GetComponent<Actor>().isDead)
                    continue;

                // 가장 가까운 적을 타겟으로 지정
                float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
                if (minDistance > distance)
                {
                    minDistance = distance;
                    targetIndex = i;
                }
            }
        }

        // 대상이 발견됬다면 대상 리턴
        if (targetIndex != -1)
            return (target = colliders[targetIndex].transform);

        return null;
    }
    #endregion Other Methods

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, attackRange);

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, viewRange);
    }
#endif
}
