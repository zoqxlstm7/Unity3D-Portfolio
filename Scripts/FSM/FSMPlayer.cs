using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMoter))]
[RequireComponent(typeof(CharacterCombat))]
public class FSMPlayer : FSMBase
{
    PlayerMoter playerMoter;        //플레이어 이동 제어 클래스
    CharacterCombat combat;         //플레이어 공격 제어 클래스

    public AudioSource audioSource;

    public LayerMask movementMask;
    public LayerMask enemyMask;
    Camera cam;

    public int attackAnimNum = 4;   //플레이어 일반공격 애니메이션의 수
    int attackNum = 0;              //일반 공격 횟수

    Interactable focus;             //현재 인터랙트된 오브젝트

    Vector3 targetPosition;         //마우스 클릭으로 클릭된 지점을 저장
    bool useSkill = false;          //스킬 사용중인지의 여부

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();

        playerMoter = GetComponent<PlayerMoter>();

        cam = Camera.main;

        combat = GetComponent<CharacterCombat>();
        combat.OnAttack += OnAttack;   
    }

    private void Update()
    {
        if (!PlayerManager.isMove)
            return;

        //스킬사용
        if (!useSkill)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                useSkill = true;

                SetState(UnitState.Skill01);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                useSkill = true;

                SetState(UnitState.Skill02);
            }
        }

        //왼쪽버튼 클릭시 해당 지점으로 이동
        if (Input.GetMouseButtonDown(0) && state != UnitState.Skill02) 
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, movementMask))
            {
                attackNum = 0;
                RemoveFocus();

                if(!useSkill)
                    SetState(UnitState.Run);

                playerMoter.MoveToPoint(hit.point);
                targetPosition = hit.point;
            }
        }

        //오른쪽버튼 클릭시 상호작용 가능한 오브젝트를 포커스
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f) && !useSkill)
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable != null)
                {
                    SetFocus(interactable);
                    targetPosition = hit.point;
                }
            }
        }
    }

    void SetFocus(Interactable newFocus)
    {
        //같은 오브젝트가 선택되었는지 검사
        if(focus != newFocus)
        {
            //포커스가 null이 아니라면 기존 focus삭제
            if (focus != null)
                focus.OnDeFocused();

            //state를 run으로 바꾸고 포커스된 오브젝트에게 다가감
            focus = newFocus;
            playerMoter.FollowTarget(newFocus);
            SetState(UnitState.Run);
        }

        newFocus.OnFocused(transform);
    }

    //현재 인터랙트된 오브젝트 삭제
    void RemoveFocus()
    {
        if (focus != null)
            focus.OnDeFocused();

        focus = null;
        playerMoter.StopFollowingTarget();
    }

    void OnAttack()
    {
        if(!useSkill)
            SetState(UnitState.Attack);
    }

    protected override IEnumerator Attack()
    {
        yield return null;

        if (attackNum >= attackAnimNum)
            attackNum = 0;

        attackNum += 1;

        //애니메이션 작동
        animator.SetTrigger("Attack0" + attackNum);
        //audioSource.clip = SoundManager.instance.AttackSound();
        //audioSource.Play();

        SetState(UnitState.Idle);
    }

    IEnumerator Skill01()
    {
        //애니메이션 재생
        animator.SetTrigger("Skill01");
        float delayTime = Time.time;        //스킬 공격 딜레이
        float attackRadius = 3f;            //스킬 공격 범위 설정

        do
        {
            yield return null;
            if (isNewState)
                break;

            //액션 구현
            if(Time.time - delayTime > 0.2f)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, attackRadius, enemyMask);
                CharacterStats enemyStats;

                for (int i = 0; i < colliders.Length; i++)
                {
                    enemyStats = colliders[i].GetComponent<CharacterStats>();
                    
                    if (enemyStats != null)
                    {
                        //적의 생명력이 다한 경우 이후 연산을 처리하지 않음
                        if (enemyStats.GetCurrentHealth() <= 0)
                            continue;

                        combat.SkillAttack(enemyStats);
                    }
                }
                delayTime = Time.time;

                audioSource.clip = SoundManager.instance.Skill01Sound();
                audioSource.Play();
            } 
        } while (!isNewState);
    }

    IEnumerator Skill02()
    {
        animator.SetTrigger("Skill02");

        float t = 0f;

        FaceTarget(targetPosition);

        do
        {
            yield return null;
            if (isNewState)
                break;

            if(agent.velocity != Vector3.zero)
                transform.position = Vector3.Lerp(transform.position, targetPosition, t);

            t += Time.deltaTime;

            if(t >= 1f)
                SetDestination(transform.position);
        } while (!isNewState);

        SetAgentStop(false);
    }

    public void Skill02TakeDamage()
    {
        float attackRadius = 3f;

        Collider[] colls = Physics.OverlapSphere(transform.position, attackRadius, enemyMask);
        RigidbodyController enemyRigidbody;
        CharacterStats enemyStats;

        for (int i = 0; i < colls.Length; i++)
        {
            enemyStats = colls[i].GetComponent<CharacterStats>();
            if (enemyStats != null)
            {
                combat.SkillAttack(enemyStats);

                //적의 생명력이 다한 경우 이후 연산을 처리하지 않음
                if (enemyStats.GetCurrentHealth() <= 0)
                    continue;
            }

            //RigidbodyController가 적용되어있는 오브젝트의 경우 넉백 적용
            enemyRigidbody = colls[i].GetComponent<RigidbodyController>();
            if (enemyRigidbody != null)
            {
                FSMUnit enemy = colls[i].GetComponent<FSMUnit>();
                if (enemy != null)
                {
                    enemy.SetKnockBack();

                    enemy.SetAgentStop(true);
                    enemy.SetAgentEnable(false);

                    enemyRigidbody.SetUseGravity(true);
                    Vector3 direction = (transform.position - enemyStats.transform.position).normalized;
                    enemyRigidbody.DirectionAddForce(-direction, 4f);

                    enemy.StartCoroutine("EndKnockBack");
                }
            }
        }
    }

    public void Skill02Sound()
    {
        audioSource.clip = SoundManager.instance.Skill02Sound();
        audioSource.Play();
    }

    //공격 지점을 바라보도록 설정
    void FaceTarget(Vector3 targetPosition)
    {
        if (agent.velocity == Vector3.zero)
            return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = lookRotation;
    }

    //애니메이션 클립에 연결되어 스킬애니메이션의 종료를 알림
    public void EndSkill()
    {
        useSkill = false;
        SetState(UnitState.Idle);
    }
}
