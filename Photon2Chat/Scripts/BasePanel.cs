using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    private void Awake()
    {
        Initialize();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdatePanel();
    }

    /// <summary>
    /// 초기화 항목 진행
    /// </summary>
    protected virtual void Initialize()
    {
        // 패널 등록
        PanelManager.RegistPanel(GetType(), this);
    }

    /// <summary>
    /// 패널 내 업데이트 처리
    /// </summary>
    protected virtual void UpdatePanel()
    {

    }

    /// <summary>
    /// 패널 노출
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 패널 숨김
    /// </summary>
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
