using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : Slot, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    // 아이템 사용시 버튼
    public override void OnUse()
    {
        if (item == null)
            return;

        base.OnUse();
        item.Use();
    }

    // 드래그  시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item)
        {
            // 드래그 슬롯 초기화 및 드래그 포지션을 이동 처리
            DragSlot.instance.InitSlot(this, icon.sprite);
            DragSlot.instance.transform.position = eventData.position;

            DragSlot.instance.IsActivated(true);
        }
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 포지션으로 이동 처리
        DragSlot.instance.transform.position = eventData.position;
    }

    // 드래그 드랍
    public void OnDrop(PointerEventData eventData)
    {
        if(DragSlot.instance.dragSlot != null)
        {
            // 스위칭 함수 호출
            SwitchingItem();
        }
    }

    // 인벤토리 내 두 아이템 스위칭
    public void SwitchingItem()
    {
        if (item != null)
        {
            // 스위칭할 아이템의 인덱스를 구함
            int index = Inventory.instance.items.IndexOf(item);
            int dragIndex = Inventory.instance.items.IndexOf(DragSlot.instance.dragSlot.item);

            // 아이템 스위칭
            Inventory.instance.items[index] = DragSlot.instance.dragSlot.item;
            Inventory.instance.items[dragIndex] = item;

            // 인벤토리 UI 업데이트 전 스위치할 슬롯 초기화
            RemoveSlot();
            DragSlot.instance.dragSlot.RemoveSlot();

            // 인벤토리 UI 업데이트
            Inventory.instance.onItemChanged.Invoke();
        }
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 슬롯 숨김 처리 및 드래그 슬롯 초기화
        DragSlot.instance.IsActivated(false);
        DragSlot.instance.ClearSlot();
    }
}
