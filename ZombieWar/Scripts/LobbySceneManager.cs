using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : BaseSceneManager
{
    // 최대값을 통한 비율 계산에 이용
    const float VELOCITY_SPEED_MAX_RATE = 2f;       
    const float RELOAD_TIME_MAX_RATE = 10f;
    const float BULLET_GUN_RANGE_RATE = 80f;
    const float DAMAGE_MAX_RATE = 30f;

    // Lerp 게이지 표현시 사용될 시간 비율
    const float LERP_GAGE_RATE = 3.0f;

    [SerializeField] Text gunNameText;              // 총이름
    [SerializeField] Image infoGageVelocitySpeed;   // 연사속도 정보
    [SerializeField] Image infoGageReloadTime;      // 재장전시간 정보
    [SerializeField] Image infoGageGunRange;        // 사거리 정보
    [SerializeField] Image infoGageDamage;          // 데미지 정보

    [SerializeField] int modelIndex;                // 선택된 캐릭터 모델 인덱스
    [SerializeField] int gunModelIndex;             // 선택된 총 모델 인덱스

    [SerializeField] GameObject[] models;           // 선택할 수 있는 캐릭터 모델 오브젝트
    [SerializeField] GameObject[] gunModels;        // 선택할 수 있는 총 모델 오브젝트

    int lastModelIndex;                             // 마지막에 선택된 캐릭터 모델 인덱스
    int lastGunIndex;                               // 마지막에 선택된 총 모델 인덱스

    bool isRoomConnected = false;                           // 게임시작 버튼이 눌렸는지 여부

    /// <summary>
    /// 초기화 작업
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        ResetModel();
        ResetGunModel();
    }

    /// <summary>
    /// 업데이트 처리
    /// </summary>
    public override void UpdateManager()
    {
        base.UpdateManager();

        ShowGunInfo();
    }

    /// <summary>
    /// 게임 시작 버튼
    /// </summary>
    public void OnStartButton()
    {
        // 두번 클릭 방지
        if (!isRoomConnected)
        {
            GameManager.Instance.NetworkManager.JoinRandomRoom(modelIndex, gunModelIndex);
            isRoomConnected = true;
        }
    }

    /// <summary>
    /// 캐릭터 모델 오브젝트 초기화
    /// </summary>
    void ResetModel()
    {
        for (int i = 0; i < models.Length; i++)
        {
            models[i].SetActive(false);
        }

        ActivatedSelectModel();
    }

    /// <summary>
    /// 화면에 표시되는 캐릭터 모델 오브젝트 처리
    /// </summary>
    void ActivatedSelectModel()
    {
        models[lastModelIndex].SetActive(false);
        models[modelIndex].SetActive(true);

        lastModelIndex = modelIndex;
    }

    /// <summary>
    /// 전시된 캐릭터 모델 오른쪽 이동 버튼
    /// </summary>
    public void OnModelSelectRight()
    {
        modelIndex++;
        // 인덱스 처음으로 초기화
        if (modelIndex == models.Length)
            modelIndex = 0;

        ActivatedSelectModel();
    }

    /// <summary>
    /// 전시된 캐릭터 모델 왼쪽 이동 버튼
    /// </summary>
    public void OnModelSelectLeft()
    {
        modelIndex--;
        // 인덱스 마지막으로 초기화
        if (modelIndex < 0)
            modelIndex = models.Length - 1;

        ActivatedSelectModel();
    }

    /// <summary>
    /// 총 모델 오브젝트 초기화
    /// </summary>
    void ResetGunModel()
    {
        for (int i = 0; i < gunModels.Length; i++)
        {
            gunModels[i].SetActive(false);
        }

        ResetInfoGage();

        ActivatedSelectGun();
    }

    /// <summary>
    /// 총 모델 게이지 정보 초기화
    /// </summary>
    void ResetInfoGage()
    {
        infoGageVelocitySpeed.fillAmount = 0f;
        infoGageReloadTime.fillAmount = 0f;
        infoGageGunRange.fillAmount = 0f;
        infoGageDamage.fillAmount = 0f;
    }

    /// <summary>
    /// 화면에 표시되는 총 모델 오브젝트 처리
    /// </summary>
    void ActivatedSelectGun()
    {
        gunModels[lastGunIndex].SetActive(false);
        gunModels[gunModelIndex].SetActive(true);

        ResetInfoGage();

        lastGunIndex = gunModelIndex;
    }

    /// <summary>
    /// 총 스탯 정보 표시
    /// </summary>
    void ShowGunInfo()
    {
        // 이름 표시
        Gun selectGun = GameManager.Instance.GunModels[gunModelIndex].GetComponent<Gun>();
        gunNameText.text = selectGun.GunName;

        // 비율에 따른 게이지 표시
        infoGageVelocitySpeed.fillAmount = Mathf.Lerp(infoGageVelocitySpeed.fillAmount, 1f - (selectGun.MuzzleVelocity / VELOCITY_SPEED_MAX_RATE), Time.deltaTime * LERP_GAGE_RATE);
        infoGageReloadTime.fillAmount = Mathf.Lerp(infoGageReloadTime.fillAmount, 1f - (selectGun.ReloadTime / RELOAD_TIME_MAX_RATE), Time.deltaTime * LERP_GAGE_RATE);
        infoGageGunRange.fillAmount = Mathf.Lerp(infoGageGunRange.fillAmount, selectGun.RangeOfShot / BULLET_GUN_RANGE_RATE, Time.deltaTime * LERP_GAGE_RATE);
        infoGageDamage.fillAmount = Mathf.Lerp(infoGageDamage.fillAmount, selectGun.Damage / DAMAGE_MAX_RATE, Time.deltaTime * LERP_GAGE_RATE);
    }

    /// <summary>
    /// 전시된 총 모델 오른쪽 이동 버튼
    /// </summary>
    public void OnGunSelectRight()
    {
        gunModelIndex++;
        // 인덱스 처음으로 초기화
        if (gunModelIndex == gunModels.Length)
            gunModelIndex = 0;

        ActivatedSelectGun();
    }

    /// <summary>
    /// 전시된 총 모델 왼쪽 이동 버튼
    /// </summary>
    public void OnGunSelectLeft()
    {
        gunModelIndex--;
        // 인덱스 마지막으로 초기화
        if (gunModelIndex < 0)
            gunModelIndex = gunModels.Length - 1;

        ActivatedSelectGun();
    }
}
