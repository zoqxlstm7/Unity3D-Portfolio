using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";        // 아이템 이름
    public Sprite icon = null;                  // 아이템 아이콘
    public bool isConsumable = false;            // 소모성 아이템인지 여부

    public int num = 0;

    // 아이템 사용 시 호출 될 함수
    public virtual void Use()
    {
        //Debug.Log(itemName + " Use Item.");
    }

    // 아이템을 인벤토리에서 삭제하는 함수
    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }
}
