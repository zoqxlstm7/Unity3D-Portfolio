using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemCtrl : MonoBehaviour
{
    public List<PickupItem> dropItems = new List<PickupItem>(); //드랍 될 아이템 리스트

    // 아이템 드랍 처리
    public void DropItem()
    {
        for (int i = 0; i < dropItems.Count; i++)
        {
            PickupItem newItem = Instantiate(dropItems[i], new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        }
    }
}
