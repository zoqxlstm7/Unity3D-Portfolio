using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossFsm : FSMBase
{
    NavMeshAgent agent;         // 네브 메쉬 에이전트 객체
    Animator animator;          // 애니메이터 객체

    public Transform target;    // 플레이어 위치 정보
    public GameObject combatPoint;  // combat 콜라이더

    protected void Start()
    {
        //base.Start();
        StartCoroutine(FsmMain());

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        target = PlayerManager.instance.player;
        combatPoint.SetActive(false);

        // 시작 상태는 대기 상태로 지정
        SetState(FsmState.IDLE);
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
        } while (!isNewState);
    }

    // 추적 상태 처리
    public IEnumerator CHASING()
    {
        animator.SetBool("run", true);

        do
        {
            yield return null;
            if (isNewState)
                break;

            // 플레이어 포지션으로 이동
            agent.SetDestination(target.position);

        } while (!isNewState);

        agent.SetDestination(transform.position);
        animator.SetBool("run", false);
    }

    // 공격 준비 상태
    public IEnumerator PREPARATION()
    {
        animator.SetTrigger("preparation");

        do
        {
            yield return null;
            if (isNewState)
                break;

            // 플레이어 방향으로 회전 처리
            Vector3 dir = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // 공격 준비 동작이 끝난 후 공격 상태 처리
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f
                && animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Preparation"))
            {
                SetState(FsmState.ATTACK);
                break;
            }

        } while (!isNewState);
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
