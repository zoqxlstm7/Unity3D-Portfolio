using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Actor
{
    // 플레이어 및 거리 정보 저장 클래스
    class DistanceData
    {
        public Actor actor;
        public float distance;

        public DistanceData(Actor actor, float distance)
        {
            this.actor = actor;
            this.distance = distance;
        }
    }

    const float REMOVE_TIME = 3.0f;                 // 제거될 때까지 걸리는 시간

    [SerializeField] MotionState state;             // 모션 상태 변수

    [SerializeField] Transform target;              // 타겟 객체
    [SerializeField] protected NavMeshAgent agent;            // Nav Mesh 객체
    [SerializeField] CapsuleCollider capsuleCollider;// 콜라이더 컴포넌트

    [SerializeField] float attackRange;             // 공격 범위
    [SerializeField] float attackSpeed;             // 공격 속도
    [SerializeField] int damage;                    // 데미지
        
    float lastActionTime;                           // 마지막 액션 시간
    string filePath;                                // 파일 경로
    public string FilePath
    {
        set => filePath = value;
    }

    Actor hitPerson;                                // 공격을 당한 대상

    private void OnEnable()
    {
        ResetForAlive();
    }

    /// <summary>
    /// 초기화 처리
    /// </summary>
    protected override void InitializeActor()
    {
        base.InitializeActor();

        // nav mesh 속도값을 지정한 속도값으로 지정
        agent.speed = speed;
    }

    /// <summary>
    /// 업데이트 처리
    /// </summary>
    protected override void UpdateActor()
    {
        // 포톤뷰가 내것인거만 실행
        if (photonView.IsMine)
        {
            BossPanel bossPanel = PanelManager.GetPanel(typeof(BossPanel)) as BossPanel;

            // 게임 오버시 리턴 및 보스연출시 리턴
            if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().IsGameOver
                || bossPanel.IsCinematic)
            {
                agent.SetDestination(transform.position);
                Animator.PlayAnimator("Run", false);
                return;
            }

            // 사망 시 리턴
            if (isDead)
                return;

            base.UpdateActor();

            UpdateMove();
            FindCloseTarget();
            CheckAttackRange();
        }
    }

    /// <summary>
    /// 움직임 처리
    /// </summary>
    void UpdateMove()
    {
        // 속도를 체크하여 애니메이션 재생
        if (agent.velocity != Vector3.zero)
            Animator.PlayAnimator("Run", true);
        else
            Animator.PlayAnimator("Run", false);

        // None이 아닌 상태일 경우 리턴
        if (state != MotionState.NONE)
            return;

        // 타겟이 없는 경우 리턴
        if (target == null)
            return;

        // 더이상 타겟이 없는 경우 리턴 및 제자리 고정
        if (target.GetComponent<Actor>().IsDead)
        {
            agent.SetDestination(transform.position);
            return;
        }

        // 타겟지점으로 이동
        agent.SetDestination(target.position);
    }

    /// <summary>
    /// 가까운 적을 찾는 함수
    /// </summary>
    void FindCloseTarget()
    {
        // 대상과 거리를 담을 변수
        List<DistanceData> dataList = new List<DistanceData>();

        // 등록된 player 객체 반환받음
        PlayersInfoPanel playersInfoPanel = PanelManager.GetPanel(typeof(PlayersInfoPanel)) as PlayersInfoPanel;
        List<Player> players = playersInfoPanel.Players;

        // 플레이어가 없는 경우 리턴
        if (players.Count == 0)
            return;

        // 거리 계산 후 대상과 거리를 저장
        for (int i = 0; i < players.Count; i++)
        {
            // 사망한 플레이어는 스킵
            if (players[i].IsDead)
                continue;

            float distance = Vector3.Distance(transform.position, players[i].transform.position);

            DistanceData data = new DistanceData(players[i], distance);
            dataList.Add(data);
        }

        // 거리를 기준으로 오름차순으로 정렬
        dataList.Sort((DistanceData a, DistanceData b) => { return (int)(a.distance - b.distance); });

        // 가장 가까운 플레이어를 대상으로 지정
        if (dataList.Count > 0)
            target = dataList[0].actor.transform;
        else
            target = null;
    }

    /// <summary>
    /// 공격 범위내에 적이 있는지 확인
    /// </summary>
    void CheckAttackRange()
    {
        if (target == null)
            return;

        // 거리 확인
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            // 범위 내에 있을 시 대상을 주시
            TargetToFace(target.position);

            // 공격 속도에 맞춰 공격 실행
            if (Time.time - lastActionTime >= attackSpeed)
            {
                hitPerson = target.GetComponent<Actor>();

                state = MotionState.ATTACK;
                OnAttack(damage);
                lastActionTime = Time.time;
            }
        }
    }

    /// <summary>
    /// 타겟지점을 바라보도록 처리하는 함수
    /// </summary>
    /// <param name="targetPos">바라볼 지점</param>
    void TargetToFace(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = lookRotation;
    }

    /// <summary>
    /// 공격 처리 함수
    /// </summary>
    /// <param name="damage">가할 데미지</param>
    public override void OnAttack(int damage)
    {
        base.OnAttack(damage);

        StartCoroutine(AttackDelay());

        if(hitPerson != null)
        {
            hitPerson.OnTakeHit(damage, this);
            hitPerson = null;
        }
    }

    /// <summary>
    /// 공격 후 딜레이 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(2.0f);
        state = MotionState.NONE;

        #region 애니메이션 시간체크
        //if (!Animator.GetAnimator().IsInTransition(0))
        //{
        //    state = State.NONE;
        //    lastActionTime = Time.time;
        //    Debug.Log("out");
        //}

        //while (Animator.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack")
        //   && Animator.GetAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        //{
        //    yield return null;

        //    state = State.NONE;
        //    lastActionTime = Time.time;
        //    Debug.Log("in");

        //    break;
        //}
        #endregion
    }

    /// <summary>
    /// 죽었을 때의 처리
    /// </summary>
    protected override void OnDead()
    {
        base.OnDead();

        // 좀비 데스 효과음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.Zombie_Death_SOUND);

        // 마스터 클라이언트에서만 실행
        if (GameManager.Instance.NetworkManager.IsMasterClient)
        {
            agent.destination = transform.position;
            photonView.RPC("SetColliderEnabled", RpcTarget.AllBuffered, false);
            StartCoroutine(DeadDelay());

            // 킬카운트 RPC 실행
            photonView.RPC("KillCountRPC", RpcTarget.AllBuffered);

            // 아이템 드랍 실행
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().ItemDropManager.DropTale(attacker);
        }
    }

    /// <summary>
    /// 콜아이더 enalbe 동기화
    /// </summary>
    /// <param name="value"></param>
    [PunRPC]
    void SetColliderEnabled(bool value)
    {
        capsuleCollider.enabled = value;
    }

    /// <summary>
    /// 킬카운트 RPC
    /// </summary>
    [PunRPC]
    void KillCountRPC()
    {
        // 킬카운트 증가
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager.KillCount++;
    }

    /// <summary>
    /// 죽은 후 딜레이 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator DeadDelay()
    {
        yield return new WaitForSeconds(REMOVE_TIME);
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EnemyManager.Remove(filePath, this);
    }

    /// <summary>
    /// 살아났을 때의 초기화 처리
    /// </summary>
    public virtual void ResetForAlive()
    {
        isDead = false;
        photonView.RPC("SetColliderEnabled", RpcTarget.AllBuffered, true);
        currentHp = maxHp;
    }

    /// <summary>
    /// 활성/비활성 처리
    /// </summary>
    public void SetActivated()
    {
        photonView.RPC("SetActivatedRPC", RpcTarget.AllBuffered);
    }

    /// <summary>
    /// 활성/비활성 RPC 함수
    /// </summary>
    [PunRPC]
    void SetActivatedRPC()
    {         
        gameObject.SetActive(!gameObject.activeSelf);
    }

    /// <summary>
    /// 공격 범위 기즈모 표시
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
