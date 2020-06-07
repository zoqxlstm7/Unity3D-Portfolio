using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// 무기 상태
/// </summary>
public enum WeaponStyle
{
    WEAPON,
    MELEE,
    THROW
}

public class Player : Actor, IPunInstantiateMagicCallback, IOnEventCallback, IPunObservable
{
    const string RUN_ANIMATION = "Run";         // 달리기 애니메이션 이름
    const string SHOOT_ANIMATION = "Shoot";     // 발사 애니메이션 이름
    const string RELOAD_ANIMATION = "Reload";   // 재장전 애니메이션 이름
    const string THROW_ANIMATION = "Throw";     // 던지기 애니메이션 이름

    public const float RELOAD_DELAY_TIME = 1f;  // 리로드 후 딜레이 시간
    const float THROW_DELAY_TIME = 0.5f;        // 던진 후 딜레이 시간

    const int RIFLE_GET_VALUE = 15;             // 라이플 탄알 기본 획득 수
    const int SNIPE_GET_VALUE = 5;              // 스나이트 탄알 기본 획득 수
    const int SHOTGUN_GET_VALUE = 5;            // 샷건 탄알 기본 획득 수
    const int THROW_ITEM_GET_VALUE = 1;         // 던질 아이템 기본 획득 수

    const byte SELECT_MODEL_EVENT = 0;          // 모델 선택 Raise이벤트 코드
    int selectModelIndex = -1;                  // 선택된 모델 인덱스
    int selectGunModelIndex = -1;               // 선택된 건 모델 인덱스
    int throwItemIndex;                         // 던질 아이템 인덱스

    [SerializeField] MotionState state;
    public MotionState State                    // 상태 객체 반환
    {
        get => state;
    }
    [SerializeField] WeaponStyle weaponState;
    public WeaponStyle WeaponState              // 무기상태 객체 반환
    {
        get => weaponState;
    }

    [SerializeField] Gun startGun;              // 시작 총
    public Gun StartGun
    {
        get => startGun;
    }

    [SerializeField] string nickName;           // 설정 닉네임
    public string NickName
    {
        get => nickName;
    }

    [SerializeField] int grenadeCount;          // 보유중인 수류탄 갯수
    public int GrenadeCount
    {
        get => grenadeCount;
    }

    [SerializeField] int healPackCount;         // 보유중인 힐팩 갯수
    public int HealPackCount
    {
        get => healPackCount;
    }
    
    public int ReloadableBulletCount            // 장전할 수 있는 총알 갯수
    {
        get => startGun.ReloadableBulletCount;
        set => startGun.ReloadableBulletCount = value;
    }

    [SerializeField] Avatar avatar;             // 애니메이션 적용에 쓰일 아바타    

    protected override void InitializeActor()
    {
        base.InitializeActor();
    }

    protected override void UpdateActor()
    {
        // 게임 오버시 리턴
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().IsGameOver)
        {
            Animator.PlayAnimator(RUN_ANIMATION, false);
            Animator.PlayAnimator(SHOOT_ANIMATION, false);
            Animator.PlayAnimator(RELOAD_ANIMATION, false);
            return;
        }

        // 보스연출시 리턴
        BossPanel bossPanel = PanelManager.GetPanel(typeof(BossPanel)) as BossPanel;
        if (bossPanel.IsCinematic)
            return;

