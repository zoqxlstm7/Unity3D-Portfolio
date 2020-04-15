using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Image icon;          // 아이템 아이콘
    public Image numImage;      // 아이템 갯수를 표현할 이미지
    public Item item;           // 획득한 아이템

    protected virtual void Start()
    {
        // 실행 시 갯수이미지 숨김 처리
        if(numImage != null)
            numImage.gameObject.SetActive(false);
    }

    // 획득 아이템 슬롯에 추가
    public void AddSlot(Item newItem)
    {
        item = newItem;
        icon.sprite = newItem.icon;
    }

    // 획득한 아이템 슬롯에 추가 및 갯수 초기화
    public void AddSlot(Item newItem, int num)
    {
        item = newItem;
        icon.sprite = newItem.icon;

        // 소모성 아이템인 경우
        if (item.isConsumable)
        {
            numImage.GetComponentInChildren<Text>().text = num.ToString();
            numImage.gameObject.SetActive(true);
        }
    }

    // 삭제 아이템 슬롯에서 제거
    public void RemoveSlot()
    {
        item = null;
        icon.sprite = null;

        // 갯수 텍스트 및 이미지 숨김 처리
        if(numImage != null)
        {
            numImage.GetComponentInChildren<Text>().text = "";
            numImage.gameObject.SetActive(false);
        }
    }

    // 아이템 사용시 버튼
    public virtual void OnUse()
    {
        if(item != null)
            Debug.Log(item.itemName + " Use.");
    }
}
