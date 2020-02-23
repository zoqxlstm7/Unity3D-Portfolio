using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMUnit : FSMBase
{
    AudioSource audioSource;

    CharacterUI characterUI;                     //데미지, 체력바 등의 유닛의 UI를 처리하는 클래스
    CharacterCombat combat;
    CharacterStats myStats;
    RigidbodyController rigidbodyController;    //리지바디 제어 클래스

    public GameObject projectile;               //유닛의 발사체
    public Transform spawnPoint;                //발사체 생성 지점

    //public float lookRadius = 10f;
    public float attackRadius = 2f;             //공격 범위
    public LayerMask enemyMask;

    Transform shortTarget = null;               //가장 가까운 대상을 담을 변수
    public bool isknockBack = false;            //현재 넉백 상태인지의 여부

    float findTime;

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();

        //유닛에 따른 어택사운드 설정
        if (transform.CompareTag("enemy"))
        {
            audioSource.clip = SoundManager.instance.EnemyAttackSound();
            audioSource.volume = 0.3f;
        }
        else if (projectile == null)
            audioSource.clip = SoundManager.instance.AttackSound();
        else
            audioSource.clip = SoundManager.instance.BowAttackSound();

        characterUI = GetComponent<CharacterUI>();
        myStats = GetComponent<CharacterStats>();
        rigidbodyController = GetComponent<RigidbodyController>();

        combat = GetComponent<CharacterCombat>();
        combat.OnAttack += OnAttack;

        agent.stoppingDistance = attackRadius;

        findTime = Time.time;
    }

    private void Update()
    {
        if (myStats.isDie)
        {
            SetState(UnitState.Die);
            return;
        }

        if (Time.time - findTime >= 0.25f)
        {
            if (isknockBack)
                return;

            Collider[] colliders = Physics.OverlapSphere(
                transform.position, Mathf.Infinity, enemyMask);

            float shortDistance = Mathf.Infinity;

            for (int i = 0; i < colliders.Length; i++)
            {
                float distance = Vector3.SqrMagnitude(transform.position - colliders[i].transform.position);
                if (shortDistance > Mathf.Pow(distance, 2))
                {
                    shortTarget = colliders[i].transform;
                    shortDistance = distance;

                    agent.SetDestination(shortTarget.position);
                    SetState(UnitState.Run);
                }
            }

            //가장 가까운 대상이 공격범위에 들어오면 공격
            if(shortDistance <= Mathf.Pow(agent.stoppingDistance, 2))
            {
                combat.Attack(shortTarget.GetComponent<CharacterStats>());
            }

            findTime = Time.time;
        }
    }

    void OnAttack()
    {
        if (state != UnitState.Attack)
            SetState(UnitState.Attack);
    }

    protected override IEnumerator Attack()
    {
        animator.SetTrigger("Attack01");
        if(projectile == null)
            audioSource.Play(); 

        do
        {
            yield return null;
            if (isNewState)
                break;

            FaceTarget();
        } while (!isNewState);
    }

    IEnumerator Die()
    {
        animator.SetTrigger("Die");
        shortTarget = null;


        yield return new WaitForSeconds(1f);

        characterUI.RemoveAllDamageText();
        characterUI.ActivatedHpBar(false);
        transform.gameObject.SetActive(false);
    }

    //공격 대상을 바라보도록 설정
    void FaceTarget()
    {
        Vector3 direction = (shortTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void LaunchProjectile()
    {
        //발사체가 있는 경우 발사체 발사
        if (projectile != null)
        {
            Instantiate(projectile, transform);
            audioSource.Play();
        }
    }

    //넉백 설정
    public void SetKnockBack()
    {
        isknockBack = true;
        SetState(UnitState.Idle);
    }

    IEnumerator EndKnockBack()
    {
        yield return new WaitForSeconds(1f);

        SetAgentEnable(true);
        SetAgentStop(false);

        rigidbodyController.SetUseGravity(false);
        isknockBack = false;

        StopCoroutine("EndKnockBack");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
