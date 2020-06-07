using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Inventory/Consumable Item")]
public class ConsumableItem : Item
{
    CharacterStats characterStats;
    public float recoveryHealth;

    // 아이템 사용 시 호출 될 함수
    public override void Use()
    {
        // 체력 회복
        characterStats = PlayerManager.instance.player.GetComponent<CharacterStats>();
        characterStats.currentHealth += recoveryHealth;

        // 회복된 체력이 최대 체력보다 크다면 최대체력값을 현재체력에 대입
        if (characterStats.currentHealth > characterStats.maxHealth)
            characterStats.currentHealth = characterStats.maxHealth;

        // 사용 후 인벤토리 내에서 아이템 삭제
        RemoveFromInventory();
    }
}
