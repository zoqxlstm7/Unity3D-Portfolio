using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public SkinnedMeshRenderer mesh;        // 장비의 메쉬를 저장할 변수
    public EquipmentSlot equipmentSlot;     // 장비슬롯을 저장할 enum변수

    public int damageModifier;            // 해당 장비의 데미지값
    public int armorModifier;             // 해당 장비의 방어력값

    public override void Use()
    {
        // 착용 장비에 등록 후 인벤토리에서 아이템 삭제
        EquipmentManager.instance.Equip(this);
        RemoveFromInventory();
    }
}
// 장비슬롯을 나타낼 enum변수
public enum EquipmentSlot { HEAD, BODY, SWORD, SHIELD };
