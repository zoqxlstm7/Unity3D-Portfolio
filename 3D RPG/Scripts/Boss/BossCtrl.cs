using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCtrl : MonoBehaviour
{
    BossFsm bossFsm;

    [SerializeField] float attackRadius = 2f;   // 공격 범위  

    [SerializeField] float attackCooltime = 0f; // 공격 쿨타임
    [SerializeField] float attackSpeed = 1f;    // 공격 속도

    bool isDie = false; // 사망 유무

    private void Start()
    {
        bossFsm = GetComponent<BossFsm>();

        // 사망 처리 콜백 함수 연결
        GetComponent<CharacterStats>().onDeath += OnDeath;
    }

    private void Update()
    {
        // 플레이어가 보스구역으로 들어오지 않았다면 리턴
        if (!BossBattleManager.instance.isStart)
            return;

        if (isDie)
            return;

        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(transform.position, bossFsm.target.position);

        // 공격 쿨다운 감소
        attackCooltime -= Time.deltaTime;

        // 공격사정거리에 들어왔고 공격 쿨다운이 다 됐다면 공격 상태 실행
        if (distance <= attackRadius)
        {
            if(attackCooltime <= 0f)
            {
                attackCooltime = 1f / attackSpeed;
                // 공격 준비 상태가 아닌경우 공격 준비 처리
                if(bossFsm.fsmState != FsmState.PREPARATION)
                    bossFsm.SetState(FsmState.PREPARATION);
            }
        }
        else
        {
            // 공격 사정거리가 아니면 추적 상태 실행
            if (bossFsm.fsmState == FsmState.IDLE)
                bossFsm.SetState(FsmState.CHASING);
        }
    }

    // 사망 처리 함수
    void OnDeath()
    {
        if (!isDie)
        {
            isDie = true;
            BossBattleManager.instance.isStart = false;
            bossFsm.SetState(FsmState.DIE);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 공격 감지 범위 기즈모
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
