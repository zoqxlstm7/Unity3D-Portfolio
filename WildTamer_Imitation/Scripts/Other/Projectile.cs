using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Projectile : MonoBehaviour
{
    #region Variables
    [SerializeField] float speed;           // 발사체 속도
    [SerializeField] float rangeDistance;   // 발사거리
    int damage;                             // 데미지

    Actor owner;                            // 소유자
    Vector3 dir;                            // 방향
    Vector3 firePos;                        // 발사위치

    bool isMove = false;                    // 움직임 여부

    CapsuleCollider2D capsuleCollider;      // 캡슐콜라이더
    #endregion Variables

    #region Property
    public string FilePath { get; set; }    // 파일 경로
    #endregion Property

    #region Unity Methods
    private void OnEnable()
    {
        // 콜라이더 활성화
        if(capsuleCollider != null)
            capsuleCollider.enabled = true;
    }

    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        if (!isMove)
            return;

        // 이동
        Vector3 moveVector = dir * speed * Time.deltaTime;
        transform.position += moveVector;

        CheckDestroyDistance();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌 객체가 Actor 클래스를 가지고 있다면 충돌체 검사
        Actor target = collision.GetComponent<Actor>();
        if(target != null)
        {
            OnCollisionProjectile(target);
        }
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 발사체 발사 함수
    /// </summary>
    /// <param name="owner">소유자</param>
    /// <param name="damage">데미지</param>
    /// <param name="dir">방향</param>
    public void Fire(Actor owner, int damage, Vector3 dir, Vector3 firePos)
    {
        this.owner = owner;
        this.damage = damage;
        this.dir = dir;
        this.firePos = firePos;

        // 방향 설정
        transform.right = dir;
        // 이동 시작
        isMove = true;
    }

    /// <summary>
    /// 충돌 검사 함수
    /// </summary>
    /// <param name="target">타겟</param>
    void OnCollisionProjectile(Actor target)
    {
        // Actor 클래스를 상속받은 객체 중 서로 다른 레이어를 가지고 있다면 데미지를 입힘
        if(owner.gameObject.layer != target.gameObject.layer)
        {
            // 콜라이더 비활성화
            capsuleCollider.enabled = false;

            target.GetComponent<IDamageable>()?.TakeDamage(damage);
            DestroyProjectile();
        }
    }

    /// <summary>
    /// 발사거리를 벗어났는지 검사하는 함수
    /// </summary>
    void CheckDestroyDistance()
    {
        float distance = Vector3.Distance(transform.position, firePos);
        // 발사거리를 벗어난경우 제거
        if (distance > rangeDistance)
            DestroyProjectile();
    }

    /// <summary>
    /// 발사체 제거 함수
    /// </summary>
    void DestroyProjectile()
    {
        isMove = false;
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().projectileManager.Remove(FilePath, this);
    }
    #endregion Other Methods
}
