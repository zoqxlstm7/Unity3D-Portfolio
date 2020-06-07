using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    EnemyFsm enemyFsm;

    [SerializeField] float attackRadius = 2f;   // 공격 범위  
    [SerializeField] float chasingRadius = 5f;  // 추격 범위

    [SerializeField] float attackCooltime = 0f; // 공격 쿨타임
    [SerializeField] float attackSpeed = 1f;    // 공격 속도

    bool isDie = false; // 사망 유무

    private void Start()
    {
        enemyFsm = GetComponent<EnemyFsm>();

        // 피격 처리 콜백 함수 연결
        GetComponent<CharacterStats>().onTakeHit += OnTakeHit;
        // 사망 처리 콜백 함수 연결
        GetComponent<CharacterStats>().onDeath += OnDeath;
    }

    private void Update()
    {
        if (isDie)
            return;

        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(transform.position, enemyFsm.target.position);

        // 공격 범위 내가 아니고, 추적 범위 안에 들어왔고 추적 상태가 아니라면 추적상태 실행
        if (distance > attackRadius && distance <= chasingRadius && enemyFsm.fsmState != FsmState.CHASING)
        {
            enemyFsm.SetState(FsmState.CHASING);
        // 추적 범위을 벗어났고, 추적 상태라면 리턴상태 실행
        }else if(distance > chasingRadius && enemyFsm.fsmState == FsmState.CHASING)
        {
            enemyFsm.SetState(FsmState.RETURN);
        }

        // 피격 상태가 아닌 경우 공격 루틴 처리
        if(enemyFsm.fsmState != FsmState.TAKE_HIT)
        {
            // 공격 쿨다운 감소
            attackCooltime -= Time.deltaTime;

            // 공격사정거리에 들어왔고 공격 쿨다운이 다 됐다면 공격 상태 실행
            if (distance <= attackRadius && attackCooltime <= 0f)
            {
                attackCooltime = 1f / attackSpeed;
                enemyFsm.SetState(FsmState.ATTACK);
            }
        }
    }

    // 피격 처리 함수
    void OnTakeHit()
    {
        enemyFsm.SetState(FsmState.TAKE_HIT);
    }

    // 사망 처리 함수
    void OnDeath()
    {
        if (!isDie)
        {
            isDie = true;
            enemyFsm.SetState(FsmState.DIE);
            // 드랍아이템 스크립트 호출
            GetComponent<DropItemCtrl>().DropItem();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 추격 감지 범위 기즈모
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chasingRadius);

        // 공격 감지 범위 기즈모
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
