using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor, IAttackable, IDamageable
{
    #region Variables
    readonly int MoveHash = Animator.StringToHash("Move");      // 이동 애니메이션
    readonly int AttackHash = Animator.StringToHash("Attack");  // 공격 애니메이션
    readonly int DeadHash = Animator.StringToHash("Dead");      // 사망 애니메이션

    Vector3 inputDirection;                                     // 입력 방향 
    public Vector3 InputDirection => inputDirection;

    [SerializeField] AttackBehaviour attackBehaviour;           // 공격 행동

    public List<Animal> tamingList = new List<Animal>();        // 테이밍한 동물

    [HideInInspector]
    public AroundPointFinder aroundPointFinder;                 // 주변지점을 찾아주는 객체
    #endregion Variables

    #region Property
    public int TamingCount => tamingList.Count;                 // 테이밍 카운트 반환
    #endregion Property
    
    #region Actor Methods
    /// <summary>
    /// Actor 초기화 함수
    /// </summary>
    protected override void InitializeActor()
    {
        base.InitializeActor();

        aroundPointFinder = GetComponent<AroundPointFinder>();

        // 초기 공격 행동 설정
        InitAttackBehaviour();
    }
   
    /// <summary>
    /// Actor 업데이트 함수
    /// </summary>
    protected override void UpdateActor()
    {
        // 사망시 리턴
        if (isDead)
            return;

        UpdateMove();
        CheckTargetInAttackRange();
    }

    /// <summary>
    /// Actor Fixed 업데이트 함수
    /// </summary>
    protected override void FixedUpdateActor()
    {
        FixedUpdateeMove();
    }

    /// <summary>
    /// 사망시 처리 함수
    /// </summary>
    protected override void OnDead()
    {
        animator.SetTrigger(DeadHash);
    }
    #endregion Actor Methods

    #region Other Methods
    /// <summary>
    /// 이동 방향 업데이트 함수
    /// </summary>
    void UpdateMove()
    {
        float h = JoyStick.GetHorizontalValue();
        float v = JoyStick.GetVerticalValue();

        // 스트라이트 flip 설정
        if(h != 0)
            spriteRenderer.flipX = (h > 0);

        inputDirection = new Vector3(h, v, 0);

        if (inputDirection != Vector3.zero)
            aroundPointFinder.SetAroundPosition();
    }

    /// <summary>
    /// 물리 이동 업데이트 함수
    /// </summary>
    void FixedUpdateeMove()
    {
        // 입력 방향으로 움직임
        inputDirection = inputDirection.normalized * speed * Time.deltaTime;
        transform.position += inputDirection;

        // 애니메이션 재생
        if (inputDirection != Vector3.zero)
            animator.SetBool(MoveHash, true);
        else
            animator.SetBool(MoveHash, false);

        inputDirection = Vector3.zero;
    }

    /// <summary>
    /// 초기 공격 행동 초기화 함수
    /// </summary>
    void InitAttackBehaviour()
    {
        CurrentAttackBehaviour = attackBehaviour;
    }

    /// <summary>
    /// 타겟이 공격 범위안에 들어왔는지 검사하는 함수
    /// </summary>
    void CheckTargetInAttackRange()
    {
        // 공격할 수 없다면 리턴
        if (!CurrentAttackBehaviour.IsAvailalbe)
            return;

        // 적을 찾음
        SearchEnemy();

        if (IsTargetInAttackRange)
        {
            // 공격 쿨타임 초기화
            CurrentAttackBehaviour.currentCoolTime = 0.0f;
            // 공격 애니메이션 재생
            animator.SetTrigger(AttackHash);
        }
    }
    #endregion Other Methods

    #region IAttackable Interface
    public AttackBehaviour CurrentAttackBehaviour
    {
        get;
        private set;
    }

    public void OnExecuteAttack()
    {
        // 공격 행동이 없다면 리턴
        if (CurrentAttackBehaviour == null)
            return;

        // 타겟 레이어 할당
        CurrentAttackBehaviour.targetMask = targetMask;
        // 공격 로직 실행
        CurrentAttackBehaviour.ExcuteAttack(target.gameObject);
    }
    #endregion IAttackable Interface

    #region IDamageable Interface
    /// <summary>
    /// 데미지 처리 함수
    /// </summary>
    /// <param name="damage">데미지</param>
    public void TakeDamage(int damage)
    {
        DecreaseHP(damage);
    }
    #endregion IDamageable Interface
}
