using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    PlayerAnimator myAnimator;
    Motor motor;

    public GameObject combatPoint; // combat 콜라이더

    float v, h;

    [SerializeField] float attackSpeed = 1f;         // 공격 속도
    [SerializeField] float attackCooldown = 0f;      // 공격 쿨다운
    [SerializeField] float dodgeCooldown = 0f;       // 닷지 쿨다운

    private void Start()
    {
        myAnimator = GetComponent<PlayerAnimator>();
        motor = GetComponent<Motor>();

        // 피격 처리 콜백 함수 연결
        GetComponent<CharacterStats>().onTakeHit += OnTakeHit;
        // 사망 처리 콜백 함수 연결
        GetComponent<CharacterStats>().onDeath += OnDeath;

        // combat 콜라이더 숨김 처리
        combatPoint.SetActive(false);
    }

    private void Update()
    {
        // 사망시 컨트롤 제어
        if (myAnimator.state == State.DIE)
            return;

        // 전투관련 커맨드 호출
        CombatCommand();

        v = Input.GetAxisRaw("Vertical");
        h = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        // 사망시 컨트롤 제어
        if (myAnimator.state == State.DIE)
            return;

        // IDLE 상태일 경우 move함수 처리
        if (myAnimator.state == State.IDLE)
            motor.Move(v, h);
    }

    // 전투관련 커맨드 처리
    void CombatCommand()
    {
        // 공격 쿨다운 감소
        attackCooldown -= Time.deltaTime;

        // 좌클릭시 공격 처리
        if (Input.GetMouseButtonDown(0))
        {
            // 공격 쿨다운이 된 경우만 공격 모션 수행
            if(attackCooldown <= 0f)
            {
                myAnimator.AttackAnimation();
                attackCooldown = 1f / attackSpeed;
                myAnimator.SetState(State.ATTACK);

                StartCoroutine(Attack());
                StartCoroutine(TurnIdle());
            }
        }

        // 닷지 쿨다운 감소
        dodgeCooldown -= Time.deltaTime;

        // 스페이스 입력시 닷지 처리
        //if (Input.GetKeyDown(KeyCode.Space) && (myAnimator.state == State.RUN || myAnimator.state == State.IDLE))
        if (Input.GetKeyDown(KeyCode.Space) && dodgeCooldown <= 0f)
        {
            //myAnimator.TriggerAnimation("dodge");
            //myAnimator.SetState(State.DODGE);

            //StartCoroutine(TurnIdle());

            dodgeCooldown = 3f;

            // 닷지 애니메이션 실행
            myAnimator.BoolAnimation("dodge", true);
            myAnimator.SetState(State.DODGE);

            StartCoroutine(Dodge());
        }
    }

    // 피격 처리 함수
    void OnTakeHit()
    {
        myAnimator.TriggerAnimation("takeDamage");
    }

    // 사망 처리 함수
    void OnDeath()
    {
        myAnimator.SetState(State.DIE);
        myAnimator.TriggerAnimation("death");
        // 특정 지점에서 다시 시작
        PlayerManager.instance.KillPlayer();
    }

    // 공격시 상태 처리
    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.5f);
        combatPoint.SetActive(true);
    }

    IEnumerator Dodge()
    {
        float t = 0;
        // 대쉬 이동 처리
        while (t < 0.9f)
        {
            yield return null;

            transform.Translate(Vector3.forward * 20f * Time.deltaTime);
            t += Time.deltaTime * 4.5f;
        }

        // 닷지 애니메이션 종료
        myAnimator.BoolAnimation("dodge", false);
        myAnimator.SetState(State.IDLE);
    }

    // IDLE 상태로 되돌림
    IEnumerator TurnIdle()
    {
        yield return new WaitForSeconds(1f);
        myAnimator.SetState(State.IDLE);
    }
}
