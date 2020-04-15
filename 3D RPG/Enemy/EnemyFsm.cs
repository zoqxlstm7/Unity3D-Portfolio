using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFsm : FSMBase
{
    NavMeshAgent agent;         // 네브 메쉬 에이전트 객체
    Animator animator;          // 애니메이터 객체

    public Transform target;    // 플레이어 위치 정보
    public GameObject combatPoint;  // combat 콜라이더

    public List<Transform> points = new List<Transform>();  // 순찰 지점을 담을 리스트
    int pointIndex = 0;                                     // 순찰 지점 인덱스

    protected void Start()
    {
        //base.Start();
        StartCoroutine(FsmMain());

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        target = PlayerManager.instance.player;
        combatPoint.SetActive(false);

        // 시작 상태는 패트롤 상태로 지정
        SetState(FsmState.PATROLL);
    }

    // FSM 메인 함수, 상태변수에 따른 코루틴함수 실행
    public override IEnumerator FsmMain()
    {
        while (true)
        {
            isNewState = false;
            yield return StartCoroutine(fsmState.ToString());
        }
    }

    // 대기 상태 처리
    public override IEnumerator IDLE()
    {
        do
        {
            yield return null;
            if (isNewState)
                break;

            // 플레이어 방향으로 회전 처리
            Vector3 dir = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        } while (!isNewState);
    }

    // 순찰 상태 처리
    public IEnumerator PATROLL()
    {
        do
        {
            yield return null;
            if (isNewState)
                break;

            animator.SetBool("run", agent.velocity != Vector3.zero);

            if (points.Count > 0)
            {
                // 순찰 지점 지정
                agent.SetDestination(points[pointIndex].position);

                // 경로 계산 중 준비되지 않은 경로가 없는경우
                if (!agent.pathPending)
                {
                    // 남아있는 거리가 정지거리보다 작거나 같은 경우
                    if(agent.remainingDistance <= agent.stoppingDistance)
                    {
                        // 탐색 경로를 가지고 있지 않거나, 속도가 0인경우 정지된 것으로 판단
                        if(!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                        {
                            // 대기 후 다음 지점 이동
                            yield return new WaitForSeconds(1.0f);
                            pointIndex++;

                            // 순찰 지점 갯수를 초과한 경우 순찰 지점 인덱스 초기화
                            if (pointIndex >= points.Count)
                                pointIndex = 0;
                        }
                    }
                }
            }
        } while (!isNewState);

        animator.SetBool("run", false);
    }

    // 추적 상태 처리
    public IEnumerator CHASING()
    {
        do
        {
            yield return null;
            if (isNewState)
                break;

            // 플레이어 포지션으로 이동
            agent.SetDestination(target.position);
            animator.SetBool("run", agent.velocity != Vector3.zero);

        } while (!isNewState);

        animator.SetBool("run", false);
    }

    // 추적을 포기하고 돌아오는 상태 처리
    public IEnumerator RETURN()
    {
        do
        {
            yield return null;
            if (isNewState)
                break;

            animator.SetBool("run", agent.velocity != Vector3.zero);

            // 마지막 순찰지점으로 이동
            agent.SetDestination(points[pointIndex].position);

            // 경로 계산 중 준비되지 않은 경로가 없는경우
            if (!agent.pathPending)
            {
                // 남아있는 거리가 정지거리보다 작거나 같은 경우
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    // 탐색 경로를 가지고 있지 않거나, 속도가 0인경우 정지된 것으로 판단
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        // 대기 후 패트롤 상태로 전환
                        yield return new WaitForSeconds(1.0f);
                        SetState(FsmState.PATROLL);
                    }
                }
            }

        } while (!isNewState);

        animator.SetBool("run", false);
    }

    // 공격 상태 처리
    public override IEnumerator ATTACK()
    {
        yield return null;
        // 공격 애니메이션 실행
        animator.SetTrigger("attack01");
        yield return new WaitForSeconds(0.5f);
        // combat 콜라이더 활성화
        combatPoint.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        // 사망상태가 아니라면 IDLE 상태 전환
        if (fsmState != FsmState.DIE)
        {
            SetState(FsmState.IDLE);
        }
    }

    // 피격 상태 처리
    public IEnumerator TAKE_HIT()
    {
        yield return null;
        // 피격 애니메이션 재생
        animator.SetTrigger("takeDamage");
        // 일정시간 대기 후 아이들 상태로 전환
        yield return new WaitForSeconds(0.8f);
        SetState(FsmState.IDLE);
    }

    // 사망 상태 처리
    public override IEnumerator DIE()
    {
        // 사망 애니메이션 재생
        animator.SetTrigger("death");
        // combat 포인트 숨김처리
        combatPoint.gameObject.SetActive(false);
        yield return new WaitForSeconds(10.0f);
        Destroy(gameObject);
    }
}
