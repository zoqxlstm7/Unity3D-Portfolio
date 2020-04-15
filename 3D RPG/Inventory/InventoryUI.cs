using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform parent;            // 인벤토리 슬롯 부모객체
    Slot[] slots;                       // 인벤토리 슬롯을 담을 배열

    private void Start()
    {
        slots = parent.GetComponentsInChildren<Slot>();

        // 아이템 획득 시 인벤토리UI 업데이트 함수를 호출할 수 있도록 콜백함수 연결
        Inventory.instance.onItemChanged += UpdateUI;
        //게임 시작시 인벤토리 UI 숨김
        transform.gameObject.SetActive(false);
    }

    // 인벤토리 UI 업데이트
    public void UpdateUI()
    {
        // 인벤토리 슬롯만큼 반복
        for (int i = 0; i < slots.Length; i++)
        {
            // 인벤토리 슬롯 초기화
            slots[i].RemoveSlot();

            // 획득한 아이템만큼만 슬롯을 초기화하고 나머지 슬롯은 NULL로 초기화
            if (i < Inventory.instance.items.Count)
            {
                // 소모성 아이템인 경우 갯수도 같이 표현
                if(Inventory.instance.items[i].isConsumable)
                    slots[i].AddSlot(Inventory.instance.items[i], Inventory.instance.items[i].num);
                else
                    slots[i].AddSlot(Inventory.instance.items[i]);
            }
        }
    }
}
