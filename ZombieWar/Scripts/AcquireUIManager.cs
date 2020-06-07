﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 획득 UI 타입
/// </summary>
public enum AcquireType
{
    BULLET,
    GRENADE,
    HEAL_PACK,
    HEAL
}

public class AcquireUIManager : MonoBehaviour
{
    // 획득 UI 객체
    [SerializeField] AcquireUI[] acquireUIs;
    // 획득 UI 객체에 표시할 스트라이트
    [SerializeField] Sprite[] sprites;
    
    int uiIndex = 0;        // 획득 UI 객체를 관리할 인덱스

    private void Start()
    {
        // 획득 UI 객체 숨김 처리
        for (int i = 0; i < acquireUIs.Length; i++)
        {
            acquireUIs[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 획득 UI 초기화
    /// </summary>
    /// <param name="position">표시될 지점</param>
    /// <param name="acquireType">표시될 획득 정보 타입</param>
    /// <returns></returns>
    public AcquireUI Generate(Vector3 position, AcquireType acquireType)
    {
        // 인덱스를 범어났을 시 초기화
        if(uiIndex >= acquireUIs.Length)
        {
            uiIndex = 0;
        }

        AcquireUI acquireUI = acquireUIs[uiIndex];

        // 위치, 인덱스, 스프라이트 셋팅
        acquireUI.transform.position = position;
        acquireUI.UiIndex = uiIndex;
        acquireUI.Icon = sprites[(int)acquireType];

        // 획득 UI 화면에 표시
        acquireUI.gameObject.SetActive(true);

        // 인덱스 증가
        uiIndex++;

        return acquireUI;
    }

    /// <summary>
    /// 획득 UI 숨김
    /// </summary>
    /// <param name="uiIndex">숨길 인덱스</param>
    public void Remove(int uiIndex)
    {
        acquireUIs[uiIndex].gameObject.SetActive(false);
    }

    /// <summary>
    /// 획득 UI 활성화
    /// </summary>
    /// <param name="value"></param>
    public void ActivatedInfoUI(int value, Actor target, AcquireType acquireType)
    {
        // 로컬리 트루인 객체만 실행
        if (target.photonView.IsMine)
        {
            // 획득 UI 노출
            AcquireUI acquireUI = Generate(target.transform.position, acquireType);
            if (acquireUI != null)
                acquireUI.SetInfo(value, target.transform);
        }
    }
}
