using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum BulletStyle : int
    {
        RIFLE = 0,
        SNIPE,
        SHOTGUN,
        NONE
    }

    const string MUZZLE_EFFECT_FILE_PATH = "Effect/MuzzleEffect";

    [SerializeField] protected BulletStyle bulletStyle;         // 탄알 정의
    public BulletStyle BulletType
    {
        get => bulletStyle;
    }
    [SerializeField] protected Transform firePoint;             // 총알 발사 위치
    [SerializeField] protected Transform emptyShellPoint;       // 탄피 배출 지점
    [SerializeField] int maxBulletCount;                        // 최대 탄알 장전 수
    [SerializeField] protected int remainBulletCount;           // 남은 탄알 수
    public int RemainBulletCount
    {
        get => remainBulletCount;
    }

    [SerializeField] protected int reloadableBulletCount; // 장전할 수 있는 남은 탄알 수
    public int ReloadableBulletCount
    {
        get => reloadableBulletCount;
        set => reloadableBulletCount = value;
    }

    [SerializeField] string gunName;                            // 총이름
    public string GunName
    {
        get => gunName;
    }

    [SerializeField] float muzzleVelocity;                      // 연사 속도
    public float MuzzleVelocity
    {
        get => muzzleVelocity;
    }

    [SerializeField] float reloadTime;                          // 재장전 시간
    public float ReloadTime
    {
        get => reloadTime;
    }

    [SerializeField] protected float rangeOfShot;               // 사정거리
    public float RangeOfShot
    {
        get => rangeOfShot;
    }

    [SerializeField] int damage;                                // 탄알 데미지
    public int Damage
    {
        get => damage;
    }

    protected float lastActionTime;                             // 기능을 수행한 마지막 시간
    bool isReload;                                              // 재장전 여부

    private void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        // 초반 탄약 장전
        remainBulletCount = maxBulletCount;

        /* 
         * 네트워크를 통해 인스턴트된 후의 포지션 설정
         */
        Transform parent = transform.parent;
        HandContainer handContainer = parent.GetChild(0).GetComponent<HandContainer>();
        transform.SetParent(handContainer.Parent);
        // 무기 포지션 설정
        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion(0, 0, 0, 0);

        // 발사 지점 설정
        firePoint = handContainer.Parent.Find("FirePoint");

        // 탄피 배출 지점 설정
        emptyShellPoint = handContainer.Parent.Find("EmptyShellPoint");
    }

    /// <summary>
    /// 공격 가능 여부 반환
    /// </summary>
    /// <returns></returns>
    public bool IsAttackable()
    {
        if (Time.time - lastActionTime > muzzleVelocity)
            return true;

        return false;
    }

    /// <summary>
    /// 발사 처리
    /// </summary>
    /// <param name="owner">발사한 객체 정보</param>
    public virtual void Fire(Actor owner)
    {
        Bullet newBullet = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BulletManager.Generate((int)bulletStyle, firePoint.position);

        if (newBullet != null)
        {
            if (owner.photonView.IsMine)
            {
                // 총 종류에 따른 사운드 재생
                GameManager.Instance.SoundManager.PlaySFX(bulletStyle.ToString());
            }
            else
            {
                // 다른 사람에 것인 경우 소리를 줄여서 출력
                GameManager.Instance.SoundManager.PlaySFX(bulletStyle.ToString(), SoundManager.OTHER_SFX_SOUND_VOLUME);
            }

            // 발사 이펙트 생성 함수 실행
            GenerateMuzzleEffect();

            newBullet.Fire(owner, firePoint.forward, damage, firePoint.position, rangeOfShot);
            // 총구 방향에 맞춰 총알 회전
            newBullet.transform.rotation = firePoint.rotation;
            // 남은 총알 감소
            remainBulletCount--;

            lastActionTime = Time.time;

            // 탄피생성 함수 실행
            GenerateEmptyShell((int)bulletStyle, owner);
        }
    }

    /// <summary>
    /// 탄피 생성 함수
    /// </summary>
    /// <param name="bulletIndex">생성할 탄알 종류</param>
    protected void GenerateEmptyShell(int bulletIndex, Actor owner)
    {
        // 탄피생성
        EmptyShell emptyShell = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EmptyShellManager.Generate(bulletIndex, emptyShellPoint.position);
        if (emptyShell != null)
        {
            if (owner.photonView.IsMine)
            {
                // 탄피 효과음 재생
                GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.EMPTY_SHELL_SOUND);
            }
            else
            {
                // 다른 사람에 것인 경우 소리를 줄여서 출력
                GameManager.Instance.SoundManager.PlaySFX(bulletStyle.ToString(), SoundManager.OTHER_SFX_SOUND_VOLUME);
            }

            // 총구 방향에 맞춰 탄피 회전
            emptyShell.transform.rotation = emptyShellPoint.transform.rotation;
            // 탄피 배출 처리 함수 실행
            emptyShell.AddForece(emptyShellPoint);
        }
    }

    /// <summary>
    /// 발사 이펙트 생성
    /// </summary>
    protected void GenerateMuzzleEffect()
    {
        // 발사 이펙트 표시
        GameObject muzzleEffect = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Generate(MUZZLE_EFFECT_FILE_PATH, firePoint.position);
        if (muzzleEffect)
            muzzleEffect.transform.rotation = firePoint.rotation;
    }

    /// <summary>
    /// 재장전해야 하는지 검사
    /// </summary>
    /// <returns>재장전 여부</returns>
    public bool CheckReload()
    {
        // 재장전해야 하는지 검사
        if(remainBulletCount <= 0 && reloadableBulletCount >= 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 재장전 트리거 실행
    /// </summary>
    public bool OnReload()
    {
        // 남아있는 총알이 있고 재장전해야되는 상황인지 검사
        if(remainBulletCount < maxBulletCount && !isReload && reloadableBulletCount > 0)
        {
            lastActionTime = Time.time;
            isReload = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 재장전 처리
    /// </summary>
    public bool Reload()
    {
        // 재장전 처리가 진행중이 아니라면 리턴
        if (!isReload)
            return false;

        // 리로드 UI 노출
        InGamePanel inGamePanel = PanelManager.GetPanel(typeof(InGamePanel)) as InGamePanel;
        inGamePanel.UpdateReloadBar(Time.time - lastActionTime, reloadTime);

        // 재장전 시간 경과 후
        if(Time.time - lastActionTime - Player.RELOAD_DELAY_TIME > reloadTime)
        {
            // 장전할 탄알이 남아있다면
            if(reloadableBulletCount > 0)
            {
                // 장전해야할 탄알 계산
                int reloadCount = maxBulletCount - remainBulletCount;

                // 장전해야되는 탄알 수가 남아있는 탄알 수보다 많은지 검사
                if (reloadCount > reloadableBulletCount)
                    reloadCount = reloadableBulletCount;

                // 총알 장전
                reloadableBulletCount -= reloadCount;
                remainBulletCount += reloadCount;

                lastActionTime = Time.time;
                isReload = false;

                return true;
            }
        }

        return false;
    }
}
