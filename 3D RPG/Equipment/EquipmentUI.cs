using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public Transform parent;            // 장비 슬롯 부모객체
    Slot[] slots;                       // 장비 슬롯을 담을 배열

    private void Start()
    {
        slots = parent.GetComponentsInChildren<Slot>();

        // 장비 탈/장착 시 장비UI 업데이트 함수를 호출할 수 있도록 콜백함수 연결
        EquipmentManager.instance.onEquipmentChanged += UpdateUI;
    }

    // 장비 UI 업데이트
    public void UpdateUI(Equipment newItem, Equipment oldItem)
    {
        // 장착할 장비가 있다면 슬롯에 추가
        if(newItem != null)
        {
            int slotIndex = (int)newItem.equipmentSlot;
            slots[slotIndex].AddSlot(newItem);
        }
        else// 장착할 장비가 없다면 슬롯에서 제거
        {
            int slotIndex = (int)oldItem.equipmentSlot;
            slots[slotIndex].RemoveSlot();
        }
    }
}
