using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSlot : Slot
{
    // 클릭시 장착 해제
    public override void OnUse()
    {
        base.OnUse();

        if(item != null)
        {
            // Equipment로 형 변환 후 장비 인덱스값을 구함
            Equipment equipment = (Equipment)item;
            int slotIndex = (int)equipment.equipmentSlot;

            // 해당 인덱스 장비 장착해제
            EquipmentManager.instance.Unequip(slotIndex);
        }
    }
}