        // 포톤뷰가 내것인거만 실행
        if (photonView.IsMine)
        {
            base.UpdateActor();

            // 재장전이 완료되었는지 확인
            if (startGun.Reload())
            {
                ReloadComplete();
            }
        }
    }

    /// <summary>
    /// 움직임 처리
    /// </summary>
    /// <param name="v">수직 입력값</param>
    /// <param name="h">수평 입력값</param>
    public void UpdateMove(float v, float h)
    {
        // none 상태가 아니면 리턴
        if (state != MotionState.NONE)
        {
            //Animator.PlayAnimator(RUN_ANIMATION, false);
            return;
        }

        // 뛰는 애니메이션 처리
        if (v == 0 && h == 0)
            Animator.PlayAnimator(RUN_ANIMATION, false);
        else
            Animator.PlayAnimator(RUN_ANIMATION, true);

        // 카메라 방향에 따른 플레이어 이동 처리
        Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 dir = camForward * v + Camera.main.transform.right * h;
        transform.position += dir.normalized * speed * Time.deltaTime;

        // 플레이어 회전 처리
        if (dir.magnitude > 1f) dir.Normalize();
        dir = transform.InverseTransformDirection(dir);
        float turnAmount = Mathf.Atan2(dir.x, dir.z);
        transform.Rotate(0, turnAmount * 500f * Time.deltaTime, 0);
    }

    /// <summary>
    /// 기본 무기(총을 든 상태) 상태로 변경
    /// </summary>
    public void SetBackBaseWeaponState()
    {
        weaponState = WeaponStyle.WEAPON;

        ThrowManager throwManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().ThrowManager;
        // 던지는 정보 설정
        throwManager.SetThrowInfo(ThrowItemType.NONE);
        throwManager.SetActivatedThrowImage(false);
    }

    /// <summary>
    /// 총알 발사 처리
    /// </summary>
    public void Fire()
    {
        // 재장전 상태일 경우 리턴
        if (state == MotionState.RELOAD)
            return;

        // 재장전이 필요한 경우한 경우 재장전 실행
        if (startGun.CheckReload())
        {
            Animator.PlayAnimator(RUN_ANIMATION, false);

            Reload();
            return;
        }

        // 공격 가능 시간이 아니라면 리턴
        if (!startGun.IsAttackable())
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            // 발사 RPC 실행
            photonView.RPC("FireRPC", RpcTarget.All, hit.point);
        }
        
    }

    /// <summary>
    /// 발사 처리 RPC
    /// </summary>
    /// <param name="point">발사 지점</param>
    [PunRPC]
    void FireRPC(Vector3 point)
    {
        // 해당 지점을 바라보는 처리
        TargetToFace(point);

        // 발사 애니메이션 처리
        Animator.PlayAnimator(SHOOT_ANIMATION, true);

        // 총알 발사
        state = MotionState.SHOOT;
        startGun.Fire(this);
    }

    /// <summary>
    /// 타겟지점을 바라보도록 처리하는 함수
    /// </summary>
    /// <param name="targetPos">바라볼 지점</param>
    void TargetToFace(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = lookRotation;
    }

    /// <summary>
    /// 총알 발사를 멈췄을 때의 처리
    /// </summary>
    public void StopFire()
    {
        // 재장전 상태일 경우 리턴
        if (state == MotionState.RELOAD)
            return;

        // 발사 애니메이션 처리
        Animator.PlayAnimator(SHOOT_ANIMATION, false);

        state = MotionState.NONE;
    }

    /// <summary>
    /// 재장전 시작
    /// </summary>
    public void Reload()
    {
        // 재장전 상태일 시 리턴
        if (state == MotionState.RELOAD)
            return;

        // 총 장작 상태가 아닌 경우 리턴
        if (weaponState != WeaponStyle.WEAPON)
            return;

        StopFire();
        if (startGun.OnReload())
        {
            // 리로드 시 리로드 UI 노출
            (PanelManager.GetPanel(typeof(InGamePanel)) as InGamePanel).ActivatedReloadBar();

            Animator.PlayAnimator(RELOAD_ANIMATION, true);

            state = MotionState.RELOAD;
        }
    }

    /// <summary>
    /// 재장전 완료
    /// </summary>
    void ReloadComplete()
    {
        Animator.PlayAnimator(RELOAD_ANIMATION, false);

        // 로컬리 true인 것만 재장전 효과음 재생
        if (photonView.IsMine)
        {
            // 재장전 효과음 재생
            GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.RELOAD_SOUND);
        }

        // 딜레이 처리 코루틴 시작
        StartCoroutine(ReloadDelay());
    }

    /// <summary>
    /// 재장전 완료 후 딜레이 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator ReloadDelay()
    {
        yield return new WaitForSeconds(RELOAD_DELAY_TIME);
        state = MotionState.NONE;

        // 리로드 완료 후 리로드 UI 숨김 처리
        (PanelManager.GetPanel(typeof(InGamePanel)) as InGamePanel).ActivatedReloadBar();
    }

    /// <summary>
    /// 던지는 상태로 전환 및 던질 아이템 타입 셋팅
    /// </summary>
    /// <param name="throwItemType">던질 아이템 타입</param>
    public void SetThrowItem(ThrowItemType throwItemType)
    {
        // 재장전 상태일 시 리턴
        if (state == MotionState.RELOAD)
            return;

        // 던지는 중일 때 리턴
        if (state == MotionState.Throw)
            return;

        // 사용할 아이템이 남아있는지 검사
        switch (throwItemType)
        {
            case ThrowItemType.GRENADE:
                if (grenadeCount <= 0)
                    return;
                break;
            case ThrowItemType.HEALPACK:
                if (healPackCount <= 0)
                    return;
                break;
        }

        // 던지기 상태로 변경
        weaponState = WeaponStyle.THROW;

        ThrowManager throwManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().ThrowManager;
        // 던지는 정보 설정
        throwManager.SetThrowInfo(throwItemType);
        throwManager.SetActivatedThrowImage(true);

        // 던질 아이템 인덱스 설정
        throwItemIndex = (int)throwItemType;
    }

    /// <summary>
    /// 던지기 처리
    /// </summary>
    public void Throw()
    {
        // 던지고 있는 중인 경우 리턴
        if (state == MotionState.Throw)
            return;

        // 사용하는 아이템 수량 감소
        switch ((ThrowItemType)throwItemIndex)
        {
            case ThrowItemType.GRENADE:
                grenadeCount--;
                break;
            case ThrowItemType.HEALPACK:
                healPackCount--;
                break;
        }

        // 마우스 지점으로 이동 벡터 생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            photonView.RPC("ThrowRPC", RpcTarget.All, hit.point);
        }
    }

    [PunRPC]
    void ThrowRPC(Vector3 point)
    {
        // 해당 지점을 바라보는 처리
        TargetToFace(point);

        // 발사 애니메이션 처리
        Animator.PlayAnimator(THROW_ANIMATION);

        state = MotionState.Throw;
        // 딜레이 처리 코루틴 시작
        StartCoroutine(ThrowDelay(point));
    }

    /// <summary>
    /// 던진 후 딜레이 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator ThrowDelay(Vector3 point)
    {
        ThrowManager throwManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().ThrowManager;
        throwManager.SetActivatedThrowImage(false);

        yield return new WaitForSeconds(THROW_DELAY_TIME);

        // Throw 함수 실행 및 정보 초기화
        // 던지기 실행
        throwManager.Throw(point, transform.position, throwItemIndex, this);
        throwManager.SetThrowInfo(ThrowItemType.NONE);

        yield return new WaitForSeconds(THROW_DELAY_TIME);
        state = MotionState.NONE;
        // 총 사용 상태로 무기 상태 변경
        weaponState = WeaponStyle.WEAPON;
    }

    /// <summary>
    /// 드랍될 아이템 처리
    /// </summary>
    /// <param name="maxValue">최대 확률</param>
    /// <param name="itemDropRate1">아이템1 드랍 확률</param>
    /// <param name="itemDropRate2">아이템2 드랍 확률</param>
    /// <param name="itemDropRate3">아이템3 드랍 확률</param>
    public void DropItem(int maxValue, int itemDropRate1, int itemDropRate2, int itemDropRate3)
    {
        int pick = Random.Range(0, maxValue + 1);

        // 비율에 따라 드랍될 아이템 선택 후 아이템 획득 함수 실행
        if (pick <= itemDropRate1)
        {
            TakeItem(AcquireType.BULLET);
        }
        else if (pick > itemDropRate1 && pick <= itemDropRate1 + itemDropRate2)
        {
            TakeItem(AcquireType.GRENADE);
        }
        else
        {
            TakeItem(AcquireType.HEAL_PACK);
        }
    }

    /// <summary>
    /// 아이템 획득 처리
    /// </summary>
    /// <param name="infoType">획득할 아이템 타입</param>
    public void TakeItem(AcquireType infoType)
    {
        int value = 0;

        // 아이템 타입에 따라 획득량 증가
        switch (infoType)
        {
            case AcquireType.BULLET:
                // 불렛 타입에 따른 총알 수 증가량 변경
                switch (startGun.BulletType)
                {
                    case Gun.BulletStyle.RIFLE:
                        value = RIFLE_GET_VALUE;
                        // 수량 증가 RPC  실행
                        photonView.RPC("GetReloadableBullet", RpcTarget.All, value, (int)infoType);
                        break;
                    case Gun.BulletStyle.SNIPE:
                        value = SNIPE_GET_VALUE;
                        // 수량 증가 RPC  실행
                        photonView.RPC("GetReloadableBullet", RpcTarget.All, value, (int)infoType);
                        break;
                    case Gun.BulletStyle.SHOTGUN:
                        value = SHOTGUN_GET_VALUE;
                        // 수량 증가 RPC  실행
                        photonView.RPC("GetReloadableBullet", RpcTarget.All, value, (int)infoType);
                        break;
                }
                break;
            case AcquireType.GRENADE:
                value = THROW_ITEM_GET_VALUE;
                // 수량 증가 RPC  실행
                photonView.RPC("GetGrenade", RpcTarget.All, value, (int)infoType);
                break;
            case AcquireType.HEAL_PACK:
                value = THROW_ITEM_GET_VALUE;
                // 수량 증가 RPC  실행
                photonView.RPC("GetHealPack", RpcTarget.All, value, (int)infoType);
                break;
        }  
    }

    /// <summary>
    /// 탄알을 얻었을 때의 RPC 실행
    /// </summary>
    /// <param name="value">증가될 값</param>
    /// <param name="type">아이템 타입</param>
    [PunRPC]
    void GetReloadableBullet(int value, int type)
    {
        ReloadableBulletCount += value;
        // 획득 UI 노출
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().AcquireUIManager.ActivatedInfoUI(value, this, (AcquireType)type);
    }

    /// <summary>
    /// 수류탄을 얻었을 때의 RPC 실행
    /// </summary>
    /// <param name="value">증가될 값</param>
    /// <param name="type">아이템 타입</param>
    [PunRPC]
    void GetGrenade(int value, int type)
    {
        grenadeCount += value;
        // 획득 UI 노출
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().AcquireUIManager.ActivatedInfoUI(value, this, (AcquireType)type);
    }

    /// <summary>
    /// 힐팩을 얻었을 때의 RPC 실행
    /// </summary>
    /// <param name="value">증가될 값</param>
    /// <param name="type">아이템 타입</param>
    [PunRPC]
    void GetHealPack(int value, int type)
    {
        healPackCount += value;
        // 획득 UI 노출
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().AcquireUIManager.ActivatedInfoUI(value, this, (AcquireType)type);
    }

    protected override void OnDead()
    {
        //Animator.PlayAnimator(RUN_ANIMATION, false);
        //Animator.PlayAnimator(SHOOT_ANIMATION, false);
        //Animator.PlayAnimator(RELOAD_ANIMATION, false);

        base.OnDead();

        // 사망 시 콜라이더 비활성 RPC 실행
        photonView.RPC("SetColliderEnabled", RpcTarget.AllBuffered);
    }

    /// <summary>
    /// 콜라이더 비활성 RPC
    /// </summary>
    [PunRPC]
    void SetColliderEnabled()
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<CapsuleCollider>().enabled = false;
    }

    #region ===Raise 이벤트 및 생성 처리===
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// Raise 이벤트 처리
    /// </summary>
    /// <param name="photonEvent"></param>
    public void OnEvent(EventData photonEvent)
    {
        // 이벤트 코드
        byte eventCode = photonEvent.Code;

        // 모델 동기화 이벤트인지 확인
        if (eventCode == SELECT_MODEL_EVENT)
        {
            Debug.Log("OnEvent");

            object[] data = (object[])photonEvent.CustomData;
            // 포톤뷰 아이디가 같은지 확인
            if (photonView.ViewID == (int)data[0])
            {
                // 캐릭터 모델 인덱스
                int modelIndex = (int)data[1];
                selectModelIndex = modelIndex;
                // 총 모델 인덱스
                int gunModelIndex = (int)data[2];
                selectGunModelIndex = gunModelIndex;

                Debug.Log("Event Call: " + selectModelIndex);
                Debug.Log("Event Call: " + selectGunModelIndex);
            }
        }
    }

    /// <summary>
    /// 포톤에 의해 객체가 생성될 때 호출되는 콜백함수
    /// </summary>
    /// <param name="info"></param>
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate");
        PhotonNetwork.AddCallbackTarget(this);
        if (info.photonView.IsMine)
        {
            // 인게임 매니저 초기화 셋팅
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero = this;

            // 카메라 설정
            FindObjectOfType<FollowCamera>().Target = transform;

            // 닉네임 설정
            nickName = GameManager.Instance.NetworkManager.NickName;
            (PanelManager.GetPanel(typeof(InGamePanel)) as InGamePanel).SetNickName(nickName);

            // 입장 메세지 출력
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().ChatManager.SendMsg("님이 입장하셨습니다.", nickName);

            // 소모품 초기값 설정
            grenadeCount = 1;
            healPackCount = 1;

            // 선택된 총, 캐릭터 모델
            selectModelIndex = GameManager.Instance.ModelIndex;
            selectGunModelIndex = GameManager.Instance.GunModelIndex;

            // RaiseEvent 매개변수 정의
            byte eventCode = SELECT_MODEL_EVENT;
            object[] content = { info.photonView.ViewID, selectModelIndex, selectGunModelIndex };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };

            // 이벤트 호출
            PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, sendOptions);
            // RPC 호출
            photonView.RPC("GenerateModelRPC", RpcTarget.AllBuffered);
        }

        // 플레이어 정보 등록
        PlayersInfoPanel playersInfoPanel = PanelManager.GetPanel(typeof(PlayersInfoPanel)) as PlayersInfoPanel;
        playersInfoPanel.ResistPlayer(this);
    }

    /// <summary>
    /// 총, 캐릭터 모델 오브젝트 생성
    /// </summary>
    [PunRPC]
    void GenerateModelRPC()
    {
        Debug.Log("GenerateModelRPC" + photonView.ViewID);
        Debug.Log("model: " + selectModelIndex);
        Debug.Log("gun: " + selectGunModelIndex);
        // 캐릭터 모델 오브젝트 생성
        Instantiate(GameManager.Instance.Models[selectModelIndex], transform);
        // 애니메이션 동기화를 위한 아바타 설정
        Animator.GetAnimator().avatar = avatar;
        // 총 모델 오브젝트 생성 후 참조
        startGun = Instantiate(GameManager.Instance.GunModels[selectGunModelIndex], transform).GetComponent<Gun>();
    }

    /// <summary>
    /// 변수 동기화
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
            // 던질 아이템 인덱스 동기화
            stream.SendNext(throwItemIndex);
            // 닉네임 동기화
            stream.SendNext(nickName);
            // 가한 피해량 동기화
            stream.SendNext(amountOfDamage);
        }
        else
        {
            throwItemIndex = (int)stream.ReceiveNext();
            nickName = (string)stream.ReceiveNext();
            amountOfDamage = (int)stream.ReceiveNext();
        }
    }
    #endregion
}
