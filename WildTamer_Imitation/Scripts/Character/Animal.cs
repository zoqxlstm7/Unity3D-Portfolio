using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(PathFinderAgent))]
[RequireComponent(typeof(AroundPointFinder))]
public class Animal : Actor, IAttackable, IDamageable
{
    #region Variables
    protected StateMachine<Animal> stateMachine;        // 상태 머신
    public StateMachine<Animal> StateMachine => stateMachine;

    [HideInInspector]
    public PathFinderAgent agent;                       // 길찾기 에이전트
    [HideInInspector]
    public Transform[] waypoints;                       // 웨이포인트지점
    [HideInInspector]
    public int waypointIndex = -1;                      // 웨이포인트 인덱스

    int originSortIndex;                                // 원래의 스프라이트 정렬순서

    [SerializeField]                                    // 공격 행동
    protected List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();
    #endregion Variables

    #region Property
    public string FilePath { get; set; }                // 파일 경로

    bool isAttack;
    public bool IsAttack
    {
        get => isAttack;
        set
        {
            if (value)
                CurrentAttackBehaviour.currentCoolTime = 0.0f;

            isAttack = value;
        }
    }
    #endregion Property

    #region Unity Methods
    private void OnEnable()
    {
        ResetVariables();
    }
    #endregion Unity Methods

    #region Actor Methods
    protected override void InitializeActor()
    {
        base.InitializeActor();

        // 정렬순서 초기화
        originSortIndex = spriteRenderer.sortingOrder;

        // 길찾기 에이전트 초기화
        agent = GetComponent<PathFinderAgent>();
        agent.Speed = speed;

        // 상태머신 초기화
        stateMachine = new StateMachine<Animal>(this, new IdleState());
        // 상태 설정
        SetState();

        // 초기 공격 행동 설정
        InitAttackBehaviour();
    }

    protected override void UpdateActor()
    {
        // 사용가능한 공격 행동 검사
        CheckAttackBehaviour();

        // 상태 업데이트
        stateMachine.UpdateState(Time.deltaTime);

        // sorting order 업데이트
        UpdateSpriteSortingOrder();
    }

    protected override void OnDead()
    {
    }
    #endregion Actor Methods

    #region Other Methods
    /// <summary>
    /// 동물 객체 초기화 함수
    /// </summary>
    void ResetVariables()
    {
        // 변수 초기화
        currentHP = maxHP;
        waypointIndex = -1;

        // 쿨타임 초기화
        if (CurrentAttackBehaviour != null)
            CurrentAttackBehaviour.currentCoolTime = 0.0f;

        // 대기상태로 전환
        if(stateMachine != null)
            stateMachine.ChangeState<IdleState>();

        // 에이전트 시작
        if(agent != null)
            agent.StartAgent();

        isDead = false;
    }

    /// <summary>
    /// 상태 초기화 함수
    /// </summary>
    protected virtual void SetState()
    {
        // 상태 등록
        stateMachine.RegistState(new MoveState());
        stateMachine.RegistState(new MoveWaypointState());
        stateMachine.RegistState(new AttackState());
        stateMachine.RegistState(new DeadState());
        // 아군일때의 상태
        stateMachine.RegistState(new FollowMoveState());
    }

    /// <summary>
    /// 초기 공격 행동 초기화 함수
    /// </summary>
    void InitAttackBehaviour()
    {
        foreach (AttackBehaviour behaviour in attackBehaviours)
        {
            // 공격 행동 할당
            if (CurrentAttackBehaviour == null)
            {
                CurrentAttackBehaviour = behaviour;
            }

            // 타겟 레이어 할당
            behaviour.targetMask = targetMask;
        }
    }

    /// <summary>
    /// 사용할 수 있는 공격 행동을 검사하는 함수
    /// </summary>
    void CheckAttackBehaviour()
    {
        // 공격 진행중이 아니라면
        if (!isAttack)
        {
            CurrentAttackBehaviour = null;
            foreach (AttackBehaviour behaviour in attackBehaviours)
            {
                // 사용할 수 있는 공격해동을 우선순위에 따라 설정
                if (behaviour.IsAvailalbe)
                {
                    if (CurrentAttackBehaviour == null || CurrentAttackBehaviour.priority < behaviour.priority)
                    {
                        CurrentAttackBehaviour = behaviour;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 플레이어와 sorting order를 비교하여 업데이트해주는 함수
    /// </summary>
    void UpdateSpriteSortingOrder()
    {
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().player;

        Vector3 playerPos = player.transform.position;
        int playerSortingIndex = player.spriteRenderer.sortingOrder;

        // 플레이어보다 뒤쪽에 있다면 오더 -1, 앞쪽에 있다면 +1
        spriteRenderer.sortingOrder = playerPos.y < transform.position.y
            ? originSortIndex + playerSortingIndex - 1 : originSortIndex + playerSortingIndex + 1;
    }

    /// <summary>
    /// 플립 설정 함수
    /// </summary>
    /// <param name="targetPos">목표지점</param>
    public void SetFlip(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        spriteRenderer.flipX = (dir.x > 0);
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
        // 공격 로직 실행
        CurrentAttackBehaviour.ExcuteAttack(target.gameObject);
        // 공격완료
        isAttack = false;
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
