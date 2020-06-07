using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    #region Singleton
    public static DragSlot instance;
    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }
    #endregion

    public Slot dragSlot;       // 드래그 되는 슬롯을 담을 객체
    public Image icon;          // 드래그 되는 아이콘

    private void Start()
    {
        // 시작 시 드래그슬롯 숨김 처리
        IsActivated(false);
    }

    // 드래그 슬롯 초기화
    public void InitSlot(Slot slot, Sprite _icon)
    {
        dragSlot = slot;
        icon.sprite = _icon;
    }

    // 드래그 슬롯 초기화
    public void ClearSlot()
    {
        dragSlot = null;
        icon.sprite = null;
    }

    // 드래그 슬롯 이미지 활성/비활성
    public void IsActivated(bool isActivated)
    {
        icon.transform.parent.gameObject.SetActive(isActivated);
    }
}
