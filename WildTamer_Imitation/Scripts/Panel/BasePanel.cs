using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    #region Unity Methods
    private void Awake()
    {
        // 패널 등록
        PanelManager.RegistPanel(GetType(), this);
    }

    private void Start()
    {
        InitializePanel();
    }

    private void Update()
    {
        UpdatePanel();
    }

    private void OnDestroy()
    {
        DestroyPanel();
    }
    #endregion Unity Methods

    #region Other Methods
    // 패널 초기화 함수
    public virtual void InitializePanel() { }
    // 패널 업데이트 함수
    public virtual void UpdatePanel() { }

    /// <summary>
    /// 패널 오브젝트를 보여주는 함수
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 패널 오브젝트를 숨기는 함수
    /// </summary>
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 패널을 제거하는 함수
    /// </summary>
    public virtual void DestroyPanel()
    {
        // 패널 등록해제
        PanelManager.UnRegistPanel(GetType());
    }
    #endregion Other Methods
}
