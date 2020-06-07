using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableQuickSlot : Slot, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    protected override void Start()
    {
        base.Start();
        // 갯수 UI 함수 콜백 연결 처리
        Inventory.instance.onItemChanged += NumTextUpdate;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 소모성 아이템 사용
            OnUse();
        }
    }

    // 아이템 사용시 버튼
    public override void OnUse()
    {
        if (item == null)
            return;

        // 아이템 사용
        if (item.num > 0)
        {
            base.OnUse();
            item.Use();
        }
    }

    // 아이템 사용시 갯수 UI 업데이트
    void NumTextUpdate()
    {
        if(item != null)
        {
            if(numImage != null)
                numImage.GetComponentInChildren<Text>().text = item.num.ToString();

            // 아이템을 모두 사용한 경우 슬롯을 비움
            if (item.num == 0)
            {
                RemoveSlot();
            }
        }   
    }

    // 드래그 시작 이벤트 처리
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            // 드래그 슬롯 초기화 및 활성화
            DragSlot.instance.InitSlot(this, icon.sprite);
            DragSlot.instance.transform.position = eventData.position;

            DragSlot.instance.IsActivated(true);
        }
    }

    // 드래그 중 이벤트 처리
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            // 마우스 포지션으로 드래그슬롯 위치 변경
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // 드래그 드랍 이벤트 처리
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            // 퀵슬롯이 비어있고 소모성 아이템인 경우 아이템과 아이콘 할당
            if (item == null && DragSlot.instance.dragSlot.item.isConsumable)
            {
                // 인벤토리 내의 가장 먼저 등록된 같은 소모품 아이템을 찾아 퀵슬롯에 등록
                for (int i = 0; i < Inventory.instance.items.Count; i++)
                {
                    if(DragSlot.instance.dragSlot.item.itemName == Inventory.instance.items[i].itemName)
                    {
                        item = Inventory.instance.items[i];
                        icon.sprite = Inventory.instance.items[i].icon;
                        break;
                    }
                }
                //item = DragSlot.instance.dragSlot.item;
                //icon.sprite = DragSlot.instance.dragSlot.icon.sprite;

                // 아이템 갯수 할당 및 갯수 UI 노출
                if (numImage != null)
                {
                    numImage.GetComponentInChildren<Text>().text = item.num.ToString();
                    numImage.gameObject.SetActive(true);
                }
            }
        }
    }

    // 드래그 끝 이벤트 처리
    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 슬롯 숨김 처리 및 드래그 슬롯 초기화
        DragSlot.instance.IsActivated(false);
        DragSlot.instance.ClearSlot();

        // 퀵슬롯 초기화
        if (item != null)
        {
            item = null;
            icon.sprite = null;
            numImage.gameObject.SetActive(false);
        }
    }
}
