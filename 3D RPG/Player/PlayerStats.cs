using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    protected override void Start()
    {
        base.Start();

        // 장비 아이템 탈장착 콜백함수 연결
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChaned;
    }

    // 장비 변경 시 스탯 변환
    void OnEquipmentChaned(Equipment newItem, Equipment oldItem)
    {
        // 새로운 장비 아이템을 장착한 경우 아이템 스탯 추가
        if(newItem != null)
        {
            damage.AddModifier(newItem.damageModifier);
            armor.AddModifier(newItem.armorModifier);
        }

        // 기존 아이템을 탈착한 경우 아이템 스탯 삭제
        if(oldItem != null)
        {
            damage.RemoveModifier(oldItem.damageModifier);
            armor.RemoveModifier(oldItem.armorModifier);
        }
    }
}
