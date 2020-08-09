using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    #region Variables
    readonly string BOMB_EFFECT_FILE_PATH = "Prefabs/BombEffect";   // 폭탄 파일 경로
    readonly float DISPLAY_TIME = 3f;                               // 보여지는 시간

    SpriteRenderer spriteRenderer;                                  // 스프라이트 렌더러
    float alpha = 0f;                                               // 알파값

    int damage;                                                     // 데미지
    LayerMask targetMask;                                           // 타겟 레이어
    float hitRange;                                                 // 타격 범위

    bool isOperate = false;                                         // 작동 여부
    #endregion Variables

    #region Property
    public string FilePath { get; set; }                            // 파일 경로
    #endregion Property

    #region Unity Methods
    private void OnEnable()
    {
        // 변수 초기화
        alpha = 0f;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isOperate)
            return;

        // 보여지는 시간동안의 알파값 계산
        alpha = Mathf.Lerp(alpha, 1f, (Time.deltaTime * DISPLAY_TIME) / DISPLAY_TIME);
        // 알파값 수정
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;

        // 알파값이 다 채워졌다면 폭탄 데미지 처리
        if(alpha >= 0.9f)
        {
            isOperate = false;
            TakeDamageFromBomb();
        }
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 폭탄 객체 초기화 함수
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="targetMask">타겟 레이어</param>
    /// <param name="hitRange">타격 범위</param>
    public void InitBomb(int damage, LayerMask targetMask, float hitRange)
    {
        this.damage = damage;
        this.targetMask = targetMask;
        this.hitRange = hitRange;

        isOperate = true;
    }

    /// <summary>
    /// 폭탄데미지 처리 함수
    /// </summary>
    void TakeDamageFromBomb()
    {
        // 알파값 초기화
        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;

        InGameSceneManager inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();
        // 폭탄 이펙트 생성
        inGameSceneManager.effectManager.Generate(BOMB_EFFECT_FILE_PATH, transform.position);

        // 타격 범위내 존재하는 객체에 데미지를 입힘
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, hitRange, targetMask);
        if(colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].GetComponent<IDamageable>()?.TakeDamage(damage);
            }
        }

        // 제거
        inGameSceneManager.bombManager.Remove(FilePath, this);
    }
    #endregion Other Methods

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}
